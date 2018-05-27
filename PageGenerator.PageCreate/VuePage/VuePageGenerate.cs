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
        public VuePageGenerate(VueCreateOption option) {
            _option = option;
        }
        public void VueTemplateCreate() {
            //CreateOption option = new VueCreateOption();
            VuePageCreate("crudTable", "crudTable");
            
        }
        public void VuePageCreate(string templateName,string saveName,Func<string,string> createFun=null) {
            templateName= $"{templateName}Template.vue";
            saveName= $"{saveName}.vue";
            string templateUrl = CSharpCodeAnalysis.SearchFileInSolution(_option.SolutionPath, templateName);
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
            _option.SavePath = @"F:\Source\Repos\"+saveName;
            PageCreate(_option);
        }
        public void VueTablePageCreate(List<CSharpProperty> formPropertys, List<CSharpProperty> columPropertys) {
            ChildTemplateCreate create = new ChildTemplateCreate(formPropertys,columPropertys);
            string formData = create.CreateFormTemplate();
            string columnData = create.CreateColumsTemplate();
            string validateData = create.CreateValidateTemplate();
            VuePageCreate("vuePage", "vuePage", (con) =>
            {
                con = con.Replace("$Title$", _option.PageTitle);
                con = con.Replace("$FormItem$", formData);
                con = con.Replace("$Columns$", columnData);
                con = con.Replace("$RuleValidate$", validateData);
                return con;
            });
            //string templateUrl = CSharpCodeAnalysis.SearchFileInSolution(_option.SolutionPath, "vuePageTemplate.vue");// "/VuePageTemplate/crudTable.vue";
            //if (string.IsNullOrEmpty(templateUrl))
            //{
            //    throw new Exception("No Find Vue Template!");
            //}
            //string content = string.Empty;
            //using (FileStream stream = new FileStream(_option.TemplateUrl, FileMode.Open, FileAccess.Read))
            //{
            //    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            //    content = reader.ReadToEnd();
                
            //}
            //_option.TemplateUrl = templateUrl;
            //content = content.Replace("$Title$",_option.PageTitle);
            //content = content.Replace("$FormItem$", formData);
            //content = content.Replace("$Columns$", columnData);
            //content = content.Replace("$RuleValidate$",validateData);
            //_option.VueTemplateData = content;
            //_option.SavePath = @"F:\Source\Repos\VuePageCodeGenerator\vuePage.vue";
            //CreatePage(_option);

        }
    }
}
