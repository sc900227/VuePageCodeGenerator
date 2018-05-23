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
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VuePageCodeGenerator.AssemblyHelper;

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

                    if (value != null && value.ToString().EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
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
                string title = "CodeGeneratorCommand";
                IEnumerable<ProjectItem> projects = GetSelectedProject();
                
                ProjectItem project = projects.FirstOrDefault();
                var pro=project.ContainingProject.ProjectItems;
                List<string> names = new List<string>();
                foreach (MethodInfo met in pro.Item(1).GetType().GetMethods())
                {
                    names.Add(met.Name);
                }

                #region Test
                string strCS = @"D:\Repos\MyGeneDocument\MyDocumentManageNetCore.Domain\bin\Debug\netcoreapp2.0\MyDocumentManageNetCore.Domain.dll";
                string strDll = strCS.Substring(0, strCS.LastIndexOf(".")) + ".dll";

                //CodeDomProvider COD = new Microsoft.CSharp.CSharpCodeProvider();
                //COD = new Microsoft.CSharp.CSharpCodeProvider();
                //CompilerParameters COM = new CompilerParameters();
                ////生成DLL，True为生成exe文件,false为生成ＤＬＬ文件
                //COM.GenerateExecutable = false;
                //COM.OutputAssembly = strDll;
                ////把ＣＳ文件生成ＤＬＬ
                //CompilerResults COMR = COD.CompileAssemblyFromFile(COM, strCS);

                //下面我们就可以根据生成的Dll反射为相关对象，供我们使用了．
                AssemblyHandler assembly = new AssemblyHandler(strCS);
                AssemblyResult result=assembly.GetClassInfo("Entitys.TB_GeneInfo");
                //Type t = a.GetType("b");
                //object obj = Activator.CreateInstance(t);
                //t.GetMethod("run").Invoke(obj, null);
                //CodeSnippetCompileUnit code = new CodeSnippetCompileUnit();

                //Assembly assembly =Assembly.LoadFrom(project.ContainingProject.FullName);
                //Microsoft.Build.Evaluation.ProjectCollection pro = new Microsoft.Build.Evaluation.ProjectCollection();
                //var items = pro.LoadProject(project.ContainingProject.FullName);
                #endregion

                //List<string> names = new List<string>();
                //if (project.GetType().IsClass)
                //{
                //    MethodInfo[] methods=project.GetType().GetMethods();
                //    foreach (var item in methods)
                //    {
                //        names.Add(item.Name);
                //    }
                //    Form1 form1 = new Form1(names);
                //    form1.Show();
                //}
                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
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
            
        }

        
        public static IEnumerable<ProjectItem> GetSelectedProject(DTE2 dte = null)
        {
            var items = (Array)CodeGeneratorCommandPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;

                if (item != null && item.Properties != null)
                    yield return item;
            }
        }
        //<summary>Gets the full paths to the currently selected item(s) in the Solution Explorer.</summary>
        public static IEnumerable<string> GetSelectedItemPaths(DTE2 dte = null)
        {
            var items = (Array)CodeGeneratorCommandPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;

                if (item != null && item.Properties != null)
                    yield return item.Properties.Item("FullPath").Value.ToString();
            }
        }
        ///<summary>Gets the paths to all files included in the selection, including files within selected folders.</summary>
        public static IEnumerable<string> GetSelectedFilePaths()
        {
            return GetSelectedItemPaths()
                .SelectMany(p => Directory.Exists(p)
                                 ? Directory.EnumerateFiles(p, "*", SearchOption.AllDirectories)
                                 : new[] { p }
                           );
        }

        ///<summary>Gets the the currently selected project(s) in the Solution Explorer.</summary>
        public static IEnumerable<Project> GetSelectedProjects()
        {
            var items = (Array)CodeGeneratorCommandPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as Project;

                if (item != null)
                    yield return item;
            }
        }
        ///<summary>Gets the directory containing the active solution file.</summary>
        public static string GetSolutionFolderPath()
        {
            EnvDTE.Solution solution = CodeGeneratorCommandPackage.DTE.Solution;

            if (solution == null)
                return null;

            //if (string.IsNullOrEmpty(solution.FullName))
            //    return GetRootFolder();

            return Path.GetDirectoryName(solution.FullName);
        }
    }
}
