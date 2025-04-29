using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ExcelProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged(nameof(FileName)); }
        }
        public string SaveFormat { get; set; } = Statics.saveFormats[0];
        public ObservableCollection<ObservableCollection<CellPropertiesModel>> cellPropertiesModels {
            get { return Statics.CellPropertiesModels; }
            set { Statics.CellPropertiesModels = value;
                OnPropertyChanged(nameof(cellPropertiesModels));
            }
        }
        private bool isCopying = false;
        public bool IsStriped 
        {
            get
            {
                return isStriped;
            }
            set
            {
                isStriped = value;OnPropertyChanged(nameof(IsStriped));
            }
        }
        private bool isStriped { get; set; } = false;
        public ObservableCollection<FontFamily> fontFamilies { get; set; } = new ObservableCollection<FontFamily>();
        public ObservableCollection<KeyValuePair<string, TextDecorationCollection?>> textDecorations { get; set; } = Statics.textDecorations;
        public ObservableCollection<string> saveFormats { get; set; } = Statics.saveFormats;
		public ObservableCollection<double> fontSizes { get; set; } = new ObservableCollection<double>();
        public CellPropertiesModel? selectedCellProperties { get; set; }
        public bool FnButtonEnabled { get; set; } = false;
        private TableLoader loader;
        public CellPropertiesModel? SelectedCellProperties 
        {   
            get {return selectedCellProperties;}
            set { selectedCellProperties = value; 
                  OnPropertyChanged(nameof(SelectedCellProperties));
                  FnButtonEnabled = SelectedCellProperties != null;
                  OnPropertyChanged(nameof(FnButtonEnabled));
            }
        }
		public ObservableCollection<KeyValuePair<string, Brush>> brushes { get; set; } = Statics.foregroundBrushes;
		public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            init();
            loader = new TableLoader();
            CellContentEditor.KeyUp += CellEditorKeypress;
        }
        private void init()
        {
			readFontFamilies();
			readFontSizes();
			makeList(20);
            makeBTNs();
			makeGrid();
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
            for (int i = 1; i < cellPropertiesModels.Count; i++) 
            {
                for (int j = 1; j < cellPropertiesModels[0].Count; j++) 
                {
                    makeTbx(i, j);
                }
            }
        }
        private void makeGrid()
        {
            for(int i = 0; i < cellPropertiesModels[0].Count; i++)
            {
				ColumnDefinition column = new ColumnDefinition();
				RowDefinition row = new RowDefinition();
                row.Height = Statics.DefaultHeight;
                column.Width = Statics.DefaultWidth;
				table_GRD.ColumnDefinitions.Add(column);
				table_GRD.RowDefinitions.Add(row);
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
			tbx.KeyUp += Tbx_Keypress;
            //tbx.LostFocus += Tbx_LostFocus;
            tbx.Text = string.Empty;
            tbx.Cursor = Cursors.Cross;
			table_GRD.Children.Add(tbx);
            tbx.TextWrapping = TextWrapping.Wrap;
        }
        private void makeBTNs()
        {
            for (int i = 1; i < cellPropertiesModels.Count; i++)
            {
                makeBTN(0, i, ((char)('A' - 1 + i)).ToString());
			}
			for (int i = 1; i < cellPropertiesModels.Count; i++)
			{
                makeBTN(i, 0, i.ToString());
			}
            makeBTN(0, 0, "");
		}
        private void makeBTN(int i, int j, string content)
        {
			Button btn = new Button();
			Grid.SetColumn(btn, j);
			Grid.SetRow(btn, i);
            btn.Content = content;
			btn.Tag = $"{i};{j}";
            btn.HorizontalContentAlignment = HorizontalAlignment.Center;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
			table_GRD.Children.Add(btn);
		}
        //private void Tbx_LostFocus(object sender, RoutedEventArgs e)
        //      {
        //          if (sender.GetType() != typeof(TextBox)) return;
        //          TextBox tbx = (TextBox)sender;
        //          tbx.BorderThickness = new Thickness(1);
        //      }
        private void Tbx_Keypress(object sender, KeyEventArgs e) {
            TextBox tbx = (TextBox)sender;
            if (e.Key == Key.Enter || e.Key == Key.Tab) {
                if (tbx.Text != "" && tbx.Text[0] == '=') {
                    try {
                        tbx.Text = Function.Compile(tbx.Text).Invoke();
                    }
                    catch { }
                }
            }
            else {
                SelectedCellProperties.Raw = tbx.Text; // needs test
            }
        }
        private void Tbx_GotFocus(object sender, RoutedEventArgs e)
		{
			if (sender.GetType() != typeof(TextBox)) return;
			TextBox tbx = (TextBox)sender;
            tbx.BorderThickness = new Thickness(2.5);
            int i = int.Parse(tbx.Tag.ToString().Split(';')[0]);
            int j = int.Parse(tbx.Tag.ToString().Split(';')[1]);
            if(SelectedCellProperties != null)
            { 
                if(isCopying) cellPropertiesModels[i][j].CopyProps(SelectedCellProperties);
                if (isCopying)
                {
                    table_GRD.ColumnDefinitions[j].Width = table_GRD.ColumnDefinitions[SelectedCellProperties.X].Width;
                    table_GRD.RowDefinitions[i].Height = table_GRD.RowDefinitions[SelectedCellProperties.Y].Height;
                }
                isCopying = false;
                SelectedCellProperties.Border_Thickness = new Thickness(1);
                SelectedCellProperties.Border_Color = Brushes.Black;
            }
			SelectedCellProperties = cellPropertiesModels[i][j];
            SelectedCellProperties.Border_Color = Brushes.CornflowerBlue;
            if (SelectedCellProperties.Raw == "") SelectedCellProperties.Raw = SelectedCellProperties.Text;
        }
        private void CellEditorKeypress(object sender, KeyEventArgs e) {
            try {
                if (SelectedCellProperties.Raw != null) {
                    if (SelectedCellProperties.Raw[0] == '=')
                        SelectedCellProperties.Text = Function.Compile(SelectedCellProperties.Raw).Invoke();
                    else SelectedCellProperties.Text = SelectedCellProperties.Raw;
                }                
            }
            catch {
                SelectedCellProperties.Text = SelectedCellProperties.Raw;
            }
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
            Window functionselectorWindow = new FunctionSelector(SelectedCellProperties);
            functionselectorWindow.ShowDialog();
		}
        private void clearCellBtn_Click(object sender, RoutedEventArgs e) {
            selectedCellProperties.Text = "";
            selectedCellProperties.Raw = "";
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
            if(SelectedCellProperties == null) return;
            CellEditWindow editWindow = new CellEditWindow(table_GRD.ColumnDefinitions[SelectedCellProperties.X].Width, table_GRD.RowDefinitions[SelectedCellProperties.Y].Height);
            editWindow.ShowDialog();
            if (editWindow.DialogResult == false) return;
            table_GRD.ColumnDefinitions[SelectedCellProperties.X].Width = new GridLength(double.Parse(editWindow.CellEditModell.Width), editWindow.CellEditModell.WidthType); 
            table_GRD.RowDefinitions[SelectedCellProperties.Y].Height = new GridLength(double.Parse(editWindow.CellEditModell.Height), editWindow.CellEditModell.HeightType);
		}

		private void save_BTN_Click(object sender, RoutedEventArgs e)
		{

            MessageBoxResult result = MessageBox.Show("Biztosan menteni akar?", "Mentés", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK) 
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog();
                if (!folderDialog.ShowDialog().Value) return;
                loader.Save(SaveFormat, Path.Combine(folderDialog.FolderName, FileName), table_GRD.ColumnDefinitions, table_GRD.RowDefinitions);
                Title = FileName;
            }
		}
        private void formatcopy_Click(object sender, RoutedEventArgs e)
        {
            isCopying = true;
        }

        private void tablestriped_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 2; i < cellPropertiesModels.Count; i+=2)
            {
                foreach (CellPropertiesModel cell in cellPropertiesModels[i])
                {
                    darkenCellPropertyBG(cellPropertiesModels[i-1][0], cell);
                }
            }
            IsStriped = !IsStriped;
        }
        private Color darkenColor(Color color, double factor = 0.9)
        {
            return Color.FromArgb(
                color.A,
                (byte)(color.R * factor),
                (byte)(color.G * factor),
                (byte)(color.B * factor));
        }
        private void darkenCellPropertyBG(CellPropertiesModel baseProperties, CellPropertiesModel cellPropertiesModel)
        {
            if (baseProperties.Background_Color is SolidColorBrush brush)
            {
                if (!isStriped) 
                {
                    Color originalColor = brush.Color;
                    Color darkerColor = darkenColor(originalColor, 0.85); // 90% brightness
                    cellPropertiesModel.Background_Color = new SolidColorBrush(darkerColor);
                }
                else
                {
                    cellPropertiesModel.Background_Color = brush;
                }
            }
        }

		private void open_Click(object sender, RoutedEventArgs e)
		{
            OpenFileDialog ofd = new OpenFileDialog();
            if (!ofd.ShowDialog().Value) return;
            loader.Load(ofd.FileName, table_GRD);
            string filename = ofd.FileName.Split('.').SkipLast(1).LastOrDefault("Excel").Split('\\').LastOrDefault("Excel");
            Title = filename;
            FileName = filename;
		}
	}
}