using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExcelProject
{
    public class CellEditModell
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public GridUnitType WidthType { get; set; }
        public GridUnitType HeightType { get; set; }
    }
}
