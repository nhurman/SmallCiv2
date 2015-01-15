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
    /// Interaction logic for MapSelector.xaml
    /// </summary>
    public partial class MapSelector : UserControl
    {
        public MapSelector()
        {
            InitializeComponent();
        }

        public bool IsValid()
        {
            return "" != this.LMap.Text;
        }

        public MapName Map()
        {
            switch (this.LMap.Text)
            {
                case "Demo":
                    return MapName.Demo;
                case "Small":
                    return MapName.Small;
                case "Normal":
                    return MapName.Normal;
                default:
                    return MapName.Demo;
            }
        }
    }
}
