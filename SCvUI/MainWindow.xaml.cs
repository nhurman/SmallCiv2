using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Threading;
using SCvLib;

namespace SCvUI
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private class IntPoint
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private Game _game = null;
        private bool _isPaused = false;
        private Unit _selectedUnit = null;
        private IntPoint _panDirection;
        private DispatcherTimer _hotzoneTimer = null;

        private static double s_panMargin = 10.0;
        private static double s_panSpeed = 10.0;

        public MainWindow()
        {
            InitializeComponent();
            this._selectedUnit = new Unit {HP = 2, HPMax = 2, Atk = 3, Def = 4, Mvt = 5};

            _panDirection = new IntPoint();
            _hotzoneTimer = new DispatcherTimer();
            _hotzoneTimer.Tick += OnScrollTick;
            _hotzoneTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            _hotzoneTimer.Start();
            
            this.MainMenu.Visibility = Visibility.Visible;
            this.GameCreator.Visibility = Visibility.Collapsed;
            this.InGame.Visibility = Visibility.Collapsed;

            EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            EventManager.RegisterClassHandler(typeof(Window),
                Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            ////////FIXME TESTS
            NewGame(MapType.Demo, new List<Tuple<string, FactionType>>()
            {
                Tuple.Create("Player1", FactionType.Elves),
                Tuple.Create("Player2", FactionType.Orcs)
            });
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
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

        private void NewGame(MapType map, List<Tuple<string, FactionType>> players)
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

            var players = new List<Tuple<string, FactionType>>
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
            UpdateHotzones();
            Paint();
            MouseUtilities.ClipCursor(IntPtr.Zero);
        }

        public void OnResume()
        {
            this._isPaused = false;
            this.FinishTurn.IsEnabled = true;
            UpdateHotzones();
            Paint();

            Point targetLoc = this.MainGrid.PointToScreen(new Point(0, 0));
            System.Drawing.Rectangle r = new System.Drawing.Rectangle(
                (int) targetLoc.X,
                (int) targetLoc.Y,
                (int) (targetLoc.X + this.MainGrid.ActualWidth),
                (int) (targetLoc.Y + this.MainGrid.ActualHeight));
            MouseUtilities.ClipCursor(ref r);
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

        private void FinishTurn_Click(object sender, RoutedEventArgs e)
        {
            this.Map.Margin = new Thickness(-100, -100, 0, 0);
        }

        #region Hotzones
        private void UpdateHotzones()
        {
            var active = 0;
            if (0 != _panDirection.X) ++active;
            if (0 != _panDirection.Y) ++active;

            if (0 == active || _isPaused)
            {
                _hotzoneTimer.Stop();
            }
            else
            {
                _hotzoneTimer.Start();
            }
        }
        private void OnScrollTick(object sender, EventArgs e)
        {
            this.Map.Margin = new Thickness(
                this.Map.Margin.Left + _panDirection.X * s_panSpeed,
                this.Map.Margin.Top + _panDirection.Y * s_panSpeed,
                0, 0);
        }
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (null == this.Map) return;

            Point p = MouseUtilities.GetMousePosition(this);

            if (p.X < s_panMargin)
            {
                _panDirection.X = 1;
            }
            else if (p.X > this.MainGrid.ActualWidth - s_panMargin - 1)
            {
                _panDirection.X = -1;
            }
            else
            {
                _panDirection.X = 0;
            }

            if (p.Y < s_panMargin)
            {
                _panDirection.Y = 1;
            }
            else if (p.Y > this.MainGrid.ActualHeight - s_panMargin - 1)
            {
                _panDirection.Y = -1;
            }
            else
            {
                _panDirection.Y = 0;
            }

            UpdateHotzones();
        }
        #endregion
    }
}
