using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.PageCreate
{
    public class CreateOption
    {
        /// <summary>
        /// 模板地址
        /// </summary>
        public string TemplateUrl { get; set; }
        /// <summary>
        /// Vue模板数据
        /// </summary>
        public string TemplateData { get; set; }
        /// <summary>
        /// 保存目录
        /// </summary>
        public string SavePath { get; set; }
    }
}
