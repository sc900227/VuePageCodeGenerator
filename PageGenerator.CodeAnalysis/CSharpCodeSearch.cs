using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.CodeAnalysis
{
    public class CSharpCodeSearch : ICodeSearch
    {
        public string GetContent(string strFilePath)
        {
            string content = string.Empty;
            using (FileStream stream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(stream, Encoding.Default);
                content = reader.ReadToEnd();
            }
            return content;
        }

        public string SearchFileInSolution(string solutionPath, string fileName)
        {
            try
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
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
