using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExcelProject
{
	public static class Statics
	{
		public static List<KeyValuePair<DependencyProperty, string>> CellPropNames = new List<KeyValuePair<DependencyProperty, string>>()
		{
			new KeyValuePair<DependencyProperty, string>(TextBox.TextProperty, "Text"),
			new KeyValuePair<DependencyProperty, string>(TextBox.FontFamilyProperty, "Font_Family"),
			new KeyValuePair<DependencyProperty, string>(TextBox.FontSizeProperty, "Font_Size"),
			new KeyValuePair<DependencyProperty, string>(TextBox.FontWeightProperty, "Font_Weight"),
			new KeyValuePair<DependencyProperty, string>(TextBox.FontStyleProperty, "Font_Style"),
			new KeyValuePair<DependencyProperty, string>(TextBox.TextDecorationsProperty, "Text_Decoration"),
			new KeyValuePair<DependencyProperty, string>(TextBox.ForegroundProperty, "Foreground_Color"),
			new KeyValuePair<DependencyProperty, string>(TextBox.BackgroundProperty, "Background_Color"),
			new KeyValuePair<DependencyProperty, string>(TextBox.VerticalContentAlignmentProperty, "Vertical_Content_Align"),
			new KeyValuePair<DependencyProperty, string>(TextBox.HorizontalContentAlignmentProperty, "Horizontal_Content_Align"),
			new KeyValuePair<DependencyProperty, string>(TextBox.BorderBrushProperty, "Border_Color"),
			new KeyValuePair<DependencyProperty, string>(TextBox.BorderThicknessProperty, "Border_Thickness")
		};

		public static Dictionary<FontWeight, Brush> WeightToBrush = new Dictionary<FontWeight, Brush>()
		{ {FontWeights.Bold, Brushes.Gray}, {FontWeights.Normal, Brushes.LightGray }};

		public static Dictionary<FontStyle, Brush> StyleToBrush = new Dictionary<FontStyle, Brush>()
		{ {FontStyles.Italic, Brushes.Gray}, {FontStyles.Normal, Brushes.LightGray }};

		public const string FONT_FILE_PATH = "Data/fonts.txt";
		public const string DEFAULT_FONT_SIZE_FILE_PATH = "Data/default_font_size.txt";

		public static ObservableCollection<KeyValuePair<string, TextDecorationCollection?>> textDecorations = new ObservableCollection<KeyValuePair<string, TextDecorationCollection?>>()
		{   new KeyValuePair<string, TextDecorationCollection?>("Nincs", null),
			new KeyValuePair<string, TextDecorationCollection?>("Normál", TextDecorations.Underline),
			new KeyValuePair<string, TextDecorationCollection?>("Vékony", TextDecorations.Baseline),
			new KeyValuePair<string, TextDecorationCollection?>("Fölé", TextDecorations.OverLine),
			new KeyValuePair<string, TextDecorationCollection?>("Áthúzás", TextDecorations.Strikethrough)
        };
		public static ObservableCollection<KeyValuePair<string, Brush>> foregroundBrushes = new ObservableCollection<KeyValuePair<string, Brush>>()
		{
			new KeyValuePair<string, Brush>("Fekete" ,Brushes.Black),
			new KeyValuePair<string, Brush>("Fehér" ,Brushes.White),
			new KeyValuePair<string, Brush>("Piros" ,Brushes.Red),
			new KeyValuePair<string, Brush>("Kék" ,Brushes.Blue),
			new KeyValuePair<string, Brush>("Zöld" ,Brushes.Green),
			new KeyValuePair<string, Brush>("Barna" ,Brushes.Brown),
			new KeyValuePair<string, Brush>("Narancs" ,Brushes.Orange),
			new KeyValuePair<string, Brush>("Szürke" ,Brushes.DarkGray),
			new KeyValuePair<string, Brush>("Arany" ,Brushes.Gold),
        };
		public static ObservableCollection<GridUnitType> gridUnitTypes = new ObservableCollection<GridUnitType>() { GridUnitType.Star, GridUnitType.Pixel, };
		public static ObservableCollection<string> saveFormats = new ObservableCollection<string>()
		{
			"TXT", "CSV"
		};
		public static GridLength DefaultHeight = new GridLength(1, GridUnitType.Star);
		public static GridLength DefaultWidth = new GridLength(1, GridUnitType.Star);
		public static Regex CellCoordRegex = new(@"^[A-Z]+\d+$");
		public static Regex CellTerritoryRegex = new(@"^[A-Z]+\d+:[A-Z]+\d+$");
		public static Regex CriteriaRegex = new(@"^(>|<|<>|!=|<=|>=)?\d+$");
		public static ObservableCollection<ObservableCollection<CellPropertiesModel>> CellPropertiesModels = new();
    }

		
	
}
