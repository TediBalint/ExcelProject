using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Dictionary<string, string> BindedParamVals { get; set; } = new();
        private List<string> paramValsInOrder = new();
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
        }
        private void CreateParamInputFields() {
            for (int i = 0; i < SelectedFunction.ParameterNames.Length; i++) {
                RowDefinition rd = new RowDefinition();
                paramInputs.RowDefinitions.Add(rd);
                createInputRow(SelectedFunction.ParameterNames[i], i);
            }
        }
        private void createInputRow(string parameterName, int i, bool bold = true)
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
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
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
            if (parameterName[0] == '*') {
                b = new Binding($"BindedParamVals[{parameterName.Replace("*", "")}{AsterixParamCount}]");
                AsterixParamCount++;
            }
            else b = new Binding($"BindedParamVals[{parameterName}]");
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
        private void evaluateBindedParams() {
            foreach (KeyValuePair<string, string> p in BindedParamVals) {
                BindedParamVals[p.Key] = Function.evaluateParameter(p.Value);
            }
        }
        private void done_Click(object sender, RoutedEventArgs e) {
            evaluateBindedParams();
            SelectedFunction.Parameters = BindedParamVals;
            foreach(var tb in paramInputs.Children.OfType<TextBox>()) {
                if (tb.Text != "") paramValsInOrder.Add(tb.Text);
            }
            SelectedFunction.raw = '=' + SelectedFunction.Name + '(' + string.Join(';', paramValsInOrder) + ')';
            DialogResult = true;
            Close();
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Tab) {
                try {
                    fnValuePreview.Foreground = Brushes.Black;
                    evaluateBindedParams();
                    SelectedFunction.Parameters = BindedParamVals;
                    FnPreview = SelectedFunction.Invoke();
                }
                catch {
                    fnValuePreview.Foreground = Brushes.Red;
                    FnPreview = "Hiba";
                }
                string? tag = ((TextBox)sender).Tag.ToString();
                if (tag[0] == '*' &&
                    BindedParamVals.ContainsKey(tag.Replace("*", "") + (AsterixParamCount - 1))
                ) {
                    paramInputs.RowDefinitions.Add(new RowDefinition());
                    foreach(UIElement obj in paramInputs.Children)
                    {
                        if (((Control)obj).Tag.ToString()[0] != '*') Grid.SetRow(obj, Grid.GetRow(obj) + 1);
                    }
                    createInputRow(tag, AsterixParamCount - 1, false);
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
    }
}
