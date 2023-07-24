using System;
using System.Collections.Generic;
using System.Threading;

namespace Minesweeper
{
    public partial class Bruteforce
    {
        
        
        private Thread[] threads;
        private int numberOfThreads;
        
        private void Init()
        {
            numberOfThreads = Environment.ProcessorCount - 1;
            if (numberOfThreads == 0)
                numberOfThreads = 1;
            threads = new Thread[numberOfThreads];
        }

        
        private static List<Cell> FindUnresolvedOpenCells()
        {
            List<Cell> area = new List<Cell>();
            foreach(Cell cell in Game.cells)
                if (cell.isOpened && cell.unknownLeft > 0 && cell.value > 0)
                     area.Add(cell);
            return area;
        }


        private static List<List<Cell>> SplitAreaIntoNonneighboringAreas(List<Cell> wholeArea)
        {
            List<List<Cell>> splitAreas = new List<List<Cell>>();
            
            if (wholeArea.Count == 0)
                 return null;
             
             int counter = 0;
             splitAreas.Add(new List<Cell>());
             splitAreas[0].Add(wholeArea[0]);
             for (int i = 1; i < wholeArea.Count; i++)
             {
                 var currentCell = wholeArea[i];
                 bool sameAsCurrentArea = false;
                 // check if current cell is close enough to any cell in current openedArea
                 foreach (Cell previousCell in splitAreas[counter])
                 {
                     if (Math.Abs(currentCell.row - previousCell.row) <= 2 &&
                         Math.Abs(currentCell.column - previousCell.column) <= 2)
                     {
                         sameAsCurrentArea = true;
                         break;
                     }
                 }

                 if (!sameAsCurrentArea)
                 {
                     counter++; // it is not close enough to any cell in previous openedArea -> create new openedArea for it
                     splitAreas.Add(new List<Cell>());
                 }
                 splitAreas[counter].Add(currentCell);

             }

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
        
        
        public void TryAll()
        {
            Init();
            var unresolvedCells = FindUnresolvedOpenCells();
            var unresolvedAreas = SplitAreaIntoNonneighboringAreas(unresolvedCells);
            
            if (unresolvedAreas == null)
                return;
            
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
                {
                    if(threads[threadNum] != null && threads[threadNum].ThreadState == ThreadState.Running)
                        threads[threadNum].Join();
                }
                
                var area = unresolvedAreas[i];
                threads[threadNum] = new Thread(() => TryAllCombinationsOnArea(area)); 
                threads[threadNum].Start();
            }
            
            for (int t = 0; t < numberOfThreads; t++)
            {
                if (threads[t] != null && threads[t].ThreadState == ThreadState.Running)
                    threads[t].Join();
            }
        }

        private void TryAllCombinationsOnArea(List<Cell> openedArea)
        {
            List<Cell> neighbourArea = GetUnopenedNeighbours(openedArea);
            FindAllCombinations(openedArea, neighbourArea, 0, 0, Game.minesLeft);
        }

        private void FindCombinationsOnArea(List<Cell> openedArea)
        {
            List<Cell> neighbourArea = GetUnopenedNeighbours(openedArea);
            bool stop = false;
            FindCombination(openedArea, neighbourArea, 0, ref stop);
        }


        private List<Cell> GetUnopenedNeighbours(List<Cell> area)
        {
            List<Cell> neighbourArea = new List<Cell>();
            foreach (Cell cell in area)
            {
                var cellNeighbours = Neighbours.Get(cell);
                foreach (Cell neigbour in cellNeighbours)
                {
                    if(!neigbour.isKnown && !neigbour.isOpened && !neighbourArea.Contains(neigbour))
                        neighbourArea.Add(neigbour);
                }
            }

            return neighbourArea;
        }
        

        private void FindAllCombinations(List<Cell> openedArea, List<Cell> neighbourArea, int neighbourIndex, int openCellIndex, int minesLeft)
        {
            if (neighbourIndex == neighbourArea.Count)
            {
                bool correct = CorrectMinePlacement(openedArea);
                if (correct)
                {
                    SaveCurrentCombination(neighbourArea);
                }
                return;
            }
            

            Cell neighbour = neighbourArea[neighbourIndex];

            // check if mine can be placed on this position
            bool possibleMine = true;
            foreach (Cell neighboursNeighbour in Neighbours.Get(neighbour))
            {
                if (neighboursNeighbour.isOpened && CountPlacedMines(neighboursNeighbour) >= neighboursNeighbour.value)
                    possibleMine = false;
            }

            if (possibleMine && minesLeft > 0) // try to place mine here if possible
            {
                neighbour.currentState = CellState.MINE;
                FindAllCombinations(openedArea, neighbourArea, neighbourIndex + 1, openCellIndex, minesLeft-1);
            }

            neighbour.currentState = CellState.NUMBER; // try to place number
            FindAllCombinations(openedArea, neighbourArea, neighbourIndex + 1, openCellIndex, minesLeft);
        }

        
        private void FindCombination(List<Cell> openedArea, List<Cell> neighbourArea, int index, ref bool stop)
        {
            if (index == neighbourArea.Count)
            {
                bool correct = CorrectMinePlacement(openedArea); 
                if (correct)
                {
                    SaveCurrentCombination(neighbourArea);
                    stop = true; // stop if correct placement was found
                }
                return;
            }
 
            neighbourArea[index].currentState = CellState.MINE;
            if(!stop)
                FindCombination(openedArea, neighbourArea, index + 1, ref stop);
 
            neighbourArea[index].currentState = CellState.NUMBER;
            if(!stop)
                FindCombination(openedArea, neighbourArea, index + 1, ref stop);
        }
        

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

            FindCombinationsOnArea(unresolvedAreas[0]); // solve first openedArea by main thread because it is definitely free
            
            for (int i = 1; i < unresolvedAreas.Count; i++)
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
                {
                    if(threads[threadNum] != null && threads[threadNum].ThreadState == ThreadState.Running)
                        threads[threadNum].Join();
                }
                
                var area = unresolvedAreas[i];
                threads[threadNum] = new Thread(() => FindCombinationsOnArea(area)); 
                threads[threadNum].Start();
            }
            
            for (int t = 0; t < numberOfThreads; t++)
            {
                if (threads[t] != null && threads[t].ThreadState == ThreadState.Running)
                    threads[t].Join();
            }
            
            
            // check correct mine placement
            for (int r = 0; r < Game.height; r++)
            {
                 for (int c = 0; c < Game.width; c++)
                 {
                     Cell cell = Game.cells[r, c];
                     if (cell.IsNumber() && cell.value != CountPlacedMines(cell))
                         return false;
                 }
             }

            return true;
        }

        private void SaveCurrentCombination(List<Cell> neighbourArea)
        {
            foreach (Cell cell in neighbourArea)
            {
                if (cell.longTermState == CellState.NONE)
                    cell.longTermState = cell.currentState;
                else if (cell.longTermState != cell.currentState)
                    cell.longTermState = CellState.UNKNOWN;
            }
        }


        private bool CorrectMinePlacement(List<Cell> area)
        {
            foreach (Cell cell in area)
            {
                if (cell.IsNumber() && cell.value != CountPlacedMines(cell))
                    return false;
            }

            return true;
        }

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