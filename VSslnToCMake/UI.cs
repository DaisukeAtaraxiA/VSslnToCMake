/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace VSslnToCMake
{
    public sealed class UI
    {
        public static bool ShowOptionGUI(EnvDTE80.DTE2 dte,
                                         out string targetPlatform,
                                         out string[] targetConfigurations)
        {
            var platforms = new List<string>();
            var cfgNames = new List<string>();
            var slnBuild = dte.Solution.SolutionBuild as SolutionBuild2;
            foreach (SolutionConfiguration2 slnCfg in slnBuild.SolutionConfigurations)
            {
                platforms.Add(slnCfg.PlatformName);
                foreach (SolutionContext context in slnCfg.SolutionContexts)
                {
                    cfgNames.Add(context.ConfigurationName);
                }
            }

            targetPlatform = null;
            targetConfigurations = null;

            var form = new StartWindow();
            form.targetPlatforms = platforms.Where(x => x != "Any CPU").Distinct().ToArray();
            form.targetConfigurations = cfgNames.Distinct().ToArray();

            var wih = new System.Windows.Interop.WindowInteropHelper(form);
            wih.Owner = new System.IntPtr(dte.MainWindow.HWnd);
            form.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            if (form.ShowDialog() != true)
            {
                return false;
            }
            targetPlatform = form.SelectedPlatform();
            targetConfigurations = form.SelectedConfigurations();
            return true;
        }
    }
}
