namespace Minesweeper
{
    public static class Values
    {
        public const int cellSize = 30;


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
            return (row < Game.height) && (row >= 0) && (column < Game.width) && (column >= 0);
        }
    }
}