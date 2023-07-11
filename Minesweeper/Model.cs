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
            solver.InitHelpArrays();
        }

        /// <summary> when there are no known unopened numbers left, player can click on any unknown cell and there has
        /// to be a number. If there wasn't originally a number, whole game field has to be generated again and this
        /// method is called </summary>
        /// <returns> true if reGeneration was successful </returns>
        public bool ReGenerate(Cell cell = null)
        {
            solver.CopyField();
            if (cell != null)
            {
                solver.helpArray[cell.row, cell.column].value = Values.NUMBER;
                solver.helpArray[cell.row, cell.column].known = true;
            }

            solver.SplitIntoSections();
            bool success = solver.FirstCombinationOnArea();
            if (success)
            {
                PlaceIntoMain();
                return true;
            }
            return false;
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
                    if(Game.cells[r,c].IsMine() || Game.cells[r,c].isOpened) continue;
                    
                    int number = Neighbours.CountMines(r, c);
                    Game.cells[r, c].value = number;
                }
            }
        }

        private void PlaceIntoMain()
        {
            foreach (Solver.helpCell h in solver.helpArray)
                Game.cells[h.row, h.column].value = h.value; //set values
            
            int minesPlaced = solver.helpArray.Cast<Solver.helpCell>().Count(h => h.value == Values.MINE);
            PlaceRemainingMines(Game.mines - minesPlaced);
            Game.minesLeft = Game.mines - solver.KnownMinesInHelpArray();
            
            SetNumbersAroundMines();
        }
    }
}