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
                createInputRow(SelectedFunction.ParameterNames[i], i);
                // harmadik oszlop:preview
            }
        }
        private void createInputRow(string parameterName, int i)
        {
            Label paramName = new Label()
            {
                Content =
                        parameterName[0] == '*' ?
                        parameterName.Replace("*", "") + (i + 1).ToString() :
                        parameterName,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Right,
                FontWeight = FontWeights.Bold,
                Tag = parameterName
            };
            SetRowAndAdd(paramName, i);
            TextBox paramValue = new TextBox()
            {
                Height = 20,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Tag = parameterName
            };
            paramValue.KeyDown += textBox_KeyDown;
            Binding b;
            if (parameterName[0] == '*')
            {
                b = new Binding($"SelectedFunction.Parameters[{parameterName.Replace("*", "")}{AsterixParamCount}]");
                AsterixParamCount++;
            }
            else
            {
                b = new Binding($"SelectedFunction.Parameters[{parameterName}]");
            }
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            paramValue.SetBinding(TextBox.TextProperty, b);
            Grid.SetColumn(paramValue, 1);
            SetRowAndAdd(paramValue, i);
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
                string? tag = ((TextBox)sender).Tag.ToString();
                if (tag[0] == '*' &&
                    SelectedFunction.Parameters.ContainsKey(tag.Replace("*", "") + (AsterixParamCount - 1))
                ) {
                    paramInputs.RowDefinitions.Add(new RowDefinition());
                    bool pushRow = false;
                    foreach(UIElement obj in paramInputs.Children)
                    {
                        if (((Control)obj).Tag.ToString()[0] != '*') Grid.SetRow(obj, Grid.GetRow(obj) + 1);
                    }
                    createInputRow(tag, AsterixParamCount - 1);
                }
            }
            //Close(); --- teszt
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
    }
}
