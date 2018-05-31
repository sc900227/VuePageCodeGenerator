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
        public bool PageCreate(CreateOption createOption)
        {
            try
            {
                createOption = createOption as VueCreateOption;
                if (File.Exists(createOption.SavePath))
                {
                    File.Delete(createOption.SavePath);
                }
                using (StreamWriter sw = new StreamWriter(createOption.SavePath, false))
                {
                    sw.Write(createOption.TemplateData);
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
            
        }
    }

}