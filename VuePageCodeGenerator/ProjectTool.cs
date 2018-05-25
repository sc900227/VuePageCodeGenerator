using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuePageCodeGenerator
{
    public class ProjectTool
    {
        
        /// <summary>
        /// Gets the Active project
        /// </summary>
        /// <returns></returns>
        public static Project GetActiveProject()
        {
            Array projects = (Array)CodeGeneratorCommandPackage.DTE.ActiveSolutionProjects;
            if (projects != null && projects.Length > 0)
            {
                return projects.GetValue(0) as Project;
            }
            projects = (Array)CodeGeneratorCommandPackage.DTE.Solution.SolutionBuild.StartupProjects;
            if (projects != null && projects.Length >= 1)
            {
                return projects.GetValue(0) as Project;
            }
            projects = (Array)CodeGeneratorCommandPackage.DTE.Solution.Projects;
            if (projects != null && projects.Length > 0)
            {
                return projects.GetValue(0) as Project;
            }
            return null;
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
