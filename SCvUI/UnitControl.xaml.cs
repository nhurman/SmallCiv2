﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
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
    /// Interaction logic for UnitControl.xaml
    /// </summary>
    public partial class UnitControl : UserControl
    {
        private TileControl _tile;
        public IUnit Unit;

        public UnitControl(IUnit unit, TileControl tile)
        {
            InitializeComponent();
            _tile = tile;
            Unit = unit;

            switch (Unit.Faction)
            {
                case FactionType.Dwarves: this.Image.Source = FindResource("DwarfImage") as ImageSource;
                    break;

                case FactionType.Elves: this.Image.Source = FindResource("ElfImage") as ImageSource;
                    break;

                case FactionType.Orcs: this.Image.Source = FindResource("OrcImage") as ImageSource;
                    break;
            }

            if (Game.Instance.CurrentPlayerId != Unit.PlayerId)
            {
                Effect = new BlurEffect();
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            _tile.HexPath.Stroke = new SolidColorBrush(Colors.Black);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _tile.HexPath.Stroke = new SolidColorBrush(Colors.LightGray);
        }
        
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.SelectedUnit = Unit;
            MainWindow.Instance.Paint();
        }
        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _tile.RightClick();
        }
    }
}
