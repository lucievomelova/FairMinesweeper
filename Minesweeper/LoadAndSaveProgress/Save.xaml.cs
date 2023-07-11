﻿using System.IO;
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
            model += Values.height + " " + Values.width + "\n"; // size of game field
            model += timePassed + "\n"; // how much time already passed
            model += Values.minesLeft + "\n";
            model += Values.flagsLeft + "\n";
            model += Values.unopenedLeft + "\n";
            model += Values.mines + "\n";


            for (int r = 0; r < Values.height; r++)
            {
                for (int c = 0; c < Values.width; c++)
                {
                    Cell cell = MainWindow.cells[r, c];
                    model += Encoder.CellToChar(cell);
                }

                model += "\n";
            }

            return model;
        }

        
    }
}