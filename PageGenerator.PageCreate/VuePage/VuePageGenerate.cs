using Newtonsoft.Json;
using PageGenerator.CodeAnalysis;
using PageGenerator.PageCreate.VuePage.DataTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PageGenerator.PageCreate.VuePage
{
    public class VuePageGenerate:VuePageCreateBase
    {
        private VueCreateOption _option;
        private string _templatePath;
        private string _pageSavePath;
        private string _routerPath;
        public VuePageGenerate(VueCreateOption option,string templatePath,string pageSavePath,string routerPath) {
            _option = option;
            _templatePath = templatePath;
            _pageSavePath = pageSavePath;
            _routerPath = routerPath;
        }
        /// <summary>
        /// 创建页面路由
        /// </summary>
        /// <param name="routerItem"></param>
        public void CreateRouter(RouterItem routerItem) {
            string templateUrl = CSharpCodeAnalysis.SearchFileInSolution(_templatePath, "routerTemplate.js");
            string routerUrl = CSharpCodeAnalysis.SearchFileInSolution(_routerPath, "router.js");
            if (string.IsNullOrEmpty(routerUrl))
            {
                throw new Exception("Vue router.js No Finded!");
            }
            if (string.IsNullOrEmpty(templateUrl))
            {
                throw new Exception("Vue routerTemplate.js No Finded!");
            }
            _option.TemplateUrl = templateUrl;
            string content = string.Empty;
            using (FileStream stream = new FileStream(_option.TemplateUrl, FileMode.Open, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                content = reader.ReadToEnd();
            }
            //生成router.js内容
            var router = JsonConvert.SerializeObject(routerItem);
            var contentRouter = content.Replace("$RouterItem$", router);
            //保存rotuer.js
            _option.TemplateData = contentRouter;
            _option.SavePath = routerUrl;
            PageCreate(_option);
            //生成routerTemplate.js内容
            var routerTemplate = $"{JsonConvert.SerializeObject(routerItem)},$RouterItem$";
            var contentTemplate = content.Replace("$RouterItem$", routerTemplate);
            //保存routerTemplate.js
            _option.TemplateData = contentTemplate;
            _option.SavePath = templateUrl;
            PageCreate(_option);
        }
        /// <summary>
        /// 创建vueTable模板
        /// </summary>
        public void VueTemplateCreate() {
           VuePageCreate("crudTable", _pageSavePath, "crudTable");
            
        }
        /// <summary>
        /// 创建vue页面
        /// </summary>
        /// <param name="templateName">模板名称</param>
        /// <param name="savePath">vue文件保存地址</param>
        /// <param name="saveName">vue文件名</param>
        /// <param name="createFun"></param>
        public void VuePageCreate(string templateName,string savePath,string saveName,Func<string,string> createFun=null) {
            templateName= $"{templateName}Template.vue";
            saveName= $"{saveName}.vue";
            string templateUrl = CSharpCodeAnalysis.SearchFileInSolution(_templatePath, templateName);
            if (string.IsNullOrEmpty(templateUrl))
            {
                throw new Exception("Vue Template No Finded!");
            }
            _option.TemplateUrl = templateUrl;
            string content = string.Empty;
            using (FileStream stream = new FileStream(_option.TemplateUrl, FileMode.Open, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                content = reader.ReadToEnd();
            }
            
            if (createFun != null)
                content = createFun(content);
            _option.TemplateData = content;
            _option.SavePath =Path.Combine(savePath, saveName);
            PageCreate(_option);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formPropertys">form属性集合</param>
        /// <param name="columPropertys">查询列表属性集合</param>
        /// <param name="apiFuns">api方法字典</param>
        /// <param name="url">请求地址</param>
        /// <param name="pageName">保存的页面名称</param>
        public void VueTablePageCreate(List<CSharpProperty> formPropertys, List<CSharpProperty> columPropertys,Dictionary<string,string> apiFuns,string url,string pageName) {
            ChildTemplateCreate create = new ChildTemplateCreate(_option.PageTitle,formPropertys,columPropertys,apiFuns,url, pageName);
            //string formData = create.CreateFormTemplate();
            //string columnData = create.CreateColumsTemplate();
            //string validateData = create.CreateValidateTemplate();
            string savePath= Path.Combine(_pageSavePath, pageName);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            VuePageCreate("vuePage", savePath, pageName, (con) =>
            {
                return create.CreateTemplate(con);
                //con = con.Replace("$Title$", _option.PageTitle);
                //con = con.Replace("$FormItem$", formData);
                //con = con.Replace("$Columns$", columnData);
                //con = con.Replace("$RuleValidate$", validateData);
                //return con;
            });
            

        }
    }
}
