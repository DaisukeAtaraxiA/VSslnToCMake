/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;

namespace VSslnToCMakePlugin
{
    class VSOutputLogger : VSslnToCMake.Logger
    {
        private OutputWindowPane outputPane = null;

        public VSOutputLogger(EnvDTE80.DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OutputWindowPanes panes =
                dte.ToolWindows.OutputWindow.OutputWindowPanes;
            outputPane = null;
            try
            {
                outputPane = panes.Item("VSslnToCMake");
            }
            catch (ArgumentException)
            {
                panes.Add("VSslnToCMake");
                outputPane = panes.Item("VSslnToCMake");
            }
        }

        public override void Warn(string message)
        {
            base.Warn("Warning: " + message);
        }
        public override void Error(string message)
        {
            base.Error("Error: " + message);
        }

        protected override void WriteLine(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!message.EndsWith(Environment.NewLine))
            {
                message += Environment.NewLine;
            }
            outputPane.OutputString(message);
        }
    }
}
