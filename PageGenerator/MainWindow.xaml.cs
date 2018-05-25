
using PageGenerator.CodeAnalysis;
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
        public MainWindow(List<CSharpMethod> cSharpMethods,string solutionPath)
        {
            _cSharpMethods = cSharpMethods;
            _solutionPath = solutionPath;
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
            var selectMethod = _cSharpMethods.Where(a => a.MethodName == cbxSelect.Text).FirstOrDefault();
            if (selectMethod==null)
            {
                return;
            }
            var selectReturnType = GetPropertyType(selectMethod.ReturnType);
            var csPath = SearchFileInSolution(selectReturnType + ".cs");
            if (!string.IsNullOrEmpty(csPath))
            {
                CSharpCodeAnalysis codeAnalysis = new CSharpCodeAnalysis(csPath);
                List<CSharpProperty> propertys=codeAnalysis.GetAllCSharpPropertys();
                foreach (var item in propertys)
                {
                    this.dgSelect.Columns.Add(new DataGridTextColumn() { Header = item.PropertyName});
                }
                
            }
            
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
        public string SearchFileInSolution(string fileName)
        {
            string[] filePath = Directory.GetFiles(_solutionPath, fileName, SearchOption.AllDirectories);
            if (filePath != null && filePath.Length > 0)
            {
                return filePath.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            BindCrudDg();
            g1.Visibility = Visibility.Collapsed;
            g2.Visibility = Visibility.Visible;
        }
    }
}
