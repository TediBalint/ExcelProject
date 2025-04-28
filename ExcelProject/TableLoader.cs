using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;


namespace ExcelProject
{
    public class TableLoader
    {
        private ObservableCollection<ObservableCollection<CellPropertiesModel>> cellPropertiesModels;
		public TableLoader(ObservableCollection<ObservableCollection<CellPropertiesModel>> _cellPropertiesModels) 
        {
            cellPropertiesModels = _cellPropertiesModels;
        }
        public void Load(string path, Grid grd)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string? data = sr.ReadLine();
                if (data == null) return;
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
				data = sr.ReadLine();
                if (data == null) throw new FileFormatException("Wrong file save format");
				string[] row_list = data.Substring(0, data.Length - 1).Split(';');
				foreach (string row in col_list)
				{
					string[] row_data = row.Split(':');
					GridUnitType gridUnitType = GridUnitType.Pixel;
					if (row_data[1] == GridUnitType.Star.ToString()) gridUnitType = GridUnitType.Star;
					RowDefinition columnDefinition = new RowDefinition();
					columnDefinition.Height = new GridLength(double.Parse(row_data[0]), gridUnitType);
					grd.RowDefinitions.Add(columnDefinition);
				}
                data = sr.ReadLine();
				if (data == null) throw new FileFormatException("Wrong file save format");
				
			}
        }
        private void readCellInfo(string data)
        {
			string[] cell_infos = data.Substring(0, data.Length - 1).Split(';');
			foreach (string cell_info in cell_infos)
			{
                string[] cell_data = cell_info.Split(":");

			}
		}
        public void Save(string format, string path, ColumnDefinitionCollection columnDefinitions, RowDefinitionCollection rowDefinitions)
        {
            Debug.WriteLine($"{path}.{format.ToLower()}");
			using (StreamWriter sw = new StreamWriter($"{path}.{format.ToLower()}"))
            {
                foreach (ColumnDefinition column in columnDefinitions)
                {
                    sw.Write($"{column.Width.Value};{column.Width.GridUnitType}:");
                }
                sw.WriteLine();
				foreach (RowDefinition row in rowDefinitions)
				{
					sw.Write($"{row.Height.Value};{row.Height.GridUnitType}:");
				}
                sw.WriteLine();
                foreach (ObservableCollection<CellPropertiesModel> row in cellPropertiesModels)
                {
                    foreach (CellPropertiesModel cell in row)
                    {
                        sw.Write(cell);
                    }
                }
			}
        }
    }
}
