using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Minesweeper
{
    public class LeadeboardRow
    {
        public int Place { get; set; }
        public string Name { get; set; }
        public int Time { get; set; }
    }
    public partial class Leaderboard : Window
    {
        public Leaderboard()
        {
            InitializeComponent();
        }
        
        
        // Called when clicking the OK button
        private void SelectDifficulty(object sender, RoutedEventArgs e)
        {
            string difficulty = DifficultyComboBox.Text; // selected difficulty
            try
            {
                DisplayLeaderboard(difficulty);
            }
            catch
            {
                MessageBox.Show("Leaderboard files were corrupted.");
            }
        }
        

        private void DisplayLeaderboard(string difficulty)
        {
            StreamReader streamReader = new StreamReader("leaderboard/" + difficulty + ".txt"); // load leaderboard from file
            var leaderboard = new ObservableCollection<LeadeboardRow>();
            int place = 1;
            string line = streamReader.ReadLine();
            while (line != null)
            {
                string[] words = line.Split(' '); // line contains name and time separated by space
                Int32.TryParse(words[1], out int time);
                leaderboard.Add(new LeadeboardRow() {Place = place, Name = words[0], Time = time});
                line = streamReader.ReadLine();
            }

            this.LeaderboardTable.ItemsSource = leaderboard;
            streamReader.Close();
        }
        
        
    }
}