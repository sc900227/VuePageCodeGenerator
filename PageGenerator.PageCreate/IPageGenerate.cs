using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.PageCreate
{
    public interface IPageGenerate
    {
        bool PageCreate(CreateOption createOption);
    }
}
