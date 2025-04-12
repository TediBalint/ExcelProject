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
        private string text;
        public string Text 
        { 
            get { return text; } 
            set { text = value; OnPropertyChanged(nameof(Text));}
        }
        private FontFamily font_Family;
        public FontFamily Font_Family 
        { 
            get { return font_Family; }
            set { font_Family = value;OnPropertyChanged(nameof(Font_Family)); } 
        }
        public GridLength Width { get; set; }
        public GridLength Height { get; set; }
        private double font_Size;
        public double Font_Size 
        {
            get { return font_Size; }
            set { font_Size = value; OnPropertyChanged(nameof(Font_Size)); } 
        }
        private FontWeight font_Weight;
        public FontWeight Font_Weight 
        { 
            get { return font_Weight; }
            set { font_Weight = value; OnPropertyChanged(nameof(font_Weight)); }
        }
		private FontStyle font_Style { get; set; }
		public FontStyle Font_Style 
        {
            get { return font_Style; }
            set { font_Style = value; OnPropertyChanged(nameof(Font_Style)); }
        }
        private TextDecorationCollection text_Decoration;
        public TextDecorationCollection Text_Decoration 
        {
            get { return text_Decoration; }
            set { text_Decoration = value; OnPropertyChanged(nameof(Text_Decoration)); } 
        }
        private Brush foreground_Color;
        public Brush Foreground_Color 
        {
            get { return foreground_Color; }
            set { foreground_Color = value; OnPropertyChanged(nameof(Foreground_Color)); } 
        }
        private Brush background_Color;
		public Brush Background_Color
		{
			get { return background_Color; }
			set { background_Color = value; OnPropertyChanged(nameof(Background_Color)); }
		}
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
