using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.CSharp;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VuePageCodeGenerator.AssemblyHelper;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PageGenerator;
using PageGenerator.CodeAnalysis;
using PageGenerator.PageCreate.VuePage;
using System.Net.Http;
using System.Web;

namespace VuePageCodeGenerator
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CodeGeneratorCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6e0722b8-da83-47fa-8d8a-b3b95168e444");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CodeGeneratorCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
            
        }
        private void menuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                IntPtr hierarchyPtr, selectionContainerPtr;
                uint projectItemId;
                IVsMultiItemSelect mis;
                IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
                monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

                IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
                if (hierarchy != null)
                {
                    object value;
                    hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_Name, out value);

                    if (value != null && value.ToString().EndsWith("AppServices.cs", StringComparison.OrdinalIgnoreCase))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CodeGeneratorCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
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
        public static void Initialize(Package package)
        {
            Instance = new CodeGeneratorCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
                Project project = ProjectTool.GetActiveProject();
                string itemPath = ProjectTool.GetSelectedItemPaths().FirstOrDefault();
                string solutionPath = ProjectTool.GetSolutionFolderPath();
                if (!itemPath.Contains("AppServices.cs"))
                {
                    throw new Exception("请选择应用服务类生成Page!");
                }

                CSharpCodeAnalysis codeAnalysis = new CSharpCodeAnalysis(itemPath);
                var methods = codeAnalysis.GetAllCSharpMethods();
                var propertys = codeAnalysis.GetAllCSharpPropertys();


                MainWindow mainWindow = new MainWindow(methods, solutionPath, itemPath);

                mainWindow.ShowActivated = true;
                mainWindow.Show();

            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    ex.Message,
                    "Error",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            }

            // Show a message box to prove we were here

            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);


        }
        
        
    }
}
