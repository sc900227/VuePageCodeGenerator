using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.PageCreate.VuePage
{
    public class VueCreateOption:CreateOption
    {
        /// <summary>
        /// 当前解决方案目录
        /// </summary>
        public string SolutionPath { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageTitle { get; set; }
        
        public VueCreateOption(string _solutionPath,string _pageTitle) {
            SolutionPath = _solutionPath;
            PageTitle = _pageTitle;
        }
    }
}
