// using System.Collections.Generic;
// using System.Linq;
//
// namespace Minesweeper
// {
//     public partial class Solver
//     {
//         /// <summary> struct that imitates Cell class (but only copies useful variables) </summary>
//         public struct helpCell
//         {
//             public int value;
//             public int unknownMinesLeft;
//             public bool known;
//             public bool secondTime; //used only when *helpCell* is parent of a section with Count == 0
//             public int row;
//             public int column;
//         }
//
//         /// <summary> copy of actual game field </summary>
//         public helpCell[,] helpArray;
//
//         public void InitHelpArrays()
//         {
//             helpArray = new helpCell[Game.height, Game.width]; //copy of MainWindow.cells
//             parents = new helpCell[Game.height, Game.width];
//         }
//
//         //copy MainWindow.cells to helpArray
//         public void CopyField()
//         {
//             for (int r = 0; r < Game.height; r++)
//             {
//                 for (int c = 0; c < Game.width; c++)
//                 {
//                     Cell cell = Game.cells[r, c];
//                     helpArray[r, c].known = true;
//
//                     if (cell.isOpened)
//                         helpArray[r, c].value = Game.cells[r, c].value;
//
//                     else if (cell.isKnown)
//                         helpArray[r, c].value = cell.value >= 0 ? Values.NUMBER : Values.MINE;
//
//                     else
//                     {
//                         helpArray[r, c].value = Values.UNKNOWN;
//                         helpArray[r, c].known = false;
//                     }
//
//                     helpArray[cell.row, cell.column].unknownMinesLeft = cell.minesLeft;
//                     helpArray[r, c].row = cell.row;
//                     helpArray[r, c].column = cell.column;
//                     parents[r, c] = helpArray[r, c];
//                 }
//             }
//         }
//
//         /// <returns> How many known mines are in helpArray </returns>
//         public int KnownMinesInHelpArray()
//         {
//             int mines = 0;
//             foreach (helpCell h in helpArray)
//                 if (h.value == Values.MINE && Game.cells[h.row, h.column].isKnown)
//                     mines++;
//
//             return mines;
//         }
//
//         /// <returns> How many cells with value Values.UNKNOWN are there in helpArray </returns>
//         private int UnknownCellsInHelpArray()
//         {
//             //return helpArray.Cast<helpCell>().Count(h => h.value == Values.UNKNOWN);
//             int counter = 0;
//             foreach (helpCell h in helpArray)
//                 if (h.value == Values.UNKNOWN)
//                     counter++;
//
//             return counter;
//         }
//
//         /// <summary> Check helpArray and count mines around all numbers </summary>
//         /// <returns> True if every number has correct number of mines around </returns>
//         private bool CorrectMinePlacement()
//         {
//             int mines = 0;
//             foreach (helpCell cell in helpArray)
//             {
//                 if (cell.value < 0)
//                 {
//                     if (cell.value == Values.MINE) mines++;
//                     continue;
//                 }
//                 if (!CorrectMinesAroundCell(cell)) return false;
//             }
//
//             //check if there are enough mines left for current mine placement
//             if (mines > Game.mines || mines + UnknownCellsInHelpArray() < Game.minesLeft)
//                 return false;
//
//             return true;
//         }
//
//         
//         #region Neighbours
//         
//         
//         private readonly int[] arr = {-1, 0, 1}; //array for going through cell neighbours
//
//         /// <summary> Check mines around cell </summary>
//         /// <returns> True if cell has the correct number of mines around </returns>
//         private bool CorrectMinesAroundCell(helpCell cell)
//         {
//             int counter = 0;
//             foreach (int r in arr)
//             {
//                 foreach (int c in arr)
//                 {
//                     if (r == 0 && c == 0) continue;
//                     if (Values.InBounds(cell.row + r, cell.column + c))
//                         if (helpArray[cell.row + r, cell.column + c].value == Values.MINE)
//                             counter++;
//                 }
//             }
//             return counter == cell.value;
//         }
//
//         private int CountNeighbourMines(helpCell cell)
//         {
//             return HelpNeighbours(cell).Count(neighbour => neighbour.value == Values.MINE);
//         }
//
//         private int CountUnknownNeighbours(helpCell cell)
//         {
//             int counter = 0;
//             foreach (int r in arr)
//             foreach (int c in arr)
//             {
//                 if (r == 0 && c == 0) continue;
//                 if (!Values.InBounds(cell.row + r, cell.column + c)) continue;
//                 if (helpArray[cell.row + r, cell.column + c].value == Values.UNKNOWN)
//                     counter++;
//             }
//             return counter;
//         }
//
//         private List<helpCell> UnknownNeighbours(helpCell cell)
//         {
//             List<helpCell> neighbours = HelpNeighbours(cell);
//             for (int i = 0; i < neighbours.Count; i++)
//                 if (neighbours[i].known)
//                     neighbours.Remove(neighbours[i--]);
//
//             return neighbours;
//         }
//
//         /// <summary> Neighbour helpCells of given helpCell </summary>
//         /// <returns> List of neighbours </returns>
//         private List<helpCell> HelpNeighbours(helpCell cell)
//         {
//             List<helpCell> neighbours = new List<helpCell>();
//             foreach (int r in arr)
//             {
//                 foreach (int c in arr)
//                 {
//                     if (r == 0 && c == 0) continue;
//                     if (Values.InBounds(cell.row + r, cell.column + c))
//                         neighbours.Add(helpArray[cell.row + r, cell.column + c]);
//                 }
//             }
//
//             return neighbours;
//         }
//
//         #endregion
//     }
// }