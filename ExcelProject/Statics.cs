using System;
using System.Collections.Generic;
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
	}
	
}
