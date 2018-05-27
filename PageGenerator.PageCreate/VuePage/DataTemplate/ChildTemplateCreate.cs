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
        public ChildTemplateCreate(List<CSharpProperty> _formPropertys, List<CSharpProperty> _columnPropertys) {
            if (_formPropertys == null || _formPropertys.Count <= 0)
                throw new Exception("propertys is null");
            formPropertys = _formPropertys;
            if (_formPropertys == null || _formPropertys.Count <= 0)
                throw new Exception("propertys is null");
            columnPropertys = _columnPropertys;
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
        
        public string CreateFormTemplate() {
            
            StringBuilder formData = new StringBuilder();
            foreach (var item in formPropertys)
            {
                if (item.PropertyType == "string") {
                    formData.Append($"<FormItem label='{item.Description}' prop='{item.PropertyName}'><Input v-model='crudItem.{item.PropertyName}' placeholder = ''></Input></FormItem> ");
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
        public string CreateColumsTemplate() {
            List<Columns> columns = new List<Columns>();
            foreach (var item in columnPropertys)
            {
                columns.Add(new Columns()
                {
                    title=item.Description,
                    key=item.PropertyName,
                    sortable= "custom",
                     handle=""
                });
            }
            columns.Add(new Columns()
            {
                title= "操作",
                key= "action",
                sortable="",
                handle= "['edit', 'delete']"
            });
            return JsonConvert.SerializeObject(columns);
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
        public string CreateValidateTemplate() {
            List<RuleValidate> validates = new List<RuleValidate>();
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
    }
}
