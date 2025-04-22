using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ExcelProject
{
    public class TableLoader
    {
        private ObservableCollection<ObservableCollection<CellPropertiesModel>> cellPropertiesModels;
		public TableLoader(ObservableCollection<ObservableCollection<CellPropertiesModel>> _cellPropertiesModels) 
        {
            cellPropertiesModels = _cellPropertiesModels;
        }
        public void Load(string path)
        {
           
        }
        public void Save(string format)
        {
            switch (format)
            {
                case "JSON":
                    SaveJSON(); break;
                case "TXT":
                    SaveTXT(); break;
                case "CSV":
                    SaveCSV(); break;
                default:
                    break;
            }
        }
        private void SaveCSV()
        {

        }
        private void SaveJSON()
        {

        }
        private void SaveTXT()
        {

        }
    }
}
