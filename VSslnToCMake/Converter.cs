/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.VCProjectEngine;

namespace VSslnToCMake
{
    static class Constants
    {
        public const string CMAKE_REQUIRED_VERSION = "3.12.2";
    }

#if (VS2015)
    public class ConverterVs2015 : AbstractConverter
#elif (VS2017)
    public class ConverterVs2017 : AbstractConverter
#endif
    {
#if (VS2015)
        public ConverterVs2015()
#elif (VS2017)
        public ConverterVs2017()
#endif
        {
            Platform = "x64";
        }

        public override bool Convert(EnvDTE.DTE dte)
        {
            var solutionInfoList = VerifySolution(dte);
            if (solutionInfoList == null)
            {
                return false;
            }

            logger.Info("Converting the projects");

            var cmProjects = new List<CMProject>();
            foreach (var solutionInfo in solutionInfoList)
            {
                var cmProject = new CMProject(solutionInfo.project);
                cmProject.setLogger(logger);
                cmProject.Platform = Platform;
                cmProject.BuildConfigurations =
                    solutionInfo.cfgs.Select(x => x.prjCfgName).ToArray();
                foreach (var cfg in solutionInfo.cfgs)
                {
                    cmProject.SetSolutionConfigurationName(cfg.prjCfgName,
                                                         cfg.slnCfgName);
                }
                if (!cmProject.Prepare())
                {
                    return false;
                }
                cmProjects.Add(cmProject);
            }

            var solutionDir = Path.GetDirectoryName(dte.Solution.FullName);

            foreach (var cmProject in cmProjects)
            {
                if (!cmProject.Convert(solutionDir, cmProjects))
                {
                    return false;
                }
            }

            logger.Info("Converting the projects - done");

            // Output CMakeLists.txt for the solution file.
            logger.Info("Converting the solution");
            logger.Info($"--- Converting {dte.Solution.FullName} ---");
            var cmakeListsPath = Path.Combine(solutionDir, "CMakeLists.txt");
            var sw = new StreamWriter(cmakeListsPath);
            sw.WriteLine($"cmake_minimum_required(VERSION {Constants.CMAKE_REQUIRED_VERSION})");
            sw.WriteLine();
            sw.WriteLine("project({0})",
                         Path.GetFileNameWithoutExtension(
                             dte.Solution.FileName));
            sw.WriteLine();
            foreach (var cmProject in cmProjects)
            {
                var projectDir = Path.GetDirectoryName(
                    cmProject.Project.FileName);
                var relativePath =
                    Utility.ToRelativePath(projectDir, solutionDir);
                sw.WriteLine($"add_subdirectory({relativePath})");
            }

            sw.Close();
            logger.Info($"  {Path.GetFileNameWithoutExtension(dte.Solution.FullName)} -> {cmakeListsPath}");

            logger.Info("Converting the solution - done");
            return true;
        }

        class SolutionInfo
        {
            public Project project;
            public (string slnCfgName, string prjCfgName)[] cfgs;
        }

        private List<SolutionInfo> VerifySolution(EnvDTE.DTE dte)
        {
            logger.Info("Checking the solution file");

            // Verfify the target platform
            if (Platform == "Any CPU")
            {
                logger.Error("Platform 'Any CPU' is not supported.");
                return null;
            }

            // Verify that project configurations are same.
            logger.Info("  Verifying that the configurations of the solution and projects match.");
            var slnBuild = dte.Solution.SolutionBuild as SolutionBuild2;

            if (TargetConfigurations == null)
            {
                TargetConfigurations = slnBuild.SolutionConfigurations
                                       .Cast<SolutionConfiguration2>()
                                       .Where(x => x.PlatformName == Platform)
                                       .Select(x => x.Name).ToArray();
            }

            var projectNamesList = new List<List<string>>();
            var cfgNamesList = new List<List<(string slnCfgName, string prjCfgName)>>();
            SolutionConfigurations slnCfgs = slnBuild.SolutionConfigurations;
            foreach (SolutionConfiguration2 slnCfg in slnCfgs)
            {
                if (slnCfg.PlatformName != Platform ||
                    !TargetConfigurations.Contains(slnCfg.Name))
                {
                    continue;
                }

                var projectNames = new List<string>();
                var cfgNames =
                    new List<(string slnCfgName, string projectCfgName)>();
                foreach (SolutionContext context in slnCfg.SolutionContexts)
                {
                    if (!context.ShouldBuild)
                    {
                        continue;
                    }
                    if (context.PlatformName != Platform)
                    {
                        logger.Error($"The platform of {context.ProjectName} does not match {Platform}.");
                        return null;
                    }
                    if (context.ConfigurationName != slnCfg.Name)
                    {
                        logger.Info($"  Configuration of {context.ProjectName} does not match ones of the solution.");
                        logger.Info($"    Solution: {slnCfg.Name}");
                        logger.Info($"    Project: {context.ConfigurationName}");
                        logger.Info($"  The configuration name '{context.ConfigurationName}' is replaced by '{slnCfg.Name}'.");
                    }
                    projectNames.Add(context.ProjectName);
                    cfgNames.Add((slnCfg.Name, context.ConfigurationName));
                }

                if (projectNames.Count == 0)
                {
                    logger.Error(
                        $"No project to build contains in configuration {slnCfg.Name}");
                    return null;
                }

                projectNamesList.Add(projectNames);
                cfgNamesList.Add(cfgNames);
            }
            
            if (projectNamesList.Count == 0)
            {
                logger.Error($"The solution file does not contain C/C++ projects on platform {Platform}.");
                return null;
            }

            for (int i = 0; i < projectNamesList.Count; i++)
            {
                projectNamesList[i].Sort();
                if (i > 0)
                {
                    if (!projectNamesList[i].SequenceEqual(projectNamesList[0]))
                    {
                        logger.Error($"The project configurations are different.");
                        return null;
                    }
                }
            }

            //
            string slnDir = Path.GetDirectoryName(dte.Solution.FullName);
            var projectsFullPath = projectNamesList[0].ConvertAll(
                x => Utility.NormalizePath(Path.Combine(slnDir, x))
                     .ToLower());

            Projects projects = dte.Solution.Projects;

            // Verifying existance of VC++ project, and platform.
            var solutionInfoList = new List<SolutionInfo>();
            foreach (Project project in projects)
            {
                System.Console.WriteLine(project.Name);
                System.Console.WriteLine(project.FileName);
                System.Console.WriteLine(project.FullName);
                VCProject vcprj = project.Object as VCProject;
                if (vcprj == null)
                {
                    logger.Warn(
                        $"Project '{project.Name}' is not a Visual C++ project.");
                    continue;
                }

                var index = projectsFullPath.FindIndex(
                    x => x == project.FullName.ToLower());
                if (index < 0)
                {
                    continue;
                }

                var solutionInfo = new SolutionInfo();
                solutionInfo.project = project;
                solutionInfo.cfgs = cfgNamesList.Select(
                    x => (x[index].slnCfgName, x[index].prjCfgName)).ToArray();

                solutionInfoList.Add(solutionInfo);
            }
            if (solutionInfoList.Count() == 0)
            {
                logger.Error("No Visual C++ projects to build.");
                return null;
            }

            logger.Info("Checking the solution file - done");
            return solutionInfoList;
        }
    }
}
