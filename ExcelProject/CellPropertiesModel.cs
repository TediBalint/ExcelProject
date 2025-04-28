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
        public int X { get; set; }
        public int Y { get; set; }
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
        private TextDecorationCollection? text_Decoration;
        public TextDecorationCollection? Text_Decoration 
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
        public VerticalAlignment vertical_Content_Align;
        public VerticalAlignment Vertical_Content_Align 
        {
            get { return vertical_Content_Align; } 
            set { vertical_Content_Align = value; OnPropertyChanged(nameof(Vertical_Content_Align)); }
        }
        private HorizontalAlignment horizontal_Content_Align;
        public HorizontalAlignment Horizontal_Content_Align 
        {
            get { return horizontal_Content_Align; }
            set { horizontal_Content_Align = value; OnPropertyChanged(nameof(Horizontal_Content_Align)); } 
        }
        private Thickness border_Thickness;
        public Thickness Border_Thickness
        {
            get { return border_Thickness; }
            set { border_Thickness = value; OnPropertyChanged(nameof(Border_Thickness));}
        }
        private Brush border_Color;
        public Brush Border_Color
        {
            get { return border_Color; }
            set { border_Color = value; OnPropertyChanged(nameof(Border_Color)); }
        }
        public CellPropertiesModel(int x, int y, bool setValues)
        {
            if (setValues)
            {
                X = x;
                Y = y;
                Text = "Default";
                Font_Size = 16;              
                Font_Family = new FontFamily("Arial");
                Font_Weight = FontWeights.Normal;
                Font_Style = FontStyles.Normal;
                Text_Decoration = null;
                Foreground_Color = Brushes.Black;
                Background_Color = Brushes.White;
                Vertical_Content_Align = VerticalAlignment.Center;
                Horizontal_Content_Align = HorizontalAlignment.Center;
                Border_Thickness = new Thickness(1.0);
                Border_Color = Brushes.Black;
            }
        }
        public CellPropertiesModel()
        {
            
        }
        public void CopyProps(CellPropertiesModel model)
        {
            Font_Size = model.Font_Size;
            Font_Family = model.Font_Family;
            Font_Weight = model.Font_Weight;
            Font_Style = model.Font_Style;
            Text_Decoration = model.Text_Decoration;
            Foreground_Color = model.Foreground_Color;
            Background_Color = model.Background_Color;
            Vertical_Content_Align = model.Vertical_Content_Align;
            Horizontal_Content_Align = model.Horizontal_Content_Align;
            Border_Thickness = model.Border_Thickness;
            Border_Color = Brushes.Black;
        }
        public CellPropertiesModel GetClone()
        {
            return (CellPropertiesModel)MemberwiseClone();
        }
		public void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		public override string ToString()
		{
			return base.ToString();
		}
		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
