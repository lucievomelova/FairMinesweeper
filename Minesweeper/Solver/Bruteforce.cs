using System;
using System.Collections.Generic;
using System.Threading;

using Area = System.Collections.Generic.List<Minesweeper.Cell>;

namespace Minesweeper
{
    public partial class Bruteforce
    {
        /// <summary> Array of additional threads used for bruteforce </summary>
        private Thread[] threads;
        
        /// <summary> Number of additional threads (minimum is one) </summary>
        private int numberOfThreads;
        
        /// <summary> List of areas that the bruteforce algorithm was used on last time. This List ensures that if
        /// an area didn't change from last time, the computation won't be done again, because it would have the same
        /// result as the previous computation. </summary>
        private List<Area> previousBorderAreas;
        
        /// <summary> Initialize additional threads </summary>
        private void Init()
        {
            numberOfThreads = Environment.ProcessorCount - 1;
            if (numberOfThreads == 0)
                numberOfThreads = 1;
            threads = new Thread[numberOfThreads];
        }

        /// <summary> Find all cells that are open and have unknown neighbors </summary>
        private static Area FindUnresolvedOpenCells()
        {
            Area area = new Area();
            foreach(Cell cell in Game.cells)
                if (cell.isOpened && cell.unknownLeft > 0 && cell.value > 0)
                     area.Add(cell);
            return area;
        }

        /// <summary> Split one area into smaller areas that are independent of each other </summary>
        /// <param name="wholeArea"> Input area</param>
        /// <returns> List of independent areas </returns>
        private static List<Area> SplitAreaIntoNonneighboringAreas(Area wholeArea)
        {
            List<Area> splitAreas = new List<Area>();
            
            if (wholeArea.Count == 0)
                 return null;
             
             int counter = 0; // counter for how many areas are there
             
             // init
             splitAreas.Add(new Area());
             splitAreas[0].Add(wholeArea[0]);
             for (int i = 1; i < wholeArea.Count; i++)
             {
                 var currentCell = wholeArea[i];
                 bool sameAsCurrentArea = false;
                 // check if current cell is close enough to any cell in current openedArea
                 foreach (Cell previousCell in splitAreas[counter])
                 {
                     if (Math.Abs(currentCell.row - previousCell.row) <= 2 && Math.Abs(currentCell.column - previousCell.column) <= 2)
                     {
                         sameAsCurrentArea = true;
                         break;
                     }
                 }

                 if (!sameAsCurrentArea)
                 {
                     counter++; // it is not close enough to any cell in previous openedArea -> create new openedArea for it
                     splitAreas.Add(new Area());
                 }
                 splitAreas[counter].Add(currentCell);
             }

             // now merge areas that were at first classified as independent but arent in fact independent 
             for(int a = 0; a < splitAreas.Count; a++)
             {
                 var currentArea = splitAreas[a];
                 foreach (Cell currentCell in currentArea)
                 {
                     for (int i = 0; i < a; i++)
                     {
                         var previousArea = splitAreas[i];
                         for (int j = 0; j < previousArea.Count; j++)
                         {
                             Cell previousCell = previousArea[j];
                             if (Math.Abs(currentCell.row - previousCell.row) <= 2 && Math.Abs(currentCell.column - previousCell.column) <= 2)
                             {
                                 previousArea.AddRange(currentArea);
                                 splitAreas.RemoveAt(a);
                                 a--;
                                 goto skip;
                             }
                         }
                     }
                 }
                 skip:;
             }

             return splitAreas;
        }

        /// <summary> Check list of new independent areas and compare it to areas from previous computations. If there
        /// are some areas that are the same, this area will be removed from new list, because the same computation was
        /// already done and it would have the same outcome. </summary>
        /// <param name="unresolvedAreas"> List of areas to be compared with saved list. If both lists share an
        /// element, this element will be removed from this list </param>
        private void RemoveSameAsPreviousAreas(List<Area> unresolvedAreas)
        {
            if (previousBorderAreas == null || unresolvedAreas == null)
                return;
            foreach (var prev in previousBorderAreas)
            {
                for (int i = 0; i < unresolvedAreas.Count; i++)
                {
                    var current = unresolvedAreas[i];
                    if (prev.Equals(current))
                    {
                        unresolvedAreas.RemoveAt(i);
                        i--;
                    }
                }
            }
        }


        /// <summary> Set cell states of all given cells to default </summary>
        private void ResetCellStates(List<Area> unresolvedAreas)
        {
            foreach (Area area in unresolvedAreas)
            foreach (Cell cell in area)
                cell.longTermState = CellState.NONE;
            
        }


        /// <summary> Try all combinations of possible mine placements. If a cell has the same value for all possible
        /// combination, then this cell must have that value. This method finds those cells. </summary>
        public void TryAllCombinations()
        {
            Init();
            var unresolvedCells = FindUnresolvedOpenCells(); // find cells to work with
            var unresolvedAreas = SplitAreaIntoNonneighboringAreas(unresolvedCells); // split into areas
            RemoveSameAsPreviousAreas(unresolvedAreas); // remove already solved areas
            
            if (unresolvedAreas == null)
                return;
            
            ResetCellStates(unresolvedAreas);
            
            // run bruteforce in threads - one thread takes care of one area at a time
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
                threads[threadNum] = new Thread(() => TryAllCombinationsOnArea(area)); 
                threads[threadNum].Start();
            }
            
            for (int t = 0; t < numberOfThreads; t++) //wait for all threads to finish
                if (threads[t] != null && threads[t].ThreadState == ThreadState.Running)
                    threads[t].Join();

            // store current unresolved areas
            unresolvedCells = FindUnresolvedOpenCells();
            previousBorderAreas = SplitAreaIntoNonneighboringAreas(unresolvedCells);
        }

        /// <summary> Method that is called from additional threads, that calls the bruteforce algorithm </summary>
        private void TryAllCombinationsOnArea(Area openedArea)
        {
            Area neighbourArea = GetUnknownNeighbours(openedArea);
            FindAllCombinations(openedArea, neighbourArea, 0, Game.unknownMinesLeft);
        }

        /// <summary> Try all combinations of mine placement on given part of the game field. THis method calls itself
        /// recursively until all possible combinations are tried </summary>
        /// <param name="openedArea"> Area of opened cells that have unknown neighbors </param>
        /// <param name="neighbourArea"> Unknown neighbors </param>
        /// <param name="index"> Index of the current neighbor cell (because the method is called recursively, in each
        /// call the value of a different cell is set. </param>
        /// <param name="minesLeft"> Number of mines that can be placed on the unopened part of the game field </param>
        private void FindAllCombinations(Area openedArea, Area neighbourArea, int index, int minesLeft)
        {
            // bottom of recursion
            if (index == neighbourArea.Count)
            {
                bool correct = CorrectMinePlacement(openedArea);
                if (correct) // save current combination if all opened cells have the correct number of mines around them
                    SaveCurrentCombination(neighbourArea);
                return;
            }

            Cell cell = neighbourArea[index];

            // check if mine can be placed on this position
            bool possibleMine = true;
            foreach (Cell neighbour in Neighbours.Get(cell))
            {
                if (neighbour.isOpened && CountPlacedMines(neighbour) >= neighbour.value)
                    possibleMine = false;
            }

            if (possibleMine && minesLeft > 0) // try to place mine here if possible
            {
                cell.currentState = CellState.MINE;
                FindAllCombinations(openedArea, neighbourArea, index + 1, minesLeft-1);
            }

            cell.currentState = CellState.NUMBER; // try to place number
            FindAllCombinations(openedArea, neighbourArea, index + 1, minesLeft);
        }

        /// <summary> Find all unknown neighbors of given area of cells</summary>
        /// <param name="area"> Opened cells whose neighbors we are checking </param>
        /// <returns> Unknown neighbors </returns>
        private Area GetUnknownNeighbours(Area area)
        {
            Area neighbourArea = new Area();
            foreach (Cell cell in area)
            {
                var cellNeighbours = Neighbours.Get(cell);
                foreach (Cell neigbour in cellNeighbours)
                {
                    if(!neigbour.isKnown && !neigbour.isOpened && !neighbourArea.Contains(neigbour) && neigbour.value != Values.NUMBER)
                        neighbourArea.Add(neigbour);
                }
            }

            return neighbourArea;
        }
        

        /// <summary> Save current mine placement on areas that are being computed. This method uses cells longTermState.
        /// When currentState is the same as longTermState, it is possible that this cell will have this value (state).
        /// But if they differ (MINE x NUMBER), then we can't figure out which value this cell will have (so it will
        /// be UNKNOWN). </summary>
        /// <param name="area"> Area of cells that we want to save </param>
        private void SaveCurrentCombination(Area area)
        {
            foreach (Cell cell in area)
            {
                if (cell.longTermState == CellState.NONE)
                    cell.longTermState = cell.currentState;
                else if (cell.longTermState != cell.currentState)
                    cell.longTermState = CellState.UNKNOWN;
            }
        }


        /// <summary> Check whether each cell in given area has the correct number of cells around </summary>
        /// <param name="area"> List of cells </param>
        /// <returns> True if given mine placement is correct, false otherwise </returns>
        private bool CorrectMinePlacement(Area area)
        {
            foreach (Cell cell in area)
            {
                if (cell.IsNumber() && cell.value != CountPlacedMines(cell))
                    return false;
            }
            return true;
        }

        /// <summary> Count mines around given cell </summary>
        /// <returns> Number of mines around the cell </returns>
        private int CountPlacedMines(Cell cell)
        {
            int count = 0;
            var neighbours = Neighbours.Get(cell);
            foreach (Cell neighbour in neighbours)
            {
                if (neighbour.isKnown && neighbour.IsMine())
                    count++;
                else if (neighbour.currentState == CellState.MINE)
                    count++;
            }

            return count;
        }
    }
}