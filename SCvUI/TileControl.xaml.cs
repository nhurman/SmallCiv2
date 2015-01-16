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
        public TileType Terrain { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }

        public ITile Tile { get; set; }

        public TileControl() : base()
        {
            
        }

        public TileControl(int x, int y, TileType type, ITile t) : base()
        {
            X = x;
            Y = y;
            Tile = t;
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

        public void RightClick()
        {
            if (MainWindow.INSTANCE._selectedUnit == null) return;
            if (MainWindow.INSTANCE._selectedUnit.PlayerId != MainWindow.INSTANCE._game.CurrentPlayerId) return;

            MainWindow.INSTANCE._game.Map.AttackOrMoveTo(MainWindow.INSTANCE._selectedUnit, Tile);
            MainWindow.INSTANCE.Paint();
            //Delay(100, (o, a) => MainWindow.INSTANCE.Paint());
        }

        private void HexPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClick();
        }

        static void Delay(int ms, EventHandler action)
        {
            var tmp = new Timer { Interval = ms };
            tmp.Tick += new EventHandler((o, e) => tmp.Enabled = false);
            tmp.Tick += action;
            tmp.Enabled = true;
        }
    }
}
