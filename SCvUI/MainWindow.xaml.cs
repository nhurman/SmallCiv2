using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Threading;
using SCvLib;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;


namespace SCvUI
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow
    {
        public static MainWindow Instance;

        private Game _game;
        private bool _isPaused;
        private Point _panDirection;
        private DispatcherTimer _panTimer;

        private static double s_panMargin = 10.0;
        private static double s_panSpeed = 10.0;

        public IUnit SelectedUnit;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
            SelectedUnit = null;

            _panTimer = null;
            _isPaused = false;
            _game = null;
            _panDirection = new Point();
            _panTimer = new DispatcherTimer();
            _panTimer.Tick += OnScrollTick;
            _panTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            _panTimer.Start();

            
            MainMenu.Visibility = Visibility.Visible;
            GameCreator.Visibility = Visibility.Collapsed;
            InGame.Visibility = Visibility.Collapsed;

            EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            EventManager.RegisterClassHandler(typeof(Window),
                Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            if (_isPaused) OnResume();
            else OnPause();
        }

        public void Paint()
        {
            // Are we in a game
            if (null == _game)
            {
                MainMenu.Visibility = Visibility.Visible;
                InGame.Visibility = Visibility.Collapsed;
                PauseMenu.Visibility = Visibility.Collapsed;
                return;
            }

            MainMenu.Visibility = Visibility.Collapsed;
            InGame.Visibility = Visibility.Visible;

            // Header
            Name1.Content = _game.Player1.Name;
            Name2.Content = _game.Player2.Name;
            Score1.Content = _game.Player1.Score.ToString();
            Score2.Content = _game.Player2.Score.ToString();
            Turn.Content = string.Format("{0}/{1}", _game.Turn, _game.Map.Turns);

            Border1.Visibility = (_game.CurrentPlayerId == 0) ? Visibility.Visible : Visibility.Hidden;
            Border2.Visibility = (_game.CurrentPlayerId == 1) ? Visibility.Visible : Visibility.Hidden;

            InGame.Visibility = Visibility.Visible;
            PauseMenu.Visibility = _isPaused ? Visibility.Visible : Visibility.Collapsed;

            // Unit panel
            UnitInfoGrid.Visibility = (null != SelectedUnit) ? Visibility.Visible : Visibility.Collapsed;
            UnitInfoGridContents.Children.Clear();
            if (null != SelectedUnit)
            {
                var i = 0;
                foreach (Unit u in SelectedUnit.Tile.Units)
                {
                    var info = new UnitDetailsControl(u);
                    UnitInfoGridContents.Children.Add(info);
                    Grid.SetRow(info, i);
                    ++i;
                }
            }

            // Map Grid
            MapGrid.Children.Clear();
            foreach (var t in _game.Map.Tiles)
            {
                var tileControl = new TileControl(t);
                MapGrid.Children.Add(tileControl);
                Grid.SetColumn(tileControl, t.X);
                Grid.SetRow(tileControl, t.Y);

                foreach (var u in t.Units)
                {
                    var unitControl = new UnitControl(u, tileControl)
                    {
                        Margin = new Thickness(
                            3*_game.Random.Next(-5, 5),
                            3 * _game.Random.Next(-5, 5),
                            0, 0)
                    };
                    tileControl.Grid.Children.Add(unitControl);

                    if (u.Mvt >= 2) Panel.SetZIndex(unitControl, 4);
                    else if (u.Mvt >= 1) Panel.SetZIndex(unitControl, 3);
                    else if (u.Mvt >= 0.5) Panel.SetZIndex(unitControl, 2);
                    else Panel.SetZIndex(unitControl, 1);
                }
            }
        }
        
        #region Main menu Events
        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            GameCreator.Visibility = Visibility.Collapsed;
            _game = GameBuilder.Load();
            OnResume();
        }

        private void Create_OnClick(object sender, RoutedEventArgs e)
        {
            GameCreator.Visibility = Visibility.Visible;
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            if (!PlayerCreator1.IsValid() || !PlayerCreator2.IsValid())
                return;

            if (!MapSelector.IsValid())
                return;

            if (PlayerCreator1.Faction() == PlayerCreator2.Faction())
                return;

            _game = GameBuilder.New(MapSelector.Map(), PlayerCreator1.Player(), PlayerCreator2.Player());
            OnResume();
        }
        #endregion        

        #region Pause menu
        public void OnPause()
        {
            _isPaused = true;
            InGame.IsEnabled = false;
            InGame.Effect = new BlurEffect();
            UpdateHotzones();
            Paint();
            MouseUtilities.ClipCursor(IntPtr.Zero);
        }

        public void OnResume()
        {
            _isPaused = false;
            InGame.IsEnabled = true;
            UpdateHotzones();
            InGame.Effect = null;
            Paint();

            if (_game == null) return;
            var targetLoc = MainGrid.PointToScreen(new System.Windows.Point(0, 0));
            Rectangle r = new Rectangle(
                (int)targetLoc.X,
                (int)targetLoc.Y,
                (int)(targetLoc.X + MainGrid.ActualWidth),
                (int)(targetLoc.Y + MainGrid.ActualHeight));
            MouseUtilities.ClipCursor(ref r);
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            OnResume();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            _game = null;
            Paint();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            GameBuilder.Save(_game);
            OnResume();
        }
        #endregion

        private void FinishTurn_Click(object sender, RoutedEventArgs e)
        {
            _game.NextTurn();
            if (_game.IsEnded())
            {
                OnGameEnded();
            }

            Paint();
        }

        private void OnGameEnded()
        {
            if (_game.Player1.Score > _game.Player2.Score)
            {
                MessageBox.Show(string.Format("{0} wins!", _game.Player1.Name));
            }
            else if (_game.Player2.Score > _game.Player1.Score)
            {
                MessageBox.Show(string.Format("{0} wins!", _game.Player2.Name));
            }
            else
            {
                MessageBox.Show(string.Format("Draw!"));
            }

            _game = null;
            Paint();
        }

        #region Hotzones
        private void UpdateHotzones()
        {
            var active = 0;
            if (0 != _panDirection.X) ++active;
            if (0 != _panDirection.Y) ++active;

            if (0 == active || _isPaused)
            {
                _panTimer.Stop();
            }
            else
            {
                _panTimer.Start();
            }
        }
        private void OnScrollTick(object sender, EventArgs e)
        {
            var x = MapGrid.Margin.Left + _panDirection.X * s_panSpeed;
            var y = MapGrid.Margin.Top + _panDirection.Y * s_panSpeed;

            if (0 == MapGrid.Children.Count) return;

            var totalW = MapGrid.Children[0].RenderSize.Width;
            totalW = (totalW + totalW/Math.Sqrt(3))*Math.Sqrt(MapGrid.Children.Count)/2;
            
            var totalH = totalW;

            if (x < 100 - totalW) x = 100 - totalW;
            if (x > MainGrid.ActualWidth - 100) x = MainGrid.ActualWidth - 100;
            
            if (y < 0 - totalH) y = 0 - totalH;
            if (y > MainGrid.ActualHeight - 150) y = MainGrid.ActualHeight - 150;

            MapGrid.Margin = new Thickness(x, y, 0, 0);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (null == MapGrid) return;

            Point p = MouseUtilities.GetMousePosition(this);

            if (p.X < s_panMargin)
            {
                _panDirection.X = 1;
            }
            else if (p.X > MainGrid.ActualWidth - s_panMargin - 1)
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
            else if (p.Y > MainGrid.ActualHeight - s_panMargin - 1)
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
