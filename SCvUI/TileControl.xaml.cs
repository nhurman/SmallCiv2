using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SCvLib;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace SCvUI
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class TileControl : UserControl
    {
        public TileType Terrain { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public ITile Tile { get; set; }
        public TileControl(ITile t) : base()
        {
            Tile = t;

            InitializeComponent();
            IUnit u = MainWindow.Instance.SelectedUnit;
            if (u != null && u.PlayerId == Game.Instance.CurrentPlayerId)
            {
                if (u.MoveCost(t) <= u.Mvt)
                {
                    this.HexPath.StrokeThickness = 4;
                }
            }

            switch (Tile.Terrain)
            {
                case TileType.Desert: this.HexPath.Fill = FindResource("DesertBrush") as Brush;
                    break;

                case TileType.Mountain: this.HexPath.Fill = FindResource("MountainBrush") as Brush;
                    break;

                case TileType.Forest: this.HexPath.Fill = FindResource("ForestBrush") as Brush;
                    break;

                case TileType.Field: this.HexPath.Fill = FindResource("FieldBrush") as Brush;
                    break;
            }
        }

        public void RightClick()
        {
            if (MainWindow.Instance.SelectedUnit == null) return;

            MainWindow.Instance.SelectedUnit.AttackOrMoveTo(Tile);
            MainWindow.Instance.Paint();
        }

        private void HexPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClick();
        }

        private void HexPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Tile.Units.Count > 0) return;
            MainWindow.Instance.SelectedUnit = null;
            MainWindow.Instance.Paint();
        }
    }
}
