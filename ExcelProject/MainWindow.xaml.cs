using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelProject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;

        public Brush BoldButtonBackground
        {
            get
            {
                if(SelectedCellProperties != null) return Statics.WeightToBrush[SelectedCellProperties.Font_Weight];
                else {  return Brushes.LightGray;  }
			}
        }
		public ObservableCollection<ObservableCollection<CellPropertiesModel>> cellPropertiesModels { get; set; } = new ObservableCollection<ObservableCollection<CellPropertiesModel>>();
        public ObservableCollection<FontFamily> fontFamilies { get; set; } = new ObservableCollection<FontFamily>();
        public ObservableCollection<TextDecorationCollection> textDecorations { get; set; } = new ObservableCollection<TextDecorationCollection>() { TextDecorations.Underline, TextDecorations.Baseline, TextDecorations.OverLine};
        public CellPropertiesModel? selectedCellProperties { get; set; }
        public CellPropertiesModel? SelectedCellProperties
        {   
            get {return selectedCellProperties;}
            set { selectedCellProperties = value; OnPropertyChanged(nameof(SelectedCellProperties)); OnPropertyChanged(nameof(BoldButtonBackground)); }
        }
		public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            setFontFamilies();
            makeList(20);
            makeTBXs();
        }
        private void setFontFamilies()
        {
            using (StreamReader sr = new StreamReader("fonts.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    line = line.Remove(0, 3);
                    fontFamilies.Add(new FontFamily(line));
                }
            }
        }


		private void next_Click(object sender, RoutedEventArgs e)
        {
            FunctionSelector fs = new FunctionSelector();
            fs.ShowDialog();
        }
        private void makeList(int side_length)
        {
            for (int i = 0; i < side_length; i++) 
            {
                ObservableCollection<CellPropertiesModel> l = new ObservableCollection<CellPropertiesModel>();
                for (int j = 0; j < side_length; j++)
                {
                    l.Add(new CellPropertiesModel(true));
                }
                cellPropertiesModels.Add(l);
            }

        }
        private void makeTBXs()
        {
            CellPropertiesModel model = new CellPropertiesModel(true);
            for (int i = 0; i < cellPropertiesModels.Count; i++) 
            {
                ColumnDefinition column = new ColumnDefinition() {Width = model.Width};
                RowDefinition row = new RowDefinition() {Height = model.Height};
				table_GRD.ColumnDefinitions.Add(column);
                table_GRD.RowDefinitions.Add(row);
                for (int j = 0; j < cellPropertiesModels[0].Count; j++) 
                {
                    makeTbx(i, j);
                }
            }
        }
        private void makeTbx(int i, int j)
        {
			TextBox tbx = new TextBox();
            Grid.SetColumn(tbx, j);
			Grid.SetRow(tbx, i);
            foreach (KeyValuePair<DependencyProperty, string> p in Statics.CellPropNames)
            {
				bindPropoerty(tbx, p, i, j);
			}
            tbx.Tag = $"{i};{j}";
			tbx.GotFocus += Tbx_GotFocus; ;
			table_GRD.Children.Add(tbx);
		}

		private void Tbx_GotFocus(object sender, RoutedEventArgs e)
		{
			if (sender.GetType() != typeof(TextBox)) return;
			TextBox tbx = (TextBox)sender;
			int i = int.Parse(tbx.Tag.ToString().Split(';')[0]);
			int j = int.Parse(tbx.Tag.ToString().Split(';')[1]);
			SelectedCellProperties = cellPropertiesModels[i][j];
		}

		private void Tbx_MouseDown(object sender, MouseButtonEventArgs e)
		{
            if (sender.GetType() != typeof(TextBox)) return;
            TextBox tbx = (TextBox)sender;
            int i = int.Parse(tbx.Tag.ToString().Split(';')[0]);
            int j = int.Parse(tbx.Tag.ToString().Split(';')[1]);
            SelectedCellProperties = cellPropertiesModels[i][j];
		}

		private void bindPropoerty(TextBox tbx, KeyValuePair<DependencyProperty, string> prop, int i, int j)
        {
			Binding binding = new Binding(prop.Value)
			{
				Source = cellPropertiesModels[i][j],
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.Default
			};
			tbx.SetBinding(prop.Key, binding);
		}

		private void gb_Click(object sender, RoutedEventArgs e)
		{
            foreach (ObservableCollection<CellPropertiesModel> item in cellPropertiesModels)
            {
                foreach (CellPropertiesModel item1 in item)
                {
                    if (item1.Text != "Default") Debug.WriteLine(item1.Text);
                }
            }
        }
		public void OnPropertyChanged(string name)
        {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

		private void fontweight_Click(object sender, RoutedEventArgs e)
		{
            if (sender.GetType() != typeof(Button)) return;
            if(SelectedCellProperties == null) return;
            if (SelectedCellProperties.Font_Weight == FontWeights.Bold) SelectedCellProperties.Font_Weight = FontWeights.Normal;
            else SelectedCellProperties.Font_Weight = FontWeights.Bold;
            OnPropertyChanged(nameof(BoldButtonBackground));
		}

		private void insertFuncBtn_Click(object sender, RoutedEventArgs e)
		{
            Window functionselectorWindw = new FunctionSelector();
            functionselectorWindw.Show();
		}
	}
}