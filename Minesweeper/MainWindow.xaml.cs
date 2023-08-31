using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;

namespace Minesweeper
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow
    {
        private Open open;
        private GameGenerator gameGenerator;
        private Solver solver;
        public readonly Timer timer;

        public Game.PreviousGame previousGame = Game.PreviousGame.NORMAL;

        /// <summary> true if current game wasn't started already (game field is empty, no buttons are opened) </summary>
        private bool newGame = true;

        public MainWindow()
        {
            InitializeComponent();
            PrepareGame();
            timer = new Timer(this);
        }

        
        // draw game field, set buttons, resize window...
        public void PrepareGame()
        {
            Initialize();
            CreateGrid();
            CreateBtns();
            Img.Set(NewGameButton, Img.NewGame);

            SizeToContent = SizeToContent.WidthAndHeight;
        }
        

        private void OpenOptions(object sender, RoutedEventArgs e)
        {
            Options options = new Options(this);
            options.Show();
        }

        private void OpenSaver(object sender, RoutedEventArgs e)
        {
            Save save = new Save(Game.cells, timer.TimePassed());
            save.Show();
        }
        
        private void OpenLoader(object sender, RoutedEventArgs e)
        {
            Load load = new Load(this);
            load.Show();
        }
        
        private void OpenLeaderboard(object sender, RoutedEventArgs e)
        {
            Leaderboard leaderboard = new Leaderboard();
            leaderboard.Show();
        }

    }
}