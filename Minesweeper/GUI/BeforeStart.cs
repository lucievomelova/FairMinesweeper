using System.IO;
using System.Windows.Controls;

namespace Minesweeper
{
    public partial class MainWindow
    {
        /// <summary> set variables for current game </summary>
        private void Initialize()
        {
            open = new Open(this);
            model = open.model;
            solver = open.solver;
            newGame = true;
            Game.cells = new Cell[Game.height, Game.width];

            MinesLeftLabel.Content = Game.mines;
            Game.flagsLeft = Game.mines;
            Game.unknownMinesLeft = Game.mines;
            Game.unopenedLeft = Game.width * Game.height;
            
            
            if (!Directory.Exists("./leaderboard"))
                Directory.CreateDirectory("./leaderboard");
            if (!File.Exists("./leaderboard/Beginner.txt"))
                File.Create("./leaderboard/Beginner.txt");
            if (!File.Exists("./leaderboard/Intermediate.txt"))
                File.Create("./leaderboard/Intermediate.txt");
            if (!File.Exists("./leaderboard/Expert.txt"))
                File.Create("./leaderboard/Expert.txt");
        }

        /// <summary> create grid where each game button will be placed </summary>
        private void CreateGrid()
        {
            //delete all rows and columns
            grid.RowDefinitions.Clear(); 
            grid.ColumnDefinitions.Clear();

            grid.Width = Game.width * Values.cellSize;
            grid.Height = Game.height * Values.cellSize;

            //create columns
            for (int i = 0; i < Game.width; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            //create rows
            for (int i = 0; i < Game.height; i++)
                grid.RowDefinitions.Add(new RowDefinition());
        }

        /// <summary> create all game buttons </summary>
        private void CreateBtns()
        {
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Game.cells[r,c] = new Cell(r, c);
                    Button btn = Game.cells[r,c].btn;
                    btn.MouseRightButtonUp += RightClick;
                    btn.MouseEnter += BtnMouseEnter;
                    btn.MouseLeave += BtnMouseLeave;
                    btn.PreviewMouseLeftButtonDown += BtnMouseDown;
                    btn.PreviewMouseLeftButtonUp += BtnMouseUp;
                    Game.cells[r,c].SetImage(Img.Empty);
                    
                    //place buttons in grid
                    Grid.SetRow(btn, r);
                    Grid.SetColumn(btn, c);
                    grid.Children.Add(btn);
                }
            }
        }

    }
}