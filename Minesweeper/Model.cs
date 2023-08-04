using System;
using System.Linq;

namespace Minesweeper
{
    /// <summary> class for generating game field </summary>
    public class Model
    {
        public Solver solver = new Solver();
        
        /// <summary> generate new game field </summary>
        public void Generate(Cell cell)
        {
            cell.value = 0;
            cell.isKnown = true;
            SetNeighboursToKnown(cell.row, cell.column);
            PlaceRemainingMines(Game.mines);
            SetNumbersAroundMines();
        }

        /// <summary> when there are no known unopened numbers left, player can click on any unknown cell and there has
        /// to be a number. If there wasn't originally a number, whole game field has to be generated again and this
        /// method is called </summary>
        /// <returns> true if reGeneration was successful </returns>
        public bool ReGenerate(Cell cell = null)
        {
            bool success = solver.bruteforce.FindCombinationOnReGeneration(cell);
            if (!success)
                return false;
            
            PlaceIntoMain();
            solver.UpdateGameField();
            success = solver.CheckCorrectMinePlacement();
            return success;
            
        }
        
        /// <summary> place remaining mines randomly in game field </summary>
        private void PlaceRemainingMines(int numberOfMines)
        {
            Random random = new Random();
            int minesPlaced = 0;
            while(minesPlaced < numberOfMines)
            {
                int r = random.Next(0, Game.height);
                int c = random.Next(0, Game.width);
                if (!Game.cells[r,c].isOpened && !Game.cells[r,c].IsMine()
                    && !Game.cells[r,c].isKnown && Game.cells[r,c].value != Values.NUMBER)
                {
                    minesPlaced++;
                    Game.cells[r,c].value = Values.MINE;
                }
            }
        }
        
        /// <summary> when all neighbours are known. Used in the beginning, because first
        /// number is always 0, so all cells around must be numbers. </summary>
        private void SetNeighboursToKnown(int row, int column)
        {
            int[] arr = {-1, 0, 1};
            foreach (int r in arr)
                foreach (int c in arr)
                    if(Values.InBounds(row+r, column+c) && !Game.cells[row+r, column+c].IsMine())
                        Game.cells[row + r, column + c].isKnown = true;              
        }

        // mines are placed, now place numbers in the game field
        private void SetNumbersAroundMines()
        {
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Cell cell = Game.cells[r, c];
                    if(cell.IsMine() || cell.isOpened) continue;
                    
                    int number = Neighbours.CountMines(cell);
                    Game.cells[r, c].value = number;
                }
            }
        }

        private void PlaceIntoMain()
        {
            Game.minesLeft = Game.mines;
            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    Cell cell = Game.cells[r, c];
                    if (cell.IsMine() && cell.isKnown)
                        Game.minesLeft--;
                    else if (cell.longTermState == CellState.MINE)
                    {
                        cell.value = Values.MINE;
                        Game.minesLeft--;
                    }
                    else if (cell.longTermState == CellState.NUMBER)
                        cell.value = Values.NUMBER;
                    else if (!cell.isKnown)
                        cell.value = Values.UNKNOWN;
                }
            }
            
            PlaceRemainingMines(Game.minesLeft);
            SetNumbersAroundMines();
        }
    }
}