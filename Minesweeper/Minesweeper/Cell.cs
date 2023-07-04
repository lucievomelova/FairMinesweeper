using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    public class Cell
    {
        public int row;

        public int column;
        
        /// <summary> cell value - either a number (0-8) or a mine (Values.MINE) </summary>
        public int value;

        /// <summary> how many mines around cell aren't known yet (only relevant for number cells) </summary>
        public int minesLeft; 
    
        /// <summary> how many unknown cells are there around the cell - those are candidates for mines </summary>
        public int unknownLeft; 
        
        /// <summary> true if cell is opened </summary>
        public bool isOpened;

        /// <summary> true if cell is still closed and it's marked with a flag </summary>
        public bool isFlag;

        /// <summary> true if cell is still closed and it's marked with a question mark </summary>
        public bool isQuestionMark;

        /// <summary> true if cell value is known - either it's already opened or it was calculated by solver.Update() </summary>
        public bool isKnown;

        /// <summary> corresponding Button on the game field </summary>
        public readonly Button btn;

        public Cell(int row, int column)
        {
            this.row = row;
            this.column = column;
            value = Values.UNKNOWN;
            minesLeft = 8;
            unknownLeft = 8;
            isOpened = false;
            isFlag = false;
            isQuestionMark = false;
            isKnown = false;
            btn = new Button();
        }
        
        public void SetImage(BitmapImage img) { Img.Set(btn, img); }

        public bool IsMine() { return value == Values.MINE; }

        public bool IsNumber() { return value  >= 0; }

        public bool IsMarked() { return isFlag || isQuestionMark; }
    }
}