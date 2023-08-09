using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper
{
    /// <summary> handles cell opening </summary>
    public class Open
    {
        public readonly Model model;
        public readonly Solver solver;
        private readonly MainWindow mainWindow;
        
        public Open(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            model = new Model();
            solver = model.solver;
        }

        public void SetCell(Cell cell, bool opened, bool flag, bool questionMark)
        {
            cell.isOpened = opened;
            if (opened) cell.isKnown = true;
            
            cell.isFlag = flag;
            cell.isQuestionMark = questionMark;

            if(cell.IsNumber() && opened)
                solver.UpdateCell(cell);

            if(flag) 
                cell.SetImage(Img.Flag);
            
            else if(questionMark)
                cell.SetImage(Img.QuestionMark);

            else if(cell.IsMine() && opened)
                OpenMine(cell);
            
            else if (opened)
                cell.SetImage(Img.Number(cell.value));
            
            else
                cell.SetImage(Img.Empty);
        }
        
        /// <summary> Open given cell </summary>
        private void OpenCell(Cell cell)
        {
            if (cell.IsMarked()) return;
            if (cell.isOpened) return;
            
            if(cell.IsNumber())
                cell.SetImage(Img.Number(cell.value));
                
            else
                cell.SetImage(Img.Flag);
                
            SetCell(cell, true, false, false);
            Game.unopenedLeft--;
        }

        public void OpenNumber(Cell cell)
        {
            OpenCell(cell);
            if (cell.value == 0) //if the number was 0, then all cells around will be opened
                OpenArea(cell);

            solver.Update(); //look for unopened but known cells around open areas
        }

        /// <summary> All cells around number cell are known and correct number of flags is around the cell. Player can
        /// click on the cell and all remaining neighbours will be opened </summary>
        public void OpenNeighbours(Cell cell)
        {
            if (Neighbours.CountFlags(cell) != cell.value) return;
            if (!Neighbours.AllAreKnown(cell)) return;
            
            foreach (Cell neighbour in Neighbours.Get(cell))
            {
                if (neighbour.IsMarked() || neighbour.isOpened) continue;
                
                if(neighbour.value == 0) OpenArea(neighbour);
                else OpenCell(neighbour);
            }
            solver.Update();
        }

        /// <summary> open area of cells (when player clicks on number 0, the area around will open too) </summary>
        public void OpenArea(Cell cell)
        {
            Queue<Cell> neighbourArea = new Queue<Cell>();
            neighbourArea.Enqueue(cell);       

            while (neighbourArea.Count > 0)
            {
                Cell current = neighbourArea.Dequeue();
                if (current.value == 0)
                {
                    foreach (Cell next in Neighbours.Get(current).Where(next => !next.isOpened && !next.IsMarked()))
                        neighbourArea.Enqueue(next);
                }
                OpenCell(current);
            }
            solver.Update();
        }

        /// <summary> When clicked cell is a mine, game ends </summary>
        public void OpenMine(Cell cell)
        {
            mainWindow.timer.Stop();
            mainWindow.previousGame = Game.PreviousGame.LOSE;
            
            Img.Set(mainWindow.NewGameButton, Img.GameOver);
            if (cell != null)
            {
                cell.SetImage(Img.MineClicked);
                mainWindow.DisableButton(cell.btn);
                cell.value = Values.MINE_CLICKED;
            }

            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    cell = Game.cells[r, c];
                    if (cell.IsMine() && cell.isKnown)
                        cell.SetImage(Img.Mine);
                    
                    else if(cell.isFlag)
                        cell.SetImage(Img.WrongMine);

                    mainWindow.DisableButton(cell.btn);
                }
            }
        }

        //user won, open whole field
        public void OpenEverything()
        {
            foreach (Cell cell in Game.cells)
            {
                if(cell.value >= 0)
                    cell.SetImage(Img.Number(cell.value));
                else
                    cell.SetImage(Img.Flag);
            }
        }

        /// <summary> Called when saved game is loaded from a file to opened cells that were opened before saving </summary>
        public static void OpenAfterLoad()
        {
            foreach (Cell cell in Game.cells)
            {
                if(cell.isOpened)
                    cell.SetImage(Img.Number(cell.value));
                else if(cell.isFlag)
                    cell.SetImage(Img.Flag);
                else if(cell.isQuestionMark)
                    cell.SetImage(Img.QuestionMark);
                else 
                    Img.UpdateUnopened(cell);
            }
        }
    }
}