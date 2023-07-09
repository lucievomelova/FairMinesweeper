using System;
using System.Windows;
using System.IO;

namespace Minesweeper
{
    public partial class Load : Window
    {
        private int rows;
        private int columns;
        private int time;
        
        public Load()
        {
            InitializeComponent();
            DirectoryInfo d = new DirectoryInfo(@".\saved\");
            FileInfo[] Files = d.GetFiles("*.txt");
            foreach (var f in Files)
            {
                string name = f.Name.Substring(0, f.Name.Length - 4);
                SavedGamesComboBox.Items.Add(f.Name);
            }
        }
        
        private void LoadGame(object sender, RoutedEventArgs e)
        {
            string filename = SavedGamesComboBox.Text;
            if (filename == "")
                MessageBox.Show("Choose file name");
            else
            {
                Close();
            }
        }

        private void LoadFromFile(string filename)
        {
            try
            {
                StreamReader streamReader = new StreamReader(filename);
                string line = streamReader.ReadLine(); // first line contains dimensions
                string[] words = line.Split(' ');
                Int32.TryParse(words[0], out rows);
                Int32.TryParse(words[1], out columns);

                line = streamReader.ReadLine(); // second line contains how much time passed
                Int32.TryParse(line, out time);
                string content = streamReader.ReadToEnd(); // remaining lines are game field
                GenerateGameFromString(content);
                Close();
            }
            catch
            {
                MessageBox.Show("Error when reading file.");
            }
        }

        private void GenerateGameFromString(string savedGame)
        {
            int row = 0;
            int column = 0;
            Cell[,] cells = new Cell[rows, columns];
            foreach(char c in savedGame)
            {
                Cell cell = Encoder.CharToCell(c, row, column);
                column++;
                if (c == '\n')
                {
                    row++;
                    column = 0;
                }
            }

            MainWindow.cells = cells;
        }

    }
}