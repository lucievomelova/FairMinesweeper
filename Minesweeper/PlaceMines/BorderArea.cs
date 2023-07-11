using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public partial class PlaceMines
    {
        /// <summary> Find cells that are open and don't have enough known mines around </summary>
        private List<helpCell> FindBorderArea()
        {
            List<helpCell> area = new List<helpCell>();
            foreach (helpCell h in helpArray)
                if (h.known && h.unknownMinesLeft > 0 && h.value > 0)
                    area.Add(h);

            return area;
        }

        /// <summary> Bruteforce for finding known numbers or mines. Every correct combination is computed and if
        /// a cell is a number or a mine in every combination, then it has to be number/mine in game. </summary>
        public void TryAll()
        {
            CopyField();
            SplitIntoSections();

            // fill state array with first combination
            if (FirstCombinationOnArea())
            {
                foreach (helpCell h in helpArray)
                {
                    if(Game.cells[h.row, h.column].isKnown)
                        stateArray[h.row, h.column] = CellState.KNOWN; //we don't care about known cells
                    else if (h.value == Values.MINE)
                        stateArray[h.row, h.column] = CellState.MINE;
                    else if (h.value >= 0 || h.value == Values.NUMBER)
                    {
                        if(CountUnknownNeighbours(h) != 8)
                            stateArray[h.row, h.column] = CellState.NUMBER;
                        else
                            stateArray[h.row, h.column] = CellState.UNOPENED;
                    }
                    else
                        stateArray[h.row, h.column] = CellState.UNOPENED; //we don't care about unopened or unknown cells
                }
            }
            
            int counter = 0;
            //now try all possible combinations on area
            while (NextCombinationOnArea())
            {
                counter++;
                foreach (helpCell h in helpArray)
                {
                    //if cell was a mine in previous combinations, we have to check if it's a mine in current combination
                    if (h.value == Values.MINE)
                    {
                        if (stateArray[h.row, h.column] == CellState.NUMBER) //it's a number in current combination
                            stateArray[h.row, h.column] = CellState.UNOPENED; //marked as unknown
                    }
                    //if cell was a number in previous combinations, we have to check if it's a number in current combination
                    else if (h.value == Values.NUMBER || h.value >= 0)
                    {
                        if (stateArray[h.row, h.column] == CellState.MINE)
                            stateArray[h.row, h.column] = CellState.UNOPENED;
                    }
                }
                if (counter > 500) return; //this is just to ensure that program doesn't for too long
            }

            for (int r = 0; r < Game.height; r++)
            {
                for (int c = 0; c < Game.width; c++)
                {
                    CellState state = stateArray[r, c];
                    Cell cell = Game.cells[r, c];
                    
                    if (state == CellState.MINE) //cell was mine in every combination
                    {
                        if (cell.IsMarked()) continue;
                        Game.minesLeft--;
                        Game.cells[r, c].isKnown = true; //mark as known
                        if(Game.DebugMode)
                            cell.SetImage(Img.Known);
                    }
                    else if (state == CellState.NUMBER) //cell was number in every combination
                    {
                        if (cell.IsMarked()) continue;
                        Game.cells[r, c].isKnown = true;//mark as known
                        if(Game.DebugMode)
                            cell.SetImage(Img.Known);
                    }
                }
            }
        }


        /// <summary> Find next correct combination of mines around border area. </summary>
        /// <returns> True if correct combination was found, otherwise false </returns>
        private bool NextCombinationOnArea()
        {
            // A correct combination of mines around area was already found, so mines are already placed around the area.
            // This method takes last combination and tries to find next combination.
            
            //index of current cell in area - starting at last cell, because correct combination was already found, so
            // mines are correctly placed around all cells in area
            int index = FindBorderArea().Count-1; 
            while(index >= 0)
            {
                bool correctCombination = FindCombinationOnArea(ref index);
                if (correctCombination) return true;
            }
            GC.Collect();
            return false;
        }

        /// <summary> Find correct combination of mines around border area. </summary>
        /// <returns> True if correct combination was found, otherwise false </returns>
        public bool FirstCombinationOnArea()
        {
            //this method return true as soon as correct combination was found
            
            int index = 0; //index of current cell in area (starting at first cell in list)
            while(index >= 0)
            {
                bool correctCombination = FindCombinationOnArea(ref index);
                if (correctCombination) return true;
            }
            GC.Collect();
            return false;
        }

        /// <summary> Try next combination of mines around border area </summary>
        /// <returns> True if found combination is correct. </returns>
        private bool FindCombinationOnArea(ref int cellIndex)
        {
            List<helpCell> area = FindBorderArea();
            if (area.Count == 0) return true;

            bool correctMinesAroundCurrentCell = NextCombinationOnCell(area[cellIndex]);
            
            if (correctMinesAroundCurrentCell) cellIndex++;
            else cellIndex--;

            if (cellIndex >= area.Count) //check if new solution was found
            {
                cellIndex--;
                if (CorrectMinePlacement()) return true;
            }
            return false;
        }
    }
}