using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;

namespace Minesweeper
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow
    {
        public static Cell[,] cells;
        private Open open;
        private Model model;
        private Solver solver;
        public readonly Timer timer;

        /// <summary> State of previous game - if it was lost or won or if it wasn't neither of those two </summary>
        public enum PreviousGame
        {
            NORMAL, //last game was ended before player won or lost (this is also the state after app start)
            WIN, //player won last game
            LOSE //player lost last game
        }
        public PreviousGame previousGame = PreviousGame.NORMAL;

        /// <summary> true if current game wasn't started already (game field is empty, no buttons are opened) </summary>
        private bool newGame = true;

        /// <summary> true if Debug Mode is on. In Debug Mode, known cells are highlighted </summary>
        public static bool DebugMode = true;
        
        public MainWindow()
        {
            InitializeComponent();
            PrepareGame();
            timer = new Timer(this);
        }


        #region BeforeStart

        /// <summary> set variables for current game </summary>
        private void Initialize()
        {
            open = new Open(this);
            model = open.model;
            solver = open.solver;
            newGame = true;
            cells = new Cell[Values.height, Values.width];

            MinesLeftLabel.Content = Values.mines;
            Values.flagsLeft = Values.mines;
            Values.minesLeft = Values.mines;
            Values.unopenedLeft = Values.width * Values.height;
        }

        /// <summary> create grid where each game button will be placed </summary>
        private void CreateGrid()
        {
            //delete all rows and columns
            grid.RowDefinitions.Clear(); 
            grid.ColumnDefinitions.Clear();

            grid.Width = Values.width * Values.cellSize;
            grid.Height = Values.height * Values.cellSize;

            //create columns
            for (int i = 0; i < Values.width; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            //create rows
            for (int i = 0; i < Values.height; i++)
                grid.RowDefinitions.Add(new RowDefinition());
        }

        /// <summary> create all game buttons </summary>
        private void CreateBtns()
        {
            for (int r = 0; r < Values.height; r++)
            {
                for (int c = 0; c < Values.width; c++)
                {
                    cells[r,c] = new Cell(r, c);
                    Button btn = cells[r,c].btn;
                    btn.MouseRightButtonUp += RightClick;
                    btn.MouseEnter += BtnMouseEnter;
                    btn.MouseLeave += BtnMouseLeave;
                    btn.PreviewMouseLeftButtonDown += BtnMouseDown;
                    btn.PreviewMouseLeftButtonUp += BtnMouseUp;
                    cells[r,c].SetImage(Img.Empty);
                    
                    //place buttons in grid
                    Grid.SetRow(btn, r);
                    Grid.SetColumn(btn, c);
                    grid.Children.Add(btn);
                }
            }
        }

        // draw game field, set buttons, resize window...
        public void PrepareGame()
        {
            Initialize();
            CreateGrid();
            CreateBtns();
            Img.Set(NewGameButton, Img.NewGame);

            SizeToContent = SizeToContent.WidthAndHeight;
        }

        #endregion

        #region NewGameBtn

        private void NewGameClick(object sender, RoutedEventArgs e)
        {
            PrepareGame();
            timer.Stop();
            previousGame = PreviousGame.NORMAL;
            TimeLabel.Content = "000";
        }

        private void NewGameMouseEnter(object sender, MouseEventArgs e)
        {
            switch (previousGame)
            {
                case PreviousGame.NORMAL:
                    Img.Set(NewGameButton, Img.NewGameMouseOver);
                    break;
                case PreviousGame.WIN:
                    Img.Set(NewGameButton, Img.WinMouseOver);
                    break;
                case PreviousGame.LOSE:
                    Img.Set(NewGameButton, Img.GameOverMouseOver);
                    break;
                default:
                    Img.Set(NewGameButton, Img.NewGameMouseOver);
                    break;
            }
        }

        private void NewGameMouseLeave(object sender, MouseEventArgs e)
        {
            switch (previousGame)
            {
                case PreviousGame.NORMAL:
                    Img.Set(NewGameButton, Img.NewGame);
                    break;
                case PreviousGame.WIN:
                    Img.Set(NewGameButton, Img.Win);
                    break;
                case PreviousGame.LOSE:
                    Img.Set(NewGameButton, Img.GameOver);
                    break;
                default:
                    Img.Set(NewGameButton, Img.NewGame);
                    break;
            }
        }

        #endregion

        #region MouseEventsOnGrid

        /// <summary> left mouse button is pressed down </summary>
        private void BtnMouseDown(object sender, MouseEventArgs e)
        {
            BtnMouseEnter(sender, e);
            Cell cell = FindCell((Button) sender); //mouse cursor is over this cell
            if(cell.value > 0 && cell.value - Neighbours.CountMarkedMines(cell) == 0 && Neighbours.AllAreKnown(cell))
                Neighbours.PressDown(cell);
        }

        /// <summary> left mouse button is released </summary>
        private void BtnMouseUp(object sender, MouseEventArgs e)
        {
            OpenBtn((Button)sender); 
            if(previousGame != PreviousGame.LOSE)
                BtnMouseLeave(sender, e);
            
            Cell cell = FindCell((Button) sender); //mouse cursor is over this cell
            if(cell.value > 0 && cell.value - Neighbours.CountMarkedMines(cell) == 0 && Neighbours.AllAreKnown(cell))
                Neighbours.ReleaseUp(cell);
        }

        private void BtnMouseEnter(object sender, MouseEventArgs e)
        {
            Cell cell = FindCell((Button) sender);
            if(!cell.isOpened && !cell.IsMarked())
                cell.SetImage(Img.EmptyMouseOver);
        }

        private void BtnMouseLeave(object sender, MouseEventArgs e)
        {
            Cell cell = FindCell((Button) sender);
            if (!cell.isOpened && !cell.IsMarked())
            {
                if(cell.isKnown && DebugMode)
                    cell.SetImage(Img.Known);
                else
                    cell.SetImage(Img.Empty);
            }
        }
        
        
        /// <summary> right mouse button is clicked </summary>
        private void RightClick(object sender, RoutedEventArgs e)
        {
            Cell cell = FindCell((Button) sender);
            Flag(cell);
            CheckWin();
        }

        #endregion

        #region GameField
        
        /// <summary> place or delete a flag or a question mark </summary>
        private void Flag(Cell cell)
        {
            //if the button is empty and the mine counter isn't on 0, place a flag
            if (!cell.isOpened && !cell.IsMarked() && Values.flagsLeft > 0)
            {
                MinesLeftLabel.Content = --Values.flagsLeft; //decrement the flag counter if flag was placed
                open.SetCell(cell, false, true, false);
            }

            //if there is a flag already placed, delete it and place a question mark 
            else if (cell.isFlag)
            {
                open.SetCell(cell, false, false, true);
                MinesLeftLabel.Content = ++Values.flagsLeft; //increment mine counter
            }
            else if(cell.isQuestionMark) //delete question mark
            {
                open.SetCell(cell, false, false, false);
            }
        }
        
        private void StartNewGame(Cell cell)
        {
            newGame = false;
            timer.Start();
            model.Generate(cell); //generate new game field
            open.OpenArea(cell); //open clicked cell (there will always be number 0, so area will be opened)
        }

        /// <summary> if unknown cell is opened - determine if there are any known and unopened numbers left. If no,
        /// then this opened cell has to be a number. Otherwise it has to be a mine. </summary>
        private void UnknownCellOpened(Cell cell)
        {
            if (solver.KnownNumbers() == 0) //no unopened known numbers exist
            {
                if (cell.IsMine()) //clicked cell was originally a mine
                {
                    bool success = model.ReGenerate(cell); //try to generate game field to fit already open buttons
                    if (success)
                        open.OpenNumber(cell);

                    else
                        open.OpenMine(cell);
                }
                else
                    open.OpenNumber(cell);
            }
            else //unopened and known number exists - this means automatic loss
                open.OpenMine(cell);
        }
        
        private void OpenBtn(Button btn)
        {
            Cell cell = FindCell(btn); //mouse cursor is over this cell
            if (cell.IsMarked()) return; //marked cell cannot be opened
            
            if (newGame) //new game will be started with this click
                StartNewGame(cell);

            else if (!cell.isKnown) //player clicked on unknown cell
                UnknownCellOpened(cell);

            else
            {
                if (!cell.isOpened && !cell.IsMarked()) //open unopened cell that isn't marked
                    open.OpenNumber(cell);
                
                else if (cell.isOpened) //cell is already opened - try to open neighbours
                    open.OpenNeighbours(cell);
            }
            CheckWin();
        }
        
        /// <summary> find cell which is represented by button *btn* </summary>
        private Cell FindCell(Button btn)
        {
            for (int r = 0; r < Values.height; r++)
            for (int c = 0; c < Values.width; c++)
                if (cells[r, c].btn == btn) return cells[r, c];
            
            return null;
        }


        #endregion
        
        #region GameEnd
        
        /// <summary> check if player has already won the game - either all mines are marked with a flag or the number of
        /// unopened cells is equal to number of mines in game </summary>
        private void CheckWin()
        {
            if ((solver.CountIncorrectFlags() == 0 && Values.flagsLeft == 0 && Values.minesLeft == 0) ||
                Values.unopenedLeft == Values.mines)
                Win();
        }

        private void Win()
        {
            timer.Stop();
            open.OpenEverything();
            previousGame = PreviousGame.WIN;
            foreach (Cell c in cells)
            {
                DisableButton(c.btn);
                if (c.IsMine())
                    c.SetImage(Img.Flag);
            }
            MinesLeftLabel.Content = 0;
            Img.Set(NewGameButton, Img.Win);
            MessageBox.Show("Good job!");
        }

        public void DisableButton(Button btn)
        {
            btn.MouseRightButtonUp -= RightClick;
            btn.MouseEnter -= BtnMouseEnter;
            btn.MouseLeave -= BtnMouseLeave;
            btn.PreviewMouseLeftButtonDown -= BtnMouseDown;
            btn.PreviewMouseLeftButtonUp -= BtnMouseUp;
        }
        
        #endregion


        public void ResumeGame()
        {
            PrepareGame();
            timer.Stop();
            previousGame = PreviousGame.NORMAL;
            newGame = false;
            model.placeMines.InitHelpArrays();
        }

        private void OpenOptions(object sender, RoutedEventArgs e)
        {
            Options options = new Options(this);
            options.Show();
        }

        private void OpenSaver(object sender, RoutedEventArgs e)
        {
            Save save = new Save(cells, timer.TimePassed());
            save.Show();
        }
        
        private void OpenLoader(object sender, RoutedEventArgs e)
        {
            Load load = new Load(this);
            load.Show();
        }

    }
}