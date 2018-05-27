using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.CodeAnalysis
{
    public class CSharpCodeAnalysis
    {
        private string _strFilePath;
        public CSharpCodeAnalysis(string strFilePath)
        {
            _strFilePath = strFilePath;
        }
        public static string SearchFileInSolution(string solutionPath, string fileName)
        {
            string[] filePath = Directory.GetFiles(solutionPath, fileName, SearchOption.AllDirectories);
            if (filePath != null && filePath.Length > 0)
            {
                return filePath.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        private string GetContent()
        {
            string content = string.Empty;
            using (FileStream stream = new FileStream(_strFilePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(stream, Encoding.Default);
                content = reader.ReadToEnd();
            }
            return content;
        }
        public List<CSharpMethod> GetAllCSharpMethods()
        {
            List<CSharpMethod> cSharpMethods = new List<CSharpMethod>();
            var methods = GetAllMethods().Where(a => a.Modifiers.ToString().Contains("public"));
            foreach (var item in methods)
            {
                cSharpMethods.Add(new CSharpMethod()
                {
                    MethodName = item.Identifier.ValueText,
                    ParameterType = item.ParameterList.Parameters.FirstOrDefault() != null ? item.ParameterList.Parameters.FirstOrDefault().Type.ToString() : "",
                    ParameterName = item.ParameterList.Parameters.FirstOrDefault() != null ? item.ParameterList.Parameters.FirstOrDefault().Identifier.ValueText : "",
                    ReturnType = GetReturnType(item.ReturnType.ToString())
                });
            }
            return cSharpMethods;
        }
        public string RemovePropertyChar(string property,string strChar1,string strChar2)
        {
            if (!string.IsNullOrEmpty(property))
            {
                if (property.Contains(strChar1))
                {
                    var propertyTemp = String.Join("", property.Substring(property.IndexOf(strChar1) + 1).Reverse());
                    property = String.Join("", propertyTemp.Substring(propertyTemp.IndexOf(strChar2) + 1).Reverse());
                }
            }
            return property;
        }
        private string GetReturnType(string type)
        {
            //string returnType = string.Empty;
            if (type.Contains("Task<"))
            {
                string returnTypeTemp = String.Join("", type.Substring(type.IndexOf("<") + 1).Reverse());
                string returnType = String.Join("", returnTypeTemp.Substring(returnTypeTemp.IndexOf(">") + 1).Reverse());
                return returnType;
            }
            if (type.Contains("Task"))
            {
                return "";
            }
            return type;
        }
        public List<MethodDeclarationSyntax> GetAllMethods()
        {
            string txt = GetContent();
            var syntaxTree = CSharpSyntaxTree.ParseText(@txt);
            var root = syntaxTree.GetRoot();
            var methods = root.DescendantNodes()
                             .OfType<MethodDeclarationSyntax>().ToList();
            return methods;
        }
        public MethodDeclarationSyntax GetMethod(string methodName)
        {
            string txt = GetContent();
            var syntaxTree = CSharpSyntaxTree.ParseText(@txt);
            var root = syntaxTree.GetRoot();
            var method = root.DescendantNodes()
                             .OfType<MethodDeclarationSyntax>()
                             .Where(md => md.Identifier.ValueText.Equals(methodName))
                             .FirstOrDefault();
            return method;
        }
        public List<CSharpProperty> GetAllCSharpPropertys()
        {
            List<CSharpProperty> cSharpProperties = new List<CSharpProperty>();
            var propertys = GetAllPropertys();
            try
            {
                foreach (var item in propertys)
                {
                    var attrs = item.AddAttributeLists().AttributeLists;
                    var des = "";
                    if (attrs != null && attrs.Count > 0)
                    {
                        var attr = attrs.Where(a => a.ToString().Contains("Description"));
                        if (attr != null)
                            des = attr.FirstOrDefault().ToString();
                    }
                    des = RemovePropertyChar(des, "(", ")").Replace("\\", "");

                    cSharpProperties.Add(new CSharpProperty()
                    {
                        PropertyName = item.Identifier.ValueText,
                        PropertyType = item.Type.ToString(),
                        Description = des
                    });
                }
                return cSharpProperties;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }
        public List<PropertyDeclarationSyntax> GetAllPropertys()
        {
            string txt = GetContent();
            var syntaxTree = CSharpSyntaxTree.ParseText(@txt);
            var root = syntaxTree.GetRoot();
            var propertys = root.DescendantNodes()
                               .OfType<PropertyDeclarationSyntax>().ToList();

            return propertys;
        }
        public AccessorDeclarationSyntax GetPropertyGetter(string propertyName)
        {
            string txt = GetContent();
            var syntaxTree = CSharpSyntaxTree.ParseText(@txt);
            var root = syntaxTree.GetRoot();
            var property = root.DescendantNodes()
                               .OfType<PropertyDeclarationSyntax>()
                               .Where(md => md.Identifier.ValueText.Equals(propertyName))
                               .FirstOrDefault();
            var getter = property.AccessorList.Accessors.First(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
            return getter;
        }
        //public List<CSharpMethod> GetAllMethodNames()
        //{
        //    List<string> methodNames = new List<string>();
        //    List<CSharpMethod> methods = new List<CSharpMethod>();
        //    var strMethodLines = File.ReadAllLines(_strFilePath)
        //                                .Where(a => (a.Contains("protected") ||
        //                                            a.Contains("private") ||
        //                                            a.Contains("public")) &&
        //                                            !a.Contains("_") && !a.Contains("class"));
        //    foreach (var item in strMethodLines)
        //    {
        //        if (item.IndexOf("(") != -1)
        //        {
        //            string strTemp = String.Join("", item.Substring(0, item.IndexOf("(")).Reverse());
        //            string methodName=String.Join("", strTemp.Substring(0, strTemp.IndexOf(" ")).Reverse());
        //            string parameterTypeTemp = item.Substring(item.IndexOf("(") + 1);
        //            string parameterType = String.Join("", parameterTypeTemp.Substring(0, parameterTypeTemp.IndexOf(" ")));
        //            string returnType = string.Empty;
        //            if (item.Contains("Task<"))
        //            {
        //                string returnTypeTemp= String.Join("", item.Substring(item.IndexOf("<")+1).Reverse());
        //                returnType= String.Join("", returnTypeTemp.Substring(returnTypeTemp.IndexOf(">")+1).Reverse());
        //            }
        //            methods.Add(new CSharpMethod()
        //            {
        //                MethodName=methodName,
        //                ParameterType=parameterType,
        //                ReturnType=returnType
        //            });
        //        }
        //    }
        //    return methods.Distinct().ToList();
        //}
    }
}
