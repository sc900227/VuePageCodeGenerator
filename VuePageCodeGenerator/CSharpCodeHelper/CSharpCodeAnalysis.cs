using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuePageCodeGenerator.CSharpCodeHelper
{
    public class CSharpCodeAnalysis
    {
        private string _strFilePath;
        public CSharpCodeAnalysis(string strFilePath) {
            _strFilePath = strFilePath;
        }

        public string GetMethod(string filename, string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(filename);
            var root = syntaxTree.GetRoot();
            var method = root.DescendantNodes()
                             .OfType<MethodDeclarationSyntax>();
                             //.Where(md => md.Identifier.ValueText.Equals(methodName))
                             //.FirstOrDefault();
            return method.ToString();
        }

        public string GetPropertyGetter(string filename, string propertyName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(filename);
            var root = syntaxTree.GetRoot();
            var property = root.DescendantNodes()
                               .OfType<PropertyDeclarationSyntax>()
                               .Where(md => md.Identifier.ValueText.Equals(propertyName))
                               .FirstOrDefault();
            var getter = property.AccessorList.Accessors.First(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
            return getter.ToString();
        }
        public List<CSharpMethod> GetAllMethodNames()
        {
            List<string> methodNames = new List<string>();
            List<CSharpMethod> methods = new List<CSharpMethod>();
            var strMethodLines = File.ReadAllLines(_strFilePath)
                                        .Where(a => (a.Contains("protected") ||
                                                    a.Contains("private") ||
                                                    a.Contains("public")) &&
                                                    !a.Contains("_") && !a.Contains("class"));
            foreach (var item in strMethodLines)
            {
                if (item.IndexOf("(") != -1)
                {
                    string strTemp = String.Join("", item.Substring(0, item.IndexOf("(")).Reverse());
                    string methodName=String.Join("", strTemp.Substring(0, strTemp.IndexOf(" ")).Reverse());
                    string parameterTypeTemp = item.Substring(item.IndexOf("(") + 1);
                    string parameterType = String.Join("", parameterTypeTemp.Substring(0, parameterTypeTemp.IndexOf(" ")));
                    string returnType = string.Empty;
                    if (item.Contains("Task<"))
                    {
                        string returnTypeTemp= String.Join("", item.Substring(item.IndexOf("<")+1).Reverse());
                        returnType= String.Join("", returnTypeTemp.Substring(returnTypeTemp.IndexOf(">")+1).Reverse());
                    }
                    methods.Add(new CSharpMethod()
                    {
                        MethodName=methodName,
                        ParameterType=parameterType,
                        ReturnType=returnType
                    });
                }
            }
            return methods.Distinct().ToList();
        }
    }
}
