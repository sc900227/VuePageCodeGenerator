
using PageGenerator.CodeAnalysis;
using PageGenerator.PageCreate.VuePage;
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
            tbUrl.Text = "api/services/app/"+_itemProjectPath.Substring(_itemProjectPath.LastIndexOf("\\") + 1).Replace("AppService.cs", "/");
        }
        private List<CSharpProperty> BindDg(DataGrid dataGrid,TextBlock textBlock, string methodName, MethodType csType) {
            var selectMethod = _cSharpMethods.Where(a => a.MethodName == methodName).FirstOrDefault();
            if (selectMethod == null)
            {
                throw new Exception("No Finded Method");
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
            var csPath = CSharpCodeAnalysis.SearchFileInSolution(_itemProjectPath.Substring(0, _itemProjectPath.LastIndexOf("\\")), className + ".cs");
            
            if (!string.IsNullOrEmpty(csPath))
            {
                CSharpCodeAnalysis codeAnalysis = new CSharpCodeAnalysis(csPath);
                propertys = codeAnalysis.GetAllCSharpPropertys();
                foreach (var item in propertys)
                {
                    dataGrid.Columns.Add(new DataGridTextColumn() { Header = item.PropertyName });
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
            VuePageGenerate pageGenerate = new VuePageGenerate(option);
            pageGenerate.VueTablePageCreate(formPropertys, columnPropertys,ApiFun,tbUrl.Text);
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            g1.Visibility = Visibility.Visible;
            g2.Visibility = Visibility.Collapsed;
        }
    }
}
