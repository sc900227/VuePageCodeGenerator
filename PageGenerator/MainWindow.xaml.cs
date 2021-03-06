﻿
using PageGenerator.CodeAnalysis;
using PageGenerator.PageCreate;
using PageGenerator.PageCreate.VuePage;
using PageGenerator.PageCreate.VuePage.DataTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PageGenerator
{
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private List<CSharpMethod> _cSharpMethods;
        private string _solutionPath;
        private string _itemProjectPath;
        private List<CSharpProperty> formPropertys;
        private List<CSharpProperty> columnPropertys;
        private List<CSharpProperty> updatePropertys;
        private List<CSharpProperty> deletePropertys;
        private Dictionary<string, string> ApiFun=new Dictionary<string, string>();
        private ICodeSearch codeSearch;
        public enum MethodType
        {
            Return,
            Parameter
        }

        public MainWindow(List<CSharpMethod> cSharpMethods,string solutionPath,string itemProjectPath)
        {
            _cSharpMethods = cSharpMethods;
            _solutionPath = solutionPath;
            _itemProjectPath = itemProjectPath;
            InitializeComponent();
            BindCrudCbx();
            tbUrl.Text = "api/services/app/" + _itemProjectPath.Substring(_itemProjectPath.LastIndexOf("\\") + 1).Replace("AppServices.cs", "/");
            tbPageName.Text = _itemProjectPath.Substring(_itemProjectPath.LastIndexOf("\\") + 1).Replace("AppServices.cs", "InfoPage");
            txtTemplatePath.Text = System.IO.Path.Combine(_solutionPath,PageContsts.VueTemplateDefaultPath);
            txtPagePath.Text= System.IO.Path.Combine(_solutionPath, PageContsts.PageDefaultSavePath);
            txtRouterPath.Text= System.IO.Path.Combine(_solutionPath, PageContsts.RouterDefaultSavePath);
            AutofacExt.InitAutofac();
            codeSearch = AutofacExt.GetFromFac<ICodeSearch>();
        }

        private void BindCrudCbx() {
            List<string> items = _cSharpMethods.Select(a =>a.MethodName).ToList();
            this.cbxSelect.ItemsSource = items;
            this.cbxSelect.SelectedIndex = 0;
            this.cbxInsert.ItemsSource = items;
            this.cbxInsert.SelectedIndex = 0;
            this.cbxUpdate.ItemsSource = items;
            this.cbxUpdate.SelectedIndex = 0;
            this.cbxDelete.ItemsSource = items;
            this.cbxDelete.SelectedIndex = 0;
            
        }
        private void BindCrudDg() {
            ApiFun.Clear();
            dgSelect.Columns.Clear();
            dgInsert.Columns.Clear();
            dgUpdate.Columns.Clear();
            dgDelete.Columns.Clear();
            ApiFun.Add("Select", cbxSelect.Text);
            ApiFun.Add("Insert", cbxInsert.Text);
            ApiFun.Add("Update", cbxUpdate.Text);
            ApiFun.Add("Delete", cbxDelete.Text);
            columnPropertys = BindDg(dgSelect,tbSelect, cbxSelect.Text,MethodType.Return);
            formPropertys=BindDg(dgInsert,tbInsert, cbxInsert.Text, MethodType.Parameter);
            updatePropertys=BindDg(dgUpdate,tbUpdate, cbxUpdate.Text, MethodType.Parameter);
            deletePropertys=BindDg(dgDelete,tbDelete, cbxDelete.Text, MethodType.Parameter);
           
        }
        private List<CSharpProperty> BindDg(DataGrid dataGrid,TextBlock textBlock, string methodName, MethodType csType) {
            if (string.IsNullOrEmpty(methodName)) {
                MessageBox.Show("Method is Null!");
                throw new Exception("Method is Null!");
            }
            var selectMethod = _cSharpMethods.Where(a => a.MethodName == methodName).FirstOrDefault();
            if (selectMethod == null)
            {
                MessageBox.Show("No Finded Method!");
                throw new Exception("No Finded Method!");
            }
            
            string className = string.Empty;
            if (csType == MethodType.Return)
            {
                className = GetPropertyType(selectMethod.ReturnType);
                textBlock.Text = $"{className}查询结果:";
            }
            else if (csType== MethodType.Parameter)
            {
                className = GetPropertyType(selectMethod.ParameterType);
                textBlock.Text = $"{className}参数:";
            }
            List<CSharpProperty> propertys = null;
            var itemPath = _itemProjectPath.Substring(0, _itemProjectPath.LastIndexOf("\\"));
            var csPath = codeSearch.SearchFileInSolution(itemPath, className + ".cs");
            //propertyName = string.Empty;
            if (!string.IsNullOrEmpty(csPath))
            {
                CSharpCodeAnalysis codeAnalysis = new CSharpCodeAnalysis(csPath);
                propertys = codeAnalysis.GetAllCSharpPropertys();
                List<CSharpProperty> abpPropertys = new List<CSharpProperty>();
                if (className.Contains("CreateOrUpdate"))
                {
                    //查找EditDto类
                    var abpPath = codeSearch.SearchFileInSolution(itemPath, propertys.FirstOrDefault().PropertyType + ".cs");

                    if (string.IsNullOrEmpty(abpPath)) {
                        MessageBox.Show("No Finded EditDto!");
                        throw new Exception("No Finded EditDto");
                    }
                    CSharpCodeAnalysis abpCodeAnalysis = new CSharpCodeAnalysis(abpPath);
                    abpPropertys = abpCodeAnalysis.GetAllCSharpPropertys();
                    foreach (var item in abpPropertys)
                    {
                        dataGrid.Columns.Add(new DataGridTextColumn() { Header = item.PropertyName });
                    }
                    return abpPropertys;

                }
                else
                {
                    //查找IEnumerable集合类
                    //var listPropertyType = propertys.FirstOrDefault(a => a.PropertyType.Contains("IEnumerable"));
                    //if (listPropertyType != null) {
                    //    var listPath = CSharpCodeAnalysis.SearchFileInSolution(_solutionPath, GetPropertyType(listPropertyType.PropertyType) + ".cs");
                    //    CSharpCodeAnalysis listCodeAnalysis = new CSharpCodeAnalysis(listPath);
                    //    var listPropertys = listCodeAnalysis.GetAllCSharpPropertys();
                    //    //追加到属性
                    //    propertys.AddRange(listPropertys);
                    //}
                    foreach (var item in propertys)
                    {
                        dataGrid.Columns.Add(new DataGridTextColumn() { Header = item.PropertyName });
                    }
                    return propertys;
                }
            }
            else
            {
                dataGrid.Columns.Add(new DataGridTextColumn() { Header = className});
            }
            return propertys;
        }
        public string GetPropertyType(string property)
        {
            if (!string.IsNullOrEmpty(property))
            {
                if (property.Contains("<"))
                {
                    var propertyTemp = String.Join("", property.Substring(property.IndexOf("<") + 1).Reverse());
                    property = String.Join("", propertyTemp.Substring(propertyTemp.IndexOf(">") + 1).Reverse());
                }
            }
            return property;
        }
        
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            BindCrudDg();
            tbTitle.Text = txtTitle.Text;
            g1.Visibility = Visibility.Collapsed;
            g2.Visibility = Visibility.Visible;
           
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            VueCreateOption option = new VueCreateOption(_solutionPath,tbTitle.Text);
            VuePageGenerate pageGenerate1 = new VuePageGenerate(option,txtTemplatePath.Text,txtPagePath.Text,txtRouterPath.Text);
            VuePageGenerate pageGenerate2 = new VuePageGenerate(option, txtTemplatePath.Text, txtPagePath.Text, txtRouterPath.Text);
            try
            {
                if (cbxRouter.IsChecked == true)
                {
                    RouterItem routerItem = new RouterItem()
                    {
                        title = tbTitle.Text,
                        path = tbPageName.Text,
                        name = tbPageName.Text,
                        icon = "document-text",
                        component = string.Format("() => import('@/views/{0}/{1}.vue')", tbPageName.Text, tbPageName.Text)
                    };

                    Dispatcher.BeginInvoke((Action)delegate () {
                        pageGenerate1.CreateRouter(routerItem);
                    });

                }
                var message=Dispatcher.Invoke((Func<string>)delegate ()
                {
                    return pageGenerate2.VueTablePageCreate(formPropertys, columnPropertys, ApiFun, tbUrl.Text, tbPageName.Text);
                    
                });
                MessageBox.Show(message);
                this.Close();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.ToString());
            }
            
             
           
           
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            g1.Visibility = Visibility.Visible;
            g2.Visibility = Visibility.Collapsed;
        }
    }
}
