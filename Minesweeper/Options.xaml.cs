using System.Windows;

namespace Minesweeper
{
    public partial class Options : Window
    {
        private MainWindow mainWindow;
        public Options(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            DebugModeRadioBtn.IsChecked = true;
        }

        private void SetDifficulty()
        {
            if (BeginnerOption.IsSelected)
            {
                Game.mines = 10;
                Game.width = 9;
                Game.height = 9;
            }
            else if (IntermediateOption.IsSelected)
            {
                Game.mines = 40;
                Game.width = 15;
                Game.height = 13;
            }
            else if(ExpertOption.IsSelected)
            {
                Game.mines = 99;
                Game.width = 30;
                Game.height = 16;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if(DebugModeRadioBtn.IsChecked == true)
                Game.DebugMode = true;
            else
                Game.DebugMode = false;

            SetDifficulty();
            
            mainWindow.PrepareGame();
            mainWindow.timer.Stop();

            Close();
        }
    }
}