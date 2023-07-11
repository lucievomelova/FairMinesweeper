using System.IO;
using System.Text.Json;
using System.Windows;

namespace Minesweeper
{
    public partial class Save : Window
    {
        private Cell[,] cells;
        private int timePassed;
        public Save(Cell[,] cells, int timePassed)
        {
            InitializeComponent();
            this.cells = cells;
            this.timePassed = timePassed;
        }
        
        private void SaveGame(object sender, RoutedEventArgs e)
        {
            string filename = FileNameLabel.Text;
            if (filename == "")
                MessageBox.Show("Enter file name");
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

        private string StateToString()
        {
            string model = "";
            model += Game.height + " " + Game.width + "\n"; // size of game field
            model += timePassed + "\n"; // how much time already passed
            model += Game.minesLeft + "\n";
            model += Game.flagsLeft + "\n";
            model += Game.unopenedLeft + "\n";
            model += Game.mines + "\n";


            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Cell cell = Game.cells[r, c];
                    model += Encoder.CellToChar(cell);
                }

                model += "\n";
            }

            return model;
        }

        
    }
}