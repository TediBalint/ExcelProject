using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
			new KeyValuePair<DependencyProperty, string>(TextBox.HorizontalContentAlignmentProperty, "Horizontal_Content_Align")
		};

		public static Dictionary<FontWeight, Brush> WeightToBrush = new Dictionary<FontWeight, Brush>()
		{ {FontWeights.Bold, Brushes.Gray}, {FontWeights.Normal, Brushes.LightGray }};

		public static Dictionary<FontStyle, Brush> StyleToBrush = new Dictionary<FontStyle, Brush>()
		{ {FontStyles.Italic, Brushes.Gray}, {FontStyles.Normal, Brushes.LightGray }};

		public const string FONT_FILE_PATH = "Data/fonts.txt";
		public const string DEFAULT_FONT_SIZE_FILE_PATH = "Data/default_font_size.txt";

		public static ObservableCollection<KeyValuePair<string, TextDecorationCollection?>> textDecorations { get; set; } = new ObservableCollection<KeyValuePair<string, TextDecorationCollection?>>()
		{	new KeyValuePair<string, TextDecorationCollection?>("Nincs", null),
			new KeyValuePair<string, TextDecorationCollection?>("Normál", TextDecorations.Underline),
			new KeyValuePair<string, TextDecorationCollection?>("Vékony", TextDecorations.Baseline),
			new KeyValuePair<string, TextDecorationCollection?>("Fölé", TextDecorations.OverLine),
			new KeyValuePair<string, TextDecorationCollection?>("Áthúzás", TextDecorations.Strikethrough)
		};
	}

		
	
}
