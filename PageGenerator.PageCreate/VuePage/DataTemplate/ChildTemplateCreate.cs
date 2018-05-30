using Newtonsoft.Json;
using PageGenerator.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.PageCreate.VuePage.DataTemplate
{
    public class ChildTemplateCreate
    {
        private List<CSharpProperty> formPropertys;
        private List<CSharpProperty> columnPropertys;
        private string templateTitle = string.Empty;
        private Dictionary<string, string> apiFuns;
        private string requestUrl = string.Empty;
        private string pageName = string.Empty;
        public ChildTemplateCreate(string _templateTitle,List<CSharpProperty> _formPropertys, List<CSharpProperty> _columnPropertys, Dictionary<string, string> _apiFuns,string url,string _pageName) {
            if (_formPropertys == null || _formPropertys.Count <= 0)
                throw new Exception("propertys is null");
            formPropertys = _formPropertys;
            if (_formPropertys == null || _formPropertys.Count <= 0)
                throw new Exception("propertys is null");
            columnPropertys = _columnPropertys;
            templateTitle = _templateTitle;
            apiFuns = _apiFuns;
            requestUrl = url;
            pageName = _pageName;
        }
        /*
         * <Form slot="modal-content" ref="crudItem"
                        :model="crudItem" 
                        :rules="ruleValidate" label-position="right" :label-width="100">
                      <FormItem label="试剂盒名称" prop="ReagentName">
                          <Input v-model="crudItem.ReagentName" placeholder="请输入试剂盒名称...">
                          </Input>
                      </FormItem>
                      <FormItem label="作用药物" prop="ForDrug">
                          <Input type="textarea" :rows="4" v-model="crudItem.ForDrug"
                                 placeholder="请简单的描述一下当前的试剂盒作用药物...">
                          </Input>
                      </FormItem>
                  </Form>
         */
        public string CreateTemplate(string content) {
            string formData = CreateFormTemplate();
            string columnData = CreateColumsTemplate();
            string validateData = CreateValidateTemplate();
            string crudItem= CreateCrudItem();
            string paramHead = GetCamelCaseProperty(pageName.Replace("InfoPage", ""));
            string param = CreateParams();
            content = content.Replace("$Title$", templateTitle);
            content = content.Replace("$PageName$", pageName);
            content = content.Replace("$Url$", requestUrl);
            content = content.Replace("$FormItem$", formData);
            content = content.Replace("$Columns$", columnData);
            content = content.Replace("$RuleValidate$", validateData);
            content = content.Replace("$CrudItem$", crudItem);
            content = content.Replace("$ParamHead$", paramHead);
            content = content.Replace("$Params$", param);
            content = content.Replace("$Select$", apiFuns["Select"]);
            content = content.Replace("$Insert$", apiFuns["Insert"]);
            content = content.Replace("$Update$", apiFuns["Update"]);
            content = content.Replace("$Delete$", apiFuns["Delete"]);
            return content;

        }
        public virtual string CreateFormTemplate() {
            
            StringBuilder formData = new StringBuilder();
            foreach (var item in formPropertys)
            {
                if (item.PropertyName != "Id") {
                    formData.Append($"<FormItem label='{item.Description}' prop='{item.PropertyName}'><Input v-model='crudItem.{item.PropertyName}' placeholder=''></Input></FormItem>");
                }
            }
            return formData.ToString();
        }
        /*[
               { title: "序号",
                   key: 'Id',
                   sortable: 'custom'
               },
               { title: "试剂盒名称", 
                   key: 'ReagentName',
                   sortable: 'custom'
               },
               { title: "作用药物", 
                   key: 'ForDrug'
               },
               {
                   title: '操作',
                   key: 'action',
                   handle:['edit', 'delete']
               }
           ]*/
        public virtual string CreateColumsTemplate() {
            StringBuilder colData = new StringBuilder();
            if (columnPropertys.FirstOrDefault(a => a.PropertyName == "Id") == null) {
                var item = new Columns()
                {
                    title="编号",
                    key="id",
                    sortable = "custom",
                    handle = ""
                };
                colData.Append($"{JsonConvert.SerializeObject(item)},");
            }
            foreach (var item in columnPropertys)
            {
                var colItem = new Columns()
                {
                    title = item.Description,
                    key = GetCamelCaseProperty(item.PropertyName),
                    sortable = "custom",
                    handle = ""
                };
                colData.Append($"{JsonConvert.SerializeObject(colItem)},");
                
            }
            colData.Append("{ 'title': '操作','key': 'action','handle':['edit', 'delete']}");
            return $"[{colData.ToString()}]";
        }
        /// <summary>
        /// 返回驼峰式字符
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public string GetCamelCaseProperty(string property) {
            return property.Substring(0, 1).ToLower() + property.Substring(1);
        }
        /*
         * {
                ReagentName: [
                  { required: true, message: "试剂盒名称不能为空", trigger: 'blur'}
                ],
                ForDrug:[
                  { required: true, message: "作用药物不能为空", trigger: 'buur'}
                ]
            }
         */
        public virtual string CreateValidateTemplate() {
            StringBuilder valData = new StringBuilder();
            foreach (var item in formPropertys)
            {
                var valItem = new RuleValidate() {
                    required=true,
                    message=$"{item.Description}不能为空！",
                    trigger="blur"
                };
                valData.Append($"{item.PropertyName}:[{JsonConvert.SerializeObject(valItem)}],");
            }
            string result = valData.ToString().Substring(0, valData.Length - 1);
            return result;
        }
        /*
         {
        Id: null,
        ReagentName: null,
        ForDrug: null
      }
         */
        public virtual string CreateCrudItem() {
            StringBuilder itemData = new StringBuilder();
            //var propertyId = formPropertys.Where(a => a.PropertyName == "ID");
            //if (propertyId == null|| propertyId.Count()<=0) {
            //    itemData.Append("Id:null,");
            //}
            foreach (var item in formPropertys)
            {
                itemData.Append($"{item.PropertyName}:null,");
            }
            string result = itemData.ToString().Substring(0, itemData.Length - 1);
            return result;
        }
        /*
         {
        ID: vm.crudItem.Id,
        ReagentName: vm.crudItem.ReagentName,
        ForDrug: vm.crudItem.ForDrug
      }
         */
        public virtual string CreateParams() {
            StringBuilder paramsData = new StringBuilder();
            //var propertyId = formPropertys.Where(a => a.PropertyName == "ID");
            //if (propertyId == null || propertyId.Count() <= 0)
            //{
            //    paramsData.Append($"ID:vm.crudItem.Id,");
            //}
            foreach (var item in formPropertys)
            {
                paramsData.Append($"{item.PropertyName}:vm.crudItem.{item.PropertyName},");
            }
            string result = paramsData.ToString().Substring(0, paramsData.Length - 1);
            return result;
        }
    }
}
