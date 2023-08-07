using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Minesweeper
{
    /// <summary> pre-solves area around open cells in current game (if possible) </summary>
    public partial class Solver
    {
        public Bruteforce bruteforce = new Bruteforce();

        /// <summary> count unopened but known numbers in game field </summary>
        public int KnownNumbers() //
        {
            return Game.cells.Cast<Cell>().Count(c => c.IsNumber() && !c.isOpened && c.isKnown);
        }

        /// <summary> find every opened nonzero number and try to update it. If a number is updated, it's neighbours
        /// are put in the queue again and UpdateCell() will be ran on them again. </summary>
        public void Update()
        {
            Queue<Cell> BorderArea = new Queue<Cell>();
            foreach (Cell c in Game.cells)
                if (c.IsNumber() && c.minesLeft != 0 && c.isOpened)
                    BorderArea.Enqueue(c);

            while (BorderArea.Count > 0)
            {
                Cell next = BorderArea.Dequeue();
                bool updated = UpdateCell(next);
                if (!updated) continue;
                
                foreach (Cell neighbour in Neighbours.Get(next))
                    if (neighbour.isOpened && neighbour.unknownLeft != 0)
                        BorderArea.Enqueue(neighbour);
            }

            bruteforce.TryAllCombinations();
            UpdateGameFieldAfterBruteforce();
        }

        /// <summary> Count known mines and unknown cells around cell. If number of known mines == cell.value or
        /// number of unknown cells == cell.value, all remaining neighbours are known.  </summary>
        /// <returns> Returns true if new known cells were found.</returns>
        public bool UpdateCell(Cell cell)
        {
            //if (cell.IsMarked()) return false;

            //true if cell will be updated
            bool updated = cell.minesLeft != cell.value - Neighbours.CountKnownMines(cell) || 
                           cell.unknownLeft != Neighbours.CountUnknown(cell);
            
            cell.minesLeft = cell.value - Neighbours.CountKnownMines(cell); //subtract number of found mines
            cell.unknownLeft = Neighbours.CountUnknown(cell); //number of empty places

            if (cell.minesLeft != cell.unknownLeft && cell.minesLeft != 0) return updated;
            
            foreach (Cell neighbour in Neighbours.Get(cell))
            {
                if (neighbour.isKnown) continue;
                
                if(Game.HighlightKnownCells && !neighbour.IsMarked())
                    neighbour.SetImage(Img.Known);
                
                neighbour.isKnown = true;
            }

            Game.unknownMinesLeft -= cell.minesLeft;
            cell.minesLeft = 0;
            cell.unknownLeft = 0;
            return updated;
        }

        /// <summary> Count all incorrectly placed flags on the game field </summary>
        public int CountIncorrectFlags()
        {
            int counter = 0;
            foreach (Cell cell in Game.cells)
            {
                if (cell.isFlag && !cell.IsMine())
                    counter++;
            }

            return counter;
        }

        /// <summary> Check all cells and if some have a longTermState that isn't UNKNOWN or NONE, update it as known</summary>
        private void UpdateGameFieldAfterBruteforce()
        {
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Cell cell = Game.cells[r, c];
                    CellState state = cell.longTermState;

                    if (state == CellState.MINE) //cell was mine in every combination
                    {
                        if (cell.IsMarked()) continue;
                        Game.unknownMinesLeft--; // another mine is known, so number of unknown mines is decremented
                        cell.isKnown = true; //mark as known
                        cell.longTermState = CellState.NONE;
                        if(Game.HighlightKnownCells && !cell.isOpened)
                            cell.SetImage(Img.Known);
                    }
                    else if (state == CellState.NUMBER) //cell was number in every combination
                    {
                        if (cell.IsMarked()) continue;
                        cell.isKnown = true; //mark as known
                        cell.longTermState = CellState.NONE;
                        if(Game.HighlightKnownCells && !cell.isOpened)
                            cell.SetImage(Img.Known);
                    }
                }
            }
        }

        public bool CheckCorrectMinePlacement()
        {
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Cell cell = Game.cells[r, c];
                    if (cell.IsNumber() && cell.value != Neighbours.CountMines(cell))
                        return false;
                }
            }
            return true;
        }
    }
}