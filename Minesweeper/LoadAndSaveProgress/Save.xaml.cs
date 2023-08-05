using System.IO;
using System.Text.Json;
using System.Windows;

namespace Minesweeper
{
    /// <summary>
    /// Class that handles saving current game to a file, so that it can be opened later.
    /// Game is saved as a text file containing values of all important variables + current state of game field
    /// </summary>
    public partial class Save : Window
    {
        /// <summary> Stores the state of timer label when game was saved </summary>
        private readonly int timePassed;
        
        public Save(Cell[,] cells, int timePassed)
        {
            InitializeComponent();
            this.timePassed = timePassed;
        }
        
        // Called after clicking the OK button
        private void SaveGame(object sender, RoutedEventArgs e)
        {
            string filename = FileNameLabel.Text;
            if (filename == "")
                MessageBox.Show("Enter file name"); // no file name was entered
            else
            {
                if (!Directory.Exists("./saved"))
                    Directory.CreateDirectory("./saved");
                StreamWriter streamWriter = new StreamWriter("saved/" + filename + ".txt");
                streamWriter.Write(StateToString());
                streamWriter.Close();
                Close();
            }
        }

        /// <summary> Transform current game state into a string that will be written in a file. </summary>
        private string StateToString()
        {
            string model = "";
            model += Game.height + " " + Game.width + "\n"; // size of game field
            model += timePassed + "\n"; // how much time already passed
            
            // game variables
            model += Game.unknownMinesLeft + "\n";
            model += Game.flagsLeft + "\n";
            model += Game.unopenedLeft + "\n";
            model += Game.mines + "\n";

            // game field
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                    model += Encoder.CellToChar(Game.cells[r, c]);

                model += "\n";
            }
            return model;
        }
    }
}