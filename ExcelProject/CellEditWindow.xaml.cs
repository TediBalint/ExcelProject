using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExcelProject
{
    /// <summary>
    /// Interaction logic for CellEditWindow.xaml
    /// </summary>
    public partial class CellEditWindow : Window
    {
        public CellEditModell CellEditModell { get; set; }
        public ObservableCollection<GridUnitType> gridUnitTypes { get; set; } = Statics.gridUnitTypes;
        public CellEditWindow(GridLength width, GridLength height)
        {
            InitializeComponent();
            DataContext = this;
            CellEditModell = new CellEditModell
            {
                Width = width.Value.ToString(),
                Height = height.Value.ToString(),
                WidthType = width.GridUnitType,
                HeightType = height.GridUnitType
            };
        }

        private void Ok_BTN_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
