using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.Windows.Controls;

namespace VuePageCodeGenerator
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ProjectPropertiesToolWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6e0722b8-da83-47fa-8d8a-b3b95168e444");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPropertiesToolWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ProjectPropertiesToolWindowCommand(Package package)
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
                var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ProjectPropertiesToolWindowCommand Instance
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
            Instance = new ProjectPropertiesToolWindowCommand(package);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        //private void ShowToolWindow(object sender, EventArgs e)
        //{
        //    // Get the instance number 0 of this tool window. This window is single instance so this instance
        //    // is actually the only one.
        //    // The last flag is set to true so that if the tool window does not exists it will be created.
        //    ToolWindowPane window = this.package.FindToolWindow(typeof(ProjectPropertiesToolWindow), 0, true);
        //    if ((null == window) || (null == window.Frame))
        //    {
        //        throw new NotSupportedException("Cannot create tool window");
        //    }

        //    IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
        //    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        //}

        private void ShowToolWindow(object sender, EventArgs e)
        {
            ToolWindowPane window = this.package.FindToolWindow(typeof(ProjectPropertiesToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create window.");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            // Get the tree view and populate it if there is a project open.  
            ProjectPropertiesToolWindowControl control = (ProjectPropertiesToolWindowControl)window.Content;
            TreeView treeView = control.treeView;

            // Reset the TreeView to 0 items.  
            treeView.Items.Clear();

            DTE dte = (DTE)this.ServiceProvider.GetService(typeof(DTE));
            Projects projects = dte.Solution.Projects;
            if (projects.Count == 0)   // no project is open  
            {
                TreeViewItem item = new TreeViewItem();
                item.Name = "Projects";
                item.ItemsSource = new string[] { "no projects are open." };
                item.IsExpanded = true;
                treeView.Items.Add(item);
                return;
            }

            Project project = projects.Item(1);
            TreeViewItem item1 = new TreeViewItem();
            item1.Header = project.Name + "Properties";
            treeView.Items.Add(item1);

            foreach (Property property in project.Properties)
            {
                TreeViewItem item = new TreeViewItem();
                item.ItemsSource = new string[] { property.Name };
                item.IsExpanded = true;
                treeView.Items.Add(item);
            }
        }

    }
}
