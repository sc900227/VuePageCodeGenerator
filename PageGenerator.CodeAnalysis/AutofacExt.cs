using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Configuration;
using Autofac.Core;

namespace PageGenerator.CodeAnalysis
{
    public static class AutofacExt
    {
        static ContainerBuilder _builder;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void InitAutofac()
        {
            _builder = new ContainerBuilder();

            //
            _builder.RegisterType<CSharpCodeSearch>().As<ICodeSearch>();


            //读取配置

            //_builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
        }
        static IContainer _container;
        static IContainer Container
        {
            get
            {
                if (_container == null)
                    _container = _builder.Build();
                return _container;
            }
        }
        /// <summary>
        /// 从容器中获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T GetFromFac<T>()
        {
            T t = Container.Resolve<T>();
            return t;
        }
    }
}
