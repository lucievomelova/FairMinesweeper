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


        // used when saving unfinished game
        public const int NUMBER_MARKED_WITH_A_FLAG = 20;
        public const int NUMBER_MARKED_WITH_A_QUESTION_MARK = 40;
        public const int UNOPENED_NUMBER = 60;
        public const int MINE_MARKED_WITH_A_FLAG = 80;
        public const int MINE_MARKED_WITH_A_QUESTION_MARK = 100;
        public const int UNMARKED_MINE = 120;
        public const int KNOWN = 10;

        public static bool InBounds(int row, int column)
        {
            return (row < height) && (row >= 0) && (column < width) && (column >= 0);
        }
    }
}