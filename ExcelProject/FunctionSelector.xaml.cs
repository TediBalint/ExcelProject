using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class FunctionSelector : Window, INotifyPropertyChanged {
        public Function[] AllFunctions { get; private set; } = Function.getAllFunctions();
        private Function[] funcsToShow;
        public Function[] FuncsToShow {
            get { return funcsToShow; }
            set { funcsToShow = value; OnPropertyChanged(nameof(FuncsToShow)); } 
        }

        private Function _selectedFunction;
        public Function SelectedFunction {
            get { return _selectedFunction; }
            set { _selectedFunction = value; OnPropertyChanged(nameof(SelectedFunction)); }
        }
        public FunctionSelector()
        {
            InitializeComponent();
            SelectedFunction = AllFunctions[0];
            FuncsToShow = AllFunctions;
            DataContext = this;
        }
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void searchBtn_Click(object sender, RoutedEventArgs e) {
            FuncsToShow = AllFunctions.Where(x => x.Name.StartsWith(fnNameSearchVal.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
    }
}
