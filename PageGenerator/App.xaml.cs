using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PageGenerator
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App() {
            
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            try
            {
                Exception ex = e.ExceptionObject as Exception;
                string err = "非窗体线程异常:\n\n";
                MessageBox.Show(err + ex.Message);// + Environment.NewLine + ex.StackTrace);
            }
            catch
            {
                MessageBox.Show("不可恢复的窗体线程异常,应用程序将退出.");
            }
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.Exception;
                string err = "窗体线程异常:\n\n";
                MessageBox.Show(err + ex.Message);// + Environment.NewLine + ex.StackTrace);
                //PVAnalyzer.Config.FileDirectory.wriStrToTxtFile(ex.StackTrace, @"c:\temp\error.txt");
                e.Handled = true;
            }
            catch
            {
                MessageBox.Show("不可恢复的窗体线程异常,应用程序将退出.");
            }
        }
    }
}
