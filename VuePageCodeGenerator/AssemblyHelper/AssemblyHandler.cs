using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VuePageCodeGenerator.AssemblyHelper
{
    public class AssemblyHandler
    {
        string _path = string.Empty;
        public AssemblyHandler(string path) {
            _path = path;
        }
        /// <summary>  
        /// 获取程序集名称列表  
        /// </summary>  
        public AssemblyResult GetAssemblyName()
        {
            AssemblyResult result = new AssemblyResult();
            string[] dicFileName = Directory.GetFileSystemEntries(_path);
            if (dicFileName != null)
            {
                List<string> assemblyList = new List<string>();
                foreach (string name in dicFileName)
                {
                    assemblyList.Add(name.Substring(name.LastIndexOf('/') + 1));
                }
                result.AssemblyName = assemblyList;
            }
            return result;
        }

        /// <summary>  
        /// 获取程序集中的类名称  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        public AssemblyResult GetClassName(string assemblyName="")
        {
            AssemblyResult result = new AssemblyResult();
            assemblyName = !string.IsNullOrEmpty(assemblyName) ? _path + assemblyName : _path;
            if (!String.IsNullOrEmpty(assemblyName))
            {
                
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                Type[] ts = assembly.GetTypes();
                List<string> classList = new List<string>();
                foreach (Type t in ts)
                {
                    //classList.Add(t.Name);  
                    classList.Add(t.FullName);
                }
                result.ClassName = classList;
            }
            return result;
        }

        /// <summary>  
        /// 获取类的属性、方法  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        /// <param name="className">类名</param>  
        public AssemblyResult GetClassInfo(string className, string assemblyName = "")
        {
            AssemblyResult result = new AssemblyResult();
            assemblyName = !string.IsNullOrEmpty(assemblyName) ? _path + assemblyName : _path;
            if (!String.IsNullOrEmpty(assemblyName) && !String.IsNullOrEmpty(className))
            {
               
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                Type type = assembly.GetType(className, true, true);
                if (type != null)
                {
                    //类的属性  
                    List<string> propertieList = new List<string>();
                    PropertyInfo[] propertyinfo = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (PropertyInfo p in propertyinfo)
                    {
                        propertieList.Add(p.ToString());
                    }
                    result.Properties = propertieList;

                    //类的方法  
                    List<string> methods = new List<string>();
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (MethodInfo mi in methodInfos)
                    {
                        methods.Add(mi.Name);
                        //方法的参数  
                        //foreach (ParameterInfo p in mi.GetParameters())  
                        //{  

                        //}  
                        //方法的返回值  
                        //string returnParameter = mi.ReturnParameter.ToString();  
                    }
                    result.Methods = methods;
                }
            }
            return result;
        }

    }
}
