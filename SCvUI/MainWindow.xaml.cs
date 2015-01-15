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

namespace SCvUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MainMenu.Visibility = Visibility.Visible;
            this.GameCreator.Visibility = Visibility.Collapsed;
            this.InGame.Visibility = Visibility.Collapsed;
        }

        #region Main menu Events
        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            this.GameCreator.Visibility = Visibility.Collapsed;
        }

        private void Create_OnClick(object sender, RoutedEventArgs e)
        {
            this.GameCreator.Visibility = Visibility.Visible;
        }
        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            this.MainMenu.Visibility = Visibility.Collapsed;
            this.InGame.Visibility = Visibility.Visible;
        }
        #endregion



        
    }

}
