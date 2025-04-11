using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExcelProject
{
    public class CellPropertiesModel: INotifyPropertyChanged
    {
        public string Text { get; set; }
        public FontFamily Font_Family { get; set; }
        public GridLength Width { get; set; }
        public GridLength Height { get; set; }
        public double Font_Size { get; set; }
        private FontWeight font_Weight;
        public FontWeight Font_Weight 
        { 
            get { return font_Weight; }
            set { font_Weight = value; OnPropertyChanged(nameof(font_Weight)); }
        }
		public FontStyle Font_Style { get; set; }
        public TextDecorationCollection Text_Decoration { get; set; }
        public Brush Foreground_Color { get; set; } 
        public Brush Background_Color { get; set; }
        public VerticalAlignment Vertical_Content_Align { get; set; }
        public HorizontalAlignment Horizontal_Content_Align { get; set; }
           

        public CellPropertiesModel(bool setValues)
        {
            if (setValues)
            {
                Text = "Default";
                Font_Size = 16;
                Width = new GridLength(1, GridUnitType.Star);
                Font_Family = new FontFamily("Arial");
                Height = new GridLength(1, GridUnitType.Star);
                Font_Weight = FontWeights.Bold;
                Font_Style = FontStyles.Italic;
                Text_Decoration = TextDecorations.OverLine;
                Foreground_Color = Brushes.Black;
                Background_Color = Brushes.White;
                Vertical_Content_Align = VerticalAlignment.Center;
                Horizontal_Content_Align = HorizontalAlignment.Center;
            }
        }
        public CellPropertiesModel()
        {
            
        }
		public void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
