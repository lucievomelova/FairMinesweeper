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
                Values.mines = 10;
                Values.width = 9;
                Values.height = 9;
            }
            else if (IntermediateOption.IsSelected)
            {
                Values.mines = 40;
                Values.width = 15;
                Values.height = 13;
            }
            else if(ExpertOption.IsSelected)
            {
                Values.mines = 99;
                Values.width = 30;
                Values.height = 16;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if(DebugModeRadioBtn.IsChecked == true)
                MainWindow.DebugMode = true;
            else
                MainWindow.DebugMode = false;

            SetDifficulty();
            
            mainWindow.PrepareGame();
            mainWindow.timer.Stop();

            Close();
        }
    }
}