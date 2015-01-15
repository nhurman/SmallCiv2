using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game _game;
        private bool _isPaused;
        private Unit _selectedUnit;

        public MainWindow()
        {
            InitializeComponent();
            this._game = null;

            this._selectedUnit = new Unit {HP = 2, HPMax = 2, Atk = 3, Def = 4, Mvt = 5};

            this.MainMenu.Visibility = Visibility.Visible;
            this.GameCreator.Visibility = Visibility.Collapsed;
            this.InGame.Visibility = Visibility.Collapsed;

            ////////FIXME TESTS
            NewGame(MapName.Demo, new List<Tuple<string, FactionName>>()
            {
                Tuple.Create("Player1", FactionName.Elves),
                Tuple.Create("Player2", FactionName.Orcs)
            });

            EventManager.RegisterClassHandler(typeof (Window),
                Keyboard.KeyDownEvent, new KeyEventHandler(keyDown), true);
        }
        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            if (this._isPaused) OnResume();
            else OnPause();
        }

        public void Paint()
        {
            // Are we in a game
            if (null == this._game)
            {
                this.MainMenu.Visibility = Visibility.Visible;
                this.InGame.Visibility = Visibility.Collapsed;
                return;
            }

            this.MainMenu.Visibility = Visibility.Collapsed;
            this.InGame.Visibility = Visibility.Visible;

            // Header
            this.Name1.Content = this._game.Player1.Name;
            this.Name2.Content = this._game.Player2.Name;
            this.Score1.Content = this._game.Player1.Score.ToString();
            this.Score2.Content = this._game.Player2.Score.ToString();
            this.Turn.Content = string.Format("{0}/{1}", this._game.Turn, this._game.LastTurn);

            this.InGame.Visibility = Visibility.Visible;
            this.PauseMenu.Visibility = this._isPaused ? Visibility.Visible : Visibility.Collapsed;

            // Unit panel
            this.SelectedUnit.Visibility = (null != this._selectedUnit) ? Visibility.Visible : Visibility.Collapsed;
            if (null != this._selectedUnit)
            {
                this.UnitName.Content = this._selectedUnit.Name;
                this.UnitHP.Content = string.Format("{0}/{1}", this._selectedUnit.HP, this._selectedUnit.HPMax);
                this.UnitAtk.Content = this._selectedUnit.Atk;
                this.UnitDef.Content = this._selectedUnit.Def;
                this.UnitMvt.Content = this._selectedUnit.Mvt;
            }
        }

        private void NewGame(MapName map, List<Tuple<string, FactionName>> players)
        {
            this._game = GameBuilder.New(map, players);
            this._selectedUnit = null;


            //this._game.Start();

            // Initialize UI
            OnResume();
            Paint();
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
            if (!this.PlayerCreator1.IsValid() || !this.PlayerCreator2.IsValid())
                return;

            if (!this.MapSelector.IsValid())
                return;

            var players = new List<Tuple<string, FactionName>>
            {
                Tuple.Create(this.PlayerCreator1.PlayerName(), this.PlayerCreator1.Faction()),
                Tuple.Create(this.PlayerCreator2.PlayerName(), this.PlayerCreator2.Faction()),
            };

            NewGame(this.MapSelector.Map(), players);
        }
        #endregion        

        #region InGame Events
        
        public void OnPause()
        {
            this._isPaused = true;
            this.FinishTurn.IsEnabled = false;
            Paint();
        }

        public void OnResume()
        {
            this._isPaused = false;
            this.FinishTurn.IsEnabled = true;
            Paint();
        }
        #endregion

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            OnResume();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            this._game = null;
            Paint();
        }
    }

}
