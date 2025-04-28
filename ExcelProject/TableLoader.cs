using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace ExcelProject
{
	public class TableLoader : INotifyPropertyChanged
	{
        public void Load(string path, Grid grd)
        {
            grd.ColumnDefinitions.Clear();
            grd.RowDefinitions.Clear();
			using (StreamReader sr = new StreamReader(path))
            {
                string? data = sr.ReadLine();
                if (data == null) return;
				Debug.WriteLine("Reading col info");
				readColInfo(data, grd);
				data = sr.ReadLine();
                if (data == null) throw new FileFormatException("Wrong file save format");
                Debug.WriteLine("Reading row info");
				readRowInfo(data, grd);
                data = sr.ReadLine();
				if (data == null) throw new FileFormatException("Wrong file save format");
                Debug.WriteLine("Reading cell info");
                readCellInfo(data);
				
			}
        }
        private void readColInfo(string data, Grid grd)
        {
			string[] col_list = data.Substring(0, data.Length - 1).Split(';');
			foreach (string col in col_list)
			{
				string[] col_data = col.Split(':');
				GridUnitType gridUnitType = GridUnitType.Pixel;
				if (col_data[1] == GridUnitType.Star.ToString()) gridUnitType = GridUnitType.Star;
				ColumnDefinition columnDefinition = new ColumnDefinition();
				columnDefinition.Width = new GridLength(double.Parse(col_data[0]), gridUnitType);
				grd.ColumnDefinitions.Add(columnDefinition);
			}
		}
        private void readRowInfo(string data, Grid grd)
        {
			string[] row_list = data.Substring(0, data.Length - 1).Split(';');
			foreach (string row in row_list)
			{
				string[] row_data = row.Split(':');
				GridUnitType gridUnitType = GridUnitType.Pixel;
				if (row_data[1] == GridUnitType.Star.ToString()) gridUnitType = GridUnitType.Star;
				RowDefinition columnDefinition = new RowDefinition();
				columnDefinition.Height = new GridLength(double.Parse(row_data[0]), gridUnitType);
				grd.RowDefinitions.Add(columnDefinition);
			}
		}
        private void readCellInfo(string data)
        {
			string[] cell_infos = data.Substring(0, data.Length - 1).Split(';');
			foreach (string cell_info in cell_infos)
			{
                string[] cell_data = cell_info.Split(":");
                int x = int.Parse(cell_data[0]);
                int y = int.Parse(cell_data[1]);
                Statics.CellPropertiesModels[y][x].Text = cell_data[2];
				Statics.CellPropertiesModels[y][x].Font_Family = new FontFamily(cell_data[3]);
				Statics.CellPropertiesModels[y][x].Font_Size = double.Parse(cell_data[4]);
				Statics.CellPropertiesModels[y][x].Font_Style = FontStyles.Italic;
                if(cell_data[5] == "Normal") Statics.CellPropertiesModels[y][x].Font_Style = FontStyles.Normal;
				Statics.CellPropertiesModels[y][x].Text_Decoration = Statics.textDecorations.FirstOrDefault(x => x.Key == cell_data[6], Statics.textDecorations.First()).Value;
				if (Enum.TryParse(cell_data[7], out VerticalAlignment vertical_alignment)) Statics.CellPropertiesModels[y][x].Vertical_Content_Align = vertical_alignment;
				if (Enum.TryParse(cell_data[8], out HorizontalAlignment horizontal_alignment)) Statics.CellPropertiesModels[y][x].Horizontal_Content_Align = horizontal_alignment;
				Statics.CellPropertiesModels[y][x].Background_Color = (SolidColorBrush)new BrushConverter().ConvertFromString(cell_data[9]);
			}
		}
        public void Save(string format, string path, ColumnDefinitionCollection columnDefinitions, RowDefinitionCollection rowDefinitions)
        {
            Debug.WriteLine($"{path}.{format.ToLower()}");
			using (StreamWriter sw = new StreamWriter($"{path}.{format.ToLower()}"))
            {
                foreach (ColumnDefinition column in columnDefinitions)
                {
                    sw.Write($"{column.Width.Value}:{column.Width.GridUnitType};");
                }
                sw.WriteLine();
				foreach (RowDefinition row in rowDefinitions)
				{
					sw.Write($"{row.Height.Value}:{row.Height.GridUnitType};");
				}
                sw.WriteLine();
                foreach (ObservableCollection<CellPropertiesModel> row in Statics.CellPropertiesModels)
                {
                    foreach (CellPropertiesModel cell in row)
                    {
                        sw.Write(cell);
                    }
                }
			}
        }
		public void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
