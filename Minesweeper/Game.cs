using System.Windows.Controls;

namespace Minesweeper
{
    public static class Game
    {
        /// <summary> how many cells (buttons) aren't opened (including marked cells) </summary>
        public static int unopenedLeft;

        /// <summary> how many flags (to mark mines) aren't placed yet </summary>
        public static int flagsLeft;

        /// <summary> how many mine locations are unknown </summary>
        public static int minesLeft;

        /// <summary> number of columns </summary>
        public static int width = 9;

        /// <summary> number of rows </summary>
        public static int height = 9;

        /// <summary> total number of mines in current game </summary>
        public static int mines = 10;

        public static Cell[,] cells;

        /// <summary> State of previous game - if it was lost or won or if it wasn't neither of those two </summary>
        public enum PreviousGame
        {
            NORMAL, //last game was ended before player won or lost (this is also the state after app start)
            WIN, //player won last game
            LOSE //player lost last game
        }

        /// <summary> true if the user wants known cells to be highlighted </summary>
        public static bool HighlightKnownCells = true;
        
        /// <summary> find cell which is represented by button *btn* </summary>
        public static Cell FindCell(Button btn)
        {
            for (int r = 0; r < height; r++)
            for (int c = 0; c < width; c++)
                if (cells[r, c].btn == btn) return cells[r, c];
            
            return null;
        }
    }
}