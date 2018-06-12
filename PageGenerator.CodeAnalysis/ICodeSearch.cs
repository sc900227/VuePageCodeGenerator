using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.CodeAnalysis
{
    public interface ICodeSearch
    {
        string SearchFileInSolution(string solutionPath, string fileName);

        string GetContent(string strFilePath);
    }
}
