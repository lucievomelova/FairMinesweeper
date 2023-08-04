using System.Windows.Controls;

namespace Minesweeper
{
    public partial class MainWindow
    {
        
        /// <summary> place or delete a flag or a question mark </summary>
        private void PlaceFlag(Cell cell)
        {
            //if the button is empty and the mine counter isn't on 0, place a flag
            if (!cell.isOpened && !cell.IsMarked() && Game.flagsLeft > 0)
            {
                MinesLeftLabel.Content = --Game.flagsLeft; //decrement the flag counter if flag was placed
                open.SetCell(cell, false, true, false);
            }

            //if there is a flag already placed, delete it and place a question mark 
            else if (cell.isFlag)
            {
                open.SetCell(cell, false, false, true);
                MinesLeftLabel.Content = ++Game.flagsLeft; //increment mine counter
            }
            else if(cell.isQuestionMark) //delete question mark
            {
                open.SetCell(cell, false, false, false);
            }
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
            Cell cell = Game.FindCell(btn); //mouse cursor is over this cell
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
            
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Cell x = Game.cells[r, c];
                    if(x.IsMine() && x.isKnown && !x.IsMarked())
                        x.SetImage(Img.Purple);
                    if(x.IsMine() && !x.isKnown )
                        x.SetImage(Img.Blue);
                }
            }
            
            CheckWin();
            
        }
        
        
    }
}