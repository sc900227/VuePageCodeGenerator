using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VuePageCodeGenerator.CSharpCodeHelper
{
    public class CSharpMethod
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ReturnType { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public string ParameterType { get; set; }
    }
}
