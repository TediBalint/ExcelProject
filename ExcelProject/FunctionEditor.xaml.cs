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

namespace ExcelProject {
    /// <summary>
    /// Interaction logic for FunctionEditor.xaml
    /// </summary>
    public partial class FunctionEditor : Window, INotifyPropertyChanged {
        public Function SelectedFunction { get; set; }
        private int AsterixParamCount = 1;
        private string _fnPreview;
        public string FnPreview { 
            get { return _fnPreview; }
            set { _fnPreview = value; OnPropertyChanged(nameof(FnPreview)); } 
        }
        public FunctionEditor(Function _selectedFunc) {
            InitializeComponent();
            SelectedFunction = _selectedFunc;
            DataContext = this;
            CreateParamInputFields();
            //close eventet handleelni kulon? 
        }
        private void CreateParamInputFields() {
            for (int i = 0; i < SelectedFunction.ParameterNames.Length; i++) {
                RowDefinition rd = new RowDefinition();
                paramInputs.RowDefinitions.Add(rd);
                Label paramName = new Label() { 
                    Content = SelectedFunction.ParameterNames[i].Replace("*", ""),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height = 25,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontWeight = FontWeights.Bold,
                };
                SetRowAndAdd(paramName, i);
                TextBox paramValue = new TextBox() {
                    Height = 20,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = SelectedFunction.ParameterNames[i],
                };
                paramValue.KeyDown += textBox_KeyDown;
                Binding b;
                if (SelectedFunction.ParameterNames[i][0] == '*') {
                    b = new Binding($"SelectedFunction.Parameters[{SelectedFunction.ParameterNames[i].Replace("*", "")}{AsterixParamCount}]") ;
                    AsterixParamCount++;
                }
                else {
                    b = new Binding($"SelectedFunction.Parameters[{SelectedFunction.ParameterNames[i]}]");
                }
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                paramValue.SetBinding(TextBox.TextProperty, b);
                Grid.SetColumn(paramValue, 1);
                SetRowAndAdd(paramValue, i);
                // harmadik oszlop:preview
            }
        }
        private void SetRowAndAdd(Control control, int row) {
            Grid.SetRow(control, row);
            paramInputs.Children.Add(control);
        }
        private void cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
        private void done_Click(object sender, RoutedEventArgs e) {
            //enteres esemény ide is?
            //DialogResult = true;
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Tab) {
                try {
                    fnValuePreview.Foreground = Brushes.Black;
                    FnPreview = SelectedFunction.Invoke();
                }
                catch {
                    fnValuePreview.Foreground = Brushes.Red;
                    FnPreview = "Hiba";
                }
                //finally: myGrid.RowDefinitions.Insert(2, newRow);
            }
            //Close(); --- teszt
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
    }
}
