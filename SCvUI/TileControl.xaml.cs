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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SCvLib;

namespace SCvUI
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class TileControl : UserControl
    {
        public TileType Terrain { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }

        public TileControl() : base()
        {
            
        }

        public TileControl(int x, int y, TileType type) : base()
        {
            X = x;
            Y = y;
            Terrain = type;

            InitializeComponent();
            switch (Terrain)
            {
                case TileType.Desert: this.HexPath.Fill = FindResource("ForestBrush") as Brush;
                    break;

                case TileType.Mountain: this.HexPath.Fill = FindResource("MountainBrush") as Brush;
                    break;

                case TileType.Forest: this.HexPath.Fill = FindResource("ForestBrush") as Brush;
                    break;

                case TileType.Field: this.HexPath.Fill = FindResource("FieldBrush") as Brush;
                    break;
            }
        }
    }
}
