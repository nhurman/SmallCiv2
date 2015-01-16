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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SCvLib;

namespace SCvUI
{
    /// <summary>
    /// Interaction logic for PlayerCreator.xaml
    /// </summary>
    public partial class PlayerCreator : UserControl
    {
        public PlayerCreator()
        {
            InitializeComponent();
        }

        public bool IsValid()
        {
            return ("" != this.LFaction.Text && "" != this.TName.Text);
        }

        public FactionType Faction()
        {
            switch (this.LFaction.Text)
            {
                case "Elves":
                    return FactionType.Elves;
                case "Dwarves":
                    return FactionType.Dwarves;
                case "Orcs":
                    return FactionType.Orcs;
                default:
                    return FactionType.Elves;
            }
        }

        public String PlayerName()
        {
            return this.TName.Text;
        }

        public Player Player()
        {
            return new Player() {Faction = Faction(), Name = PlayerName(), Score = 0};
        }
    }
}
