namespace Minesweeper
{
    public static class Values
    {
        /// <summary> how many cells (buttons) aren't opened (including marked cells) </summary>
        public static int unopenedLeft; 
        
        /// <summary> how many flags (to mark mines) aren't placed yet </summary>
        public static int flagsLeft; 
        
        /// <summary> how many mine locations are unknown </summary>
        public static int minesLeft;


        public const int cellSize = 30;

        /// <summary> number of columns </summary>
        public static int width = 9;

        /// <summary> number of rows </summary>
        public static int height = 9;

        /// <summary> total number of mines in current game </summary>
        public static int mines = 10;
        

        //constants
        public const int NUMBER = -1; //cell is a number
        public const int MINE = -2; //cell is a mine
        public const int UNKNOWN = -3; //we can't be sure if cell is a number or a mine
        public const int MINE_CLICKED = -4; //cell is a mine and player clicked on it

        public static bool InBounds(int row, int column)
        {
            return (row < height) && (row >= 0) && (column < width) && (column >= 0);
        }
    }
}