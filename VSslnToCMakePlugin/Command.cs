/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

using System;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using EnvDTE80;

namespace VSslnToCMakePlugin
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Command
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1aa475c6-5861-4387-91f1-9e701f312faf");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private EnvDTE.DTE dte;
        private static VSOutputLogger logger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private Command(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            menuItem.BeforeQueryStatus += (sender, evt) =>
            {
                if (this.dte != null)
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    var item = sender as OleMenuCommand;
                    item.Enabled = dte.Solution.IsOpen;
                }
            };
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Command Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Command's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new Command(package, commandService);
            Instance.dte = await package.GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (logger == null)
            {
                logger = new VSOutputLogger(dte as EnvDTE80.DTE2);
            }

            if (!VSslnToCMake.UI.ShowOptionGUI(
                    dte as EnvDTE80.DTE2, out string targetPlatform,
                    out string[] targetConfigurations))
            {
                return;
            }

            VSslnToCMake.AbstractConverter converter = null;
            if (dte.Version == "14.0")
            {
                converter = new VSslnToCMake.ConverterVs2015();
            }
            else if (dte.Version == "15.0")
            {
                converter = new VSslnToCMake.ConverterVs2017();
            }

            converter.SetLogger(logger);
            converter.Platform = targetPlatform;
            converter.TargetConfigurations = targetConfigurations;
            converter.Convert(dte);
        }
    }
}
