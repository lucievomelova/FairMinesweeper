namespace Minesweeper
{
    /// <summary> Class that places mines around open areas - either when game field is reGenerated or when Solver
    /// can't solve something </summary>
    public partial class PlaceMines
    {
        private enum CellState
        {
            MINE, 
            NUMBER, 
            KNOWN, 
            UNOPENED
        }

        /// <summary> Array used for bruteforce. Each cell has a beginning state. If NUMBER or MINE state doesn't change,
        /// then that cell contains NUMBER or MINE </summary>
        readonly CellState[,] stateArray;

        public PlaceMines()
        {
            stateArray = new CellState[Values.height, Values.width];
        }
    }
}