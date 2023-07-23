// using System.Collections.Generic;
// using System.Linq;
//
// namespace Minesweeper
// {
//     public partial class Bruteforce
//     {
//         
//          private List<Cell> UnknownNeighbours(Cell cell)
//          {
//              List<Cell> neighbours = Neighbours.Get(cell);
//              for (int i = 0; i < neighbours.Count; i++)
//                  if (neighbours[i].isKnown)
//                      neighbours.Remove(neighbours[i--]);
//
//              return neighbours;
//          }
//         
//         public void SplitIntoSections()
//         {
//             List<Cell> area = FindUnresolvedOpenCells();
//             foreach (Cell cell in area)
//             {
//                 foreach (Cell neighbour in UnknownNeighbours(cell))
//                     if (Equals(neighbour, neighbour.parent))
//                     {
//                         neighbour.parent = cell;
//                         cell.children.Add(neighbour);
//                     }
//             }
//         }
//
//         /// <summary> Find section whose parent is *cell* </summary>
//         /// <param name="cell"> Section parent </param>
//         private List<Cell> Section(Cell cell)
//         {
//             return cell.children;
//         }
//
//         
//         /// <summary> Set unknown neighbours around cell to numbers </summary>
//         /// <param name="cell"></param>
//         private void SectionAroundCellToNumbers(Cell cell)
//         {
//             foreach (Cell c in Section(cell))
//                 if (c.value == Values.UNKNOWN && Equals(c.parent, cell))
//                     c.currentState = CellState.NUMBER;
//         }
//     
//
//         /// <summary> Find mine in section with parent *cell* on the highest position </summary>
//         /// <param name="cell"> Section parent </param>
//         /// <returns> Position of last mine: -1 if no mine was found, 0-7 if mine was found. Each number 0-7 represents
//         /// one neighbour around *cell* </returns>
//         private int FindLastMineinSection(Cell cell)
//         {
//             int position = -1;
//             foreach (var c in Section(cell).Where(c => c.value == Values.MINE))
//                 position = Section(cell).IndexOf(c);
//
//             return position;
//         }
//
//         
//         /// <summary> Place remaining mines around a cell </summary>
//         /// <returns> True if a solution was found, false otherwise </returns>
//         private bool NextCombinationOnCell(Cell cell)
//         {
//             int position = FindLastMineinSection(cell); //position of last placed mine around cel
//             List<Cell> section = Section(cell);
//             SectionAroundCellToNumbers(cell);
//
//             if (position == -1) //if no mines were placed yet, place all remaining mines around cell
//             {
//                 int mines = cell.value - CountNeighbourMines(cell);
//                 if (mines > section.Count) return false; //more mines than avaliable places
//                 if (mines < 0) return false; //incorrect number of mines
//
//                 if (mines == 0) //no mines to be placed, there is already correct number of mines around cell
//                 {
//                     if (!cell.secondTime)
//                     {
//                         helpArray[cell.row, cell.column].secondTime = true;
//                         return true;
//                     }
//
//                     //if this is the second time this same combination is tried, return false
//                     helpArray[cell.row, cell.column].secondTime = false;
//                     return false;
//                 }
//
//                 foreach (Cell h in section) //place mines around cell
//                 {
//                     helpArray[h.row, h.column].value = Values.MINE;
//                     if (--mines == 0) break;
//                 }
//                 return true;
//             }
//
//
//             //next section is applied when a mine was found. First this mine will be deleted and the program will try to 
//             //place it further (on higher position). If this isn't possible, other mines will be deleted until a mine 
//             //that can be moved further is found
//
//             //delete last mine
//             Cell mineCell = section[position];
//             helpArray[mineCell.row, mineCell.column].value = Values.NUMBER;
//
//             int deletedMines = 1;
//             
//             //if last mine was on the last position, delete previous mines until a mine that can be placed further is found
//             while (position + deletedMines >= section.Count)
//             {
//                 section = Section(cell);
//                 position = FindLastMineinSection(cell); //last mine position
//                 if (position < 0) return false; //no mine was found - position == -1
//
//                 mineCell = section[position]; //current mine
//                 helpArray[mineCell.row, mineCell.column].value = Values.NUMBER; //delete mine
//                 if (++deletedMines > cell.unknownMinesLeft) return false;
//             }
//
//             //place remaining mines around cell
//             for (int i = 0; i < deletedMines; i++)
//             {
//                 if (++position >= 8) return false;
//                 mineCell = section[position];
//                 helpArray[mineCell.row, mineCell.column].value = Values.MINE;
//             }
//             return true;
//         }
//     }
// }