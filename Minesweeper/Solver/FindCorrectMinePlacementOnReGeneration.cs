using System.Threading;

using Area = System.Collections.Generic.List<Minesweeper.Cell>;

namespace Minesweeper
{
    // this file contains methods used when ReGenerate() is called
    
    public partial class Bruteforce
    {
        /// <summary> Find one correct mine placement around open cells </summary>
        /// <param name="clickedCell"> Cell that is being opened now - it is definitely a number </param>
        /// <returns> True if a correct combination exists, otherwise false </returns>
        public bool FindCombinationOnReGeneration(Cell clickedCell)
        {
            Init();
            var unresolvedCells = FindUnresolvedOpenCells();
            clickedCell.longTermState = CellState.NUMBER;
            if (unresolvedCells.Contains(clickedCell))
                unresolvedCells.Remove(clickedCell);
            
            var unresolvedAreas = SplitAreaIntoNonneighboringAreas(unresolvedCells);
            if (unresolvedAreas == null)
                return true;

            foreach (Area area in unresolvedAreas)
            foreach (Cell cell in area)
                cell.longTermState = CellState.NONE;
            
            for (int i = 0; i < unresolvedAreas.Count; i++)
            {
                int threadNum = i % numberOfThreads; // default thread number
                for (int t = 0; t < numberOfThreads; t++)
                {
                    if (threads[t] == null || !threads[t].IsAlive) // thread that is not doing anything now
                    {
                        threadNum = t;
                        break;
                    }
                }

                if (threadNum == i % numberOfThreads)
                    if(threads[threadNum] != null && threads[threadNum].ThreadState == ThreadState.Running)
                        threads[threadNum].Join();
                
                var area = unresolvedAreas[i];
                threads[threadNum] = new Thread(() => FindOneCombinationOnArea(area)); 
                threads[threadNum].Start();
            }
            
            for (int t = 0; t < numberOfThreads; t++)
                if (threads[t] != null && threads[t].ThreadState == ThreadState.Running)
                    threads[t].Join();
            return true;
        }

        /// <summary> Method called by additional threads, that tries to find a correct mine placement for given area </summary>
        /// <param name="openedArea"></param>
        private void FindOneCombinationOnArea(Area openedArea)
        {
            Area neighbourArea = GetUnknownNeighbours(openedArea);
            bool stop = false;
            FindCombination(openedArea, neighbourArea, 0, Game.unknownMinesLeft, ref stop);
        }

        /// <summary> Method that is called recursively, that tries to find a correct mine placement for given area  </summary>
        /// <param name="openedArea"> Area of opened cells that have unknown neighbors </param>
        /// <param name="neighbourArea"> Unknown neighbors </param>
        /// <param name="index"> Index of the current neighbor cell (because the method is called recursively, in each
        /// call the value of a different cell is set. </param>
        /// <param name="minesLeft"> Number of mines that can be placed on the unopened part of the game field </param>
        /// <param name="stop"> Indicates whether a correct combination was found already </param>
        private void FindCombination(Area openedArea, Area neighbourArea, int index, int minesLeft, ref bool stop)
        {
            if (index == neighbourArea.Count)
            {
                bool correct = CorrectMinePlacement(openedArea); 
                if (correct)
                    stop = true; // correct placement was found
                return;
            }
            
            Cell cell = neighbourArea[index];

            // check if mine can be placed on this position
            bool possibleMine = true;
            foreach (Cell neighbour in Neighbours.Get(cell))
                if (neighbour.isOpened && CountPlacedMines(neighbour) >= neighbour.value)
                    possibleMine = false;

            if (!stop)
            {
                if (possibleMine && minesLeft > 0) // try to place mine here if possible
                {
                    neighbourArea[index].longTermState = CellState.MINE;
                    neighbourArea[index].currentState = CellState.MINE;
                    FindCombination(openedArea, neighbourArea, index + 1, minesLeft - 1, ref stop);
                }
            }
            if (!stop)
            {
                neighbourArea[index].longTermState = CellState.NUMBER;
                neighbourArea[index].currentState = CellState.NUMBER;
                FindCombination(openedArea, neighbourArea, index + 1, minesLeft, ref stop);
            }
        }
    }
}