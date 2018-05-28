using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.PageCreate.VuePage
{
    public class VuePageCreateBase : IPageGenerate
    {
        public void PageCreate(CreateOption createOption)
        {
            createOption = createOption as VueCreateOption;
            //if (!Directory.Exists(createOption.SavePath)) {
            //    Directory.CreateDirectory(createOption.SavePath);
            //}
            if (File.Exists(createOption.SavePath))
            {
                File.Delete(createOption.SavePath);
            }
            using (StreamWriter sw = new StreamWriter(createOption.SavePath,false))
            {
                sw.Write(createOption.TemplateData);
            }
                
            
        }
    }

}