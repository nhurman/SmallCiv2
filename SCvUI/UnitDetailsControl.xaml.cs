using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SCvLib;

namespace SCvUI
{
    /// <summary>
    /// Interaction logic for UnitDetailsControl.xaml
    /// </summary>
    public partial class UnitDetailsControl : UserControl
    {
        public IUnit Unit { get; set; }

        public UnitDetailsControl(IUnit unit)
        {
            InitializeComponent();
            Unit = unit;

            if (MainWindow.Instance.SelectedUnit == Unit)
            {
                this.BgRect.Fill = new SolidColorBrush(Colors.White);
            }

            switch (Unit.Faction)
            {
                case FactionType.Dwarves: this.Image.Source = FindResource("DwarfImage") as ImageSource;
                    break;

                case FactionType.Elves: this.Image.Source = FindResource("ElfImage") as ImageSource;
                    break;

                case FactionType.Orcs: this.Image.Source = FindResource("OrcImage") as ImageSource;
                    break;
            }

            this.HP.Content = Unit.HP;
            this.Mvt.Content = Unit.Mvt;
            this.Atk.Content = Unit.Atk;
            this.Def.Content = Unit.Def;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.SelectedUnit = Unit;
            MainWindow.Instance.Paint();
        }
    }
}
