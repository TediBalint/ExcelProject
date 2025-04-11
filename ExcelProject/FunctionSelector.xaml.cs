using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ExcelProject
{
    /// <summary>
    /// Interaction logic for FunctionSelector.xaml
    /// </summary>
    public partial class FunctionSelector : Window
    {
        public FunctionSelector()
        {
            InitializeComponent();
            Function f = new("SZUMHA", "1;2;3;4");
            string s = f.Invoke();
        }
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
