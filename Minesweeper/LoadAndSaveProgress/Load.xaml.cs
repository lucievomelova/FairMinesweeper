using System;
using System.Windows;
using System.IO;

namespace Minesweeper
{
    /// <summary>
    /// Class that handles loading a previously saved game.
    /// All saved games are stored in a folder named *saved/* and contain all necessary game data.
    /// All files are stored as text files.
    /// </summary>
    public partial class Load : Window
    {
        private MainWindow mainWindow;
        
        public Load(MainWindow mainWindow)
        {
            InitializeComponent();
            DirectoryInfo d = new DirectoryInfo(@".\saved\");
            FileInfo[] Files = d.GetFiles("*.txt"); // get filenames
            foreach (var f in Files)
            {
                string name = f.Name.Substring(0, f.Name.Length - 4);
                SavedGamesComboBox.Items.Add(f.Name); 
            }

            this.mainWindow = mainWindow;
        }
        
        // Called when clicking the OK button
        private void LoadGame(object sender, RoutedEventArgs e)
        {
            string filename = SavedGamesComboBox.Text; // selected file
            if (filename == "")
                MessageBox.Show("Choose file name"); // nothing was selected
            else
            {
                LoadFromFile("saved/" + filename); // load selected game
            }
        }

        /// <summary> Load selected game </summary>
        /// <param name="filename"> Filename that was selected </param>
        private void LoadFromFile(string filename)
        {
            try
            {
                StreamReader streamReader = new StreamReader(filename);
                string line = streamReader.ReadLine(); // first line contains dimensions
                string[] words = line.Split(' ');
                Int32.TryParse(words[0], out Game.height);
                Int32.TryParse(words[1], out Game.width);
                line = streamReader.ReadLine(); // second line contains how much time passed
                Int32.TryParse(line, out int time);
                
                mainWindow.ResumeGame(time); // resume loaded game

                line = streamReader.ReadLine(); // third line contains number of mines left
                Int32.TryParse(line, out Game.unknownMinesLeft);
                line = streamReader.ReadLine(); // fourth line contains number of flags left
                Int32.TryParse(line, out Game.flagsLeft);
                mainWindow.MinesLeftLabel.Content = Game.flagsLeft;
                
                line = streamReader.ReadLine(); // number of unopened cells
                Int32.TryParse(line, out Game.unopenedLeft);
                line = streamReader.ReadLine(); // total number of mines in current game field
                Int32.TryParse(line, out Game.mines);

                // remaining lines are game field
                for (int r = 0; r < Game.height; r++)
                {
                    line = streamReader.ReadLine();
                    for (int c = 0; c < Game.width; c++)
                        Encoder.CharToCell(line[c], Game.cells[r,c]);
                }

                for (int r = 0; r < Game.height; r++)
                {
                    for (int c = 0; c < Game.width; c++)
                    {
                        Cell cell = Game.cells[r, c];
                        cell.unknownLeft = Neighbours.CountUnknown(cell);
                        if(cell.value > 0)
                            cell.minesLeft = cell.value - Neighbours.CountKnownMines(cell);
                    }
                }
                Open.OpenAfterLoad();
                Close();
                
                mainWindow.CheckWin(); // check if game isn't finished (players can save finished games too)
            
                if (mainWindow.previousGame != Game.PreviousGame.WIN)
                {
                    mainWindow.timer.SetStartTime(time);
                    mainWindow.timer.Start();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("File is corrupted. Please select another file. ");
            }
        }

    }
}