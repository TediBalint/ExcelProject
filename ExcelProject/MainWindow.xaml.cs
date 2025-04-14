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
		public ObservableCollection<ObservableCollection<CellPropertiesModel>> cellPropertiesModels { get; set; } = new ObservableCollection<ObservableCollection<CellPropertiesModel>>();
        public ObservableCollection<FontFamily> fontFamilies { get; set; } = new ObservableCollection<FontFamily>();
        public ObservableCollection<KeyValuePair<string, TextDecorationCollection?>> textDecorations { get; set; } = Statics.textDecorations;
        public ObservableCollection<double> fontSizes { get; set; } = new ObservableCollection<double>();
        public CellPropertiesModel? selectedCellProperties { get; set; }
        public CellPropertiesModel? SelectedCellProperties
        {   
            get {return selectedCellProperties;}
            set { selectedCellProperties = value; OnPropertyChanged(nameof(SelectedCellProperties)); }
        }
        public ObservableCollection<KeyValuePair<string, Brush>> brushes { get; set; } = Statics.foregroundBrushes;
		public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            readFontFamilies();
            readFontSizes();
            makeList(20);
            makeTBXs();
        }
        
        private void readFontFamilies()
        {
            using (StreamReader sr = new StreamReader(Statics.FONT_FILE_PATH))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if(line == null) continue;
                    line = line.Remove(0, 3);
                    fontFamilies.Add(new FontFamily(line));
                }
            }
        }
        private void readFontSizes()
        {
            using (StreamReader sr = new StreamReader(Statics.DEFAULT_FONT_SIZE_FILE_PATH))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if(line == null) continue;
                    fontSizes.Add(double.Parse(line));
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
                    l.Add(new CellPropertiesModel(j, i, true));
                }
                cellPropertiesModels.Add(l);
            }
        }
        private void makeTBXs()
        {
            CellPropertiesModel model = new CellPropertiesModel(0, 0, true);
            for (int i = 1; i < cellPropertiesModels.Count; i++) 
            {
                ColumnDefinition column = new ColumnDefinition() {Width = model.Width};
                RowDefinition row = new RowDefinition() {Height = model.Height};
				table_GRD.ColumnDefinitions.Add(column);
                table_GRD.RowDefinitions.Add(row);
                for (int j = 1; j < cellPropertiesModels[0].Count; j++) 
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
			tbx.GotFocus += Tbx_GotFocus;
            tbx.Cursor = Cursors.Cross;
			table_GRD.Children.Add(tbx);
		}
        private void makeBTNs()
        {
            for (int i = 0; i < cellPropertiesModels.Count; i++)
            {

            }
        }
		private void Tbx_GotFocus(object sender, RoutedEventArgs e)
		{
			if (sender.GetType() != typeof(TextBox)) return;
			TextBox tbx = (TextBox)sender;
            int i = int.Parse(tbx.Tag.ToString().Split(';')[0]);
			int j = int.Parse(tbx.Tag.ToString().Split(';')[1]);
            if (SelectedCellProperties != null) SelectedCellProperties.Border_Thickness = new Thickness(1);
            SelectedCellProperties = cellPropertiesModels[i][j];
            SelectedCellProperties.Border_Thickness = new Thickness(2.5);
        }
		private void bindPropoerty(TextBox tbx, KeyValuePair<DependencyProperty, string> prop, int i, int j)
        {
			Binding binding = new Binding(prop.Value)
			{
				Source = cellPropertiesModels[i][j],
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
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
            if(SelectedCellProperties == null) return;
            if (SelectedCellProperties.Font_Weight == FontWeights.Bold) SelectedCellProperties.Font_Weight = FontWeights.Normal;
            else SelectedCellProperties.Font_Weight = FontWeights.Bold;
		}

		private void insertFuncBtn_Click(object sender, RoutedEventArgs e)
		{
            Window functionselectorWindw = new FunctionSelector();
            functionselectorWindw.ShowDialog();
		}

		private void size_plus_Click(object sender, RoutedEventArgs e)
		{
            if (SelectedCellProperties == null) return;
            int idx = fontSizes.IndexOf(SelectedCellProperties.Font_Size) + 1;
            if(idx < fontSizes.Count) SelectedCellProperties.Font_Size = fontSizes[idx];
		}

		private void size_minus_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedCellProperties == null) return;
			int idx = fontSizes.IndexOf(SelectedCellProperties.Font_Size) - 1;
			if (idx >= 0) SelectedCellProperties.Font_Size = fontSizes[idx];
		}

		private void fontstyle_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedCellProperties == null) return;
            if (SelectedCellProperties.Font_Style == FontStyles.Normal) SelectedCellProperties.Font_Style = FontStyles.Italic;
            else SelectedCellProperties.Font_Style = FontStyles.Normal;
		}

		private void StyleBTN_Click(object sender, RoutedEventArgs e)
		{
            if (sender.GetType() != typeof(Button) || SelectedCellProperties == null) return;
            Button btn = (Button)sender;
            SelectedCellProperties.Background_Color = btn.Background;
            SelectedCellProperties.Foreground_Color = btn.Foreground;
		}

		private void VerticalContentAlignmentBTN_Click(object sender, RoutedEventArgs e)
		{
            if (sender.GetType() != typeof(Button) || SelectedCellProperties == null) return;
			Button btn = (Button)sender;
			if (Enum.TryParse(btn.Tag.ToString(), out VerticalAlignment alignment)) SelectedCellProperties.Vertical_Content_Align = alignment;
		}
		private void HorizontalContentAlign_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (sender.GetType() != typeof(Button) || SelectedCellProperties == null) return;
			Button btn = (Button)sender;
			if (Enum.TryParse(btn.Tag.ToString(), out HorizontalAlignment alignment)) SelectedCellProperties.Horizontal_Content_Align = alignment;
		}
        private void cellEdit_Click(object sender, RoutedEventArgs e)
        {
            Window editWindow = new CellEditWindow();
            editWindow.ShowDialog();
        }
    }
}