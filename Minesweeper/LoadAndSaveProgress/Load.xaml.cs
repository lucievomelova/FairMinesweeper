using System;
using System.Windows;
using System.IO;

namespace Minesweeper
{
    public partial class Load : Window
    {
        private int time;
        private MainWindow mainWindow;
        
        public Load(MainWindow mainWindow)
        {
            InitializeComponent();
            DirectoryInfo d = new DirectoryInfo(@".\saved\");
            FileInfo[] Files = d.GetFiles("*.txt");
            foreach (var f in Files)
            {
                string name = f.Name.Substring(0, f.Name.Length - 4);
                SavedGamesComboBox.Items.Add(f.Name);
            }

            this.mainWindow = mainWindow;
        }
        
        private void LoadGame(object sender, RoutedEventArgs e)
        {
            string filename = SavedGamesComboBox.Text;
            if (filename == "")
                MessageBox.Show("Choose file name");
            else
            {
                LoadFromFile("saved/" + filename);
            }
        }

        private void LoadFromFile(string filename)
        {
            try
            {
                StreamReader streamReader = new StreamReader(filename);
                string line = streamReader.ReadLine(); // first line contains dimensions
                string[] words = line.Split(' ');
                Int32.TryParse(words[0], out Game.height);
                Int32.TryParse(words[1], out Game.width);
                
                mainWindow.ResumeGame(time);


                line = streamReader.ReadLine(); // second line contains how much time passed
                Int32.TryParse(line, out time);
                line = streamReader.ReadLine(); // third line contains number of mines left
                Int32.TryParse(line, out Game.minesLeft);
                line = streamReader.ReadLine(); // fourth line contains number of flags left
                Int32.TryParse(line, out Game.flagsLeft);
                line = streamReader.ReadLine(); // number of unopened cells
                Int32.TryParse(line, out Game.unopenedLeft);
                line = streamReader.ReadLine(); // total number of mines in current game field
                Int32.TryParse(line, out Game.mines);

                // remaining lines are game field
                for (int r = 0; r < Game.height; r++)
                {
                    line = streamReader.ReadLine();
                    for (int c = 0; c < Game.width; c++)
                    {
                        Encoder.CharToCell(line[c], Game.cells[r,c]);
                    }
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
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}