/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using VSslnToCMake;

namespace VSslnToCMakeCUI
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            int vsProcessId = 0;
            bool showGUI = false;

            // Parse command line options.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-pid")
                {
                    if (i + 1 >= args.Length ||
                        !Int32.TryParse(args[i + 1], out vsProcessId))
                    {
                        PrintUsage();
                        return 1;
                    }
                    ++i;
                }
                else if (args[i] == "-gui")
                {
                  showGUI = true;
                }
            }
            if (vsProcessId == 0)
            {
                PrintUsage();
                return 1;
            }

            // Attach to Visual Studio.
            EnvDTE.DTE dte = GetDTE(vsProcessId, 120);
            if (dte == null)
            {
                System.Console.WriteLine(
                    "ERROR: Failed to attach Visual Studio.");
                return 1;
            }

            // Do conversion.
            AbstractConverter converter = new ConverterVs2015();
            converter.SetLogger(new StdLogger());
            if (showGUI)
            {
                if (UI.ShowOptionGUI(dte as EnvDTE80.DTE2,
                                     out string targetPlatform,
                                     out string[] targetConfigurations))
                {
                    converter.Platform = targetPlatform;
                    converter.TargetConfigurations = targetConfigurations;
                }
                else
                {
                    return 1;
                }
            }
            converter.Convert(dte);

            return 0;
        }

        private static void PrintUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine(  "Usage:");
            sb.AppendLine();
            sb.AppendFormat("  {0} -pid process_id [-gui]",
                            System.IO.Path.GetFileName(
                                Assembly.GetExecutingAssembly().Location));
            sb.AppendLine();
            sb.AppendLine();
            System.Console.Write(sb.ToString());
        }

        private static EnvDTE.DTE GetDTE(int processId, int timeout)
        {
            EnvDTE.DTE res = null;
            DateTime startTime = DateTime.Now;

            while (res == null &&
                   DateTime.Now.Subtract(startTime).Seconds < timeout)
            {
                System.Threading.Thread.Sleep(1000);
                res = GetDTE(processId);
            }
            return res;
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// Gets the DTE object from any devenv process.
        /// </summary>
        /// <param name="processId">
        /// <returns>
        /// Retrieved DTE object or <see langword="null"> if not found.
        /// </see></returns>
        private static EnvDTE.DTE GetDTE(int processId)
        {
            object runningObject = null;

            IBindCtx bindCtx = null;
            IRunningObjectTable rot = null;
            IEnumMoniker enumMonikers = null;

            try
            {
                Marshal.ThrowExceptionForHR(CreateBindCtx(reserved: 0, ppbc: out bindCtx));
                bindCtx.GetRunningObjectTable(out rot);
                rot.EnumRunning(out enumMonikers);

                IMoniker[] moniker = new IMoniker[1];
                IntPtr numberFetched = IntPtr.Zero;
                while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                {
                    IMoniker runningObjectMoniker = moniker[0];

                    string name = null;

                    try
                    {
                        if (runningObjectMoniker != null)
                        {
                            runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Do nothing, there is something in the ROT that we do not have access to.
                    }

                    Regex monikerRegex = new Regex(@"!VisualStudio.DTE\.\d+\.\d+\:" + processId, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrEmpty(name) && monikerRegex.IsMatch(name))
                    {
                        Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                        break;
                    }
                }
            }
            finally
            {
                if (enumMonikers != null)
                {
                    Marshal.ReleaseComObject(enumMonikers);
                }

                if (rot != null)
                {
                    Marshal.ReleaseComObject(rot);
                }

                if (bindCtx != null)
                {
                    Marshal.ReleaseComObject(bindCtx);
                }
            }

            return runningObject as EnvDTE.DTE;
        }
    }
}
