using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Minesweeper
{
    public partial class Win : Window
    {
        private int time;
        
        public Win(int time)
        {
            InitializeComponent();
            this.time = time;
        }

        private string GameDifficultyToString()
        {
            switch (Game.difficulty)
            {
                case Difficulty.Beginner:
                    return "Beginner";
                case Difficulty.Intermediate:
                    return "Intermediate";
                case Difficulty.Expert:
                    return "Expert";
            }

            return "";
        }
        
        // Called when clicking the OK button
        private void SaveToLeaderboard(object sender, RoutedEventArgs e)
        {
            string lineToBeAdded = NameTextBox.Text + " " + time.ToString();
            string filename = "leaderboard/" + GameDifficultyToString() + ".txt";
            StreamReader streamReader = new StreamReader(filename); // load leaderboard from file

            string newFileContent = "";
            string line = streamReader.ReadLine();

            bool newLinePlaced = false;
            
            while (line != null)
            {
                string[] words = line.Split(' '); // line contains name and time separated by space
                Int32.TryParse(words[1], out int t);

                if (!newLinePlaced && t > time)
                {
                    newFileContent += lineToBeAdded + "\n";
                    newLinePlaced = true;
                }
                newFileContent += line + "\n";
                line = streamReader.ReadLine();
            }
            
            if (!newLinePlaced)
            {
                newFileContent += lineToBeAdded + "\n";
            }
            
            streamReader.Close();
            StreamWriter streamWriter = new StreamWriter(filename);
            streamWriter.Write(newFileContent);
            streamWriter.Close();
            Close();
        }
    }
}