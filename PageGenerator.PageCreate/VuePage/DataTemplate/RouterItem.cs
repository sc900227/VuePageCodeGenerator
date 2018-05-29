using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageGenerator.PageCreate.VuePage.DataTemplate
{
    /*
     {
                path: 'articleinfo-page',
                title: '人员管理',
                name: 'articleinfo-page',
                icon: 'document-text',
                component: () => import('@/views/person-page/PersonInfoPage.vue')
            }
     */
    public class RouterItem
    {
        public string path { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string component { get; set; }
    }
}
