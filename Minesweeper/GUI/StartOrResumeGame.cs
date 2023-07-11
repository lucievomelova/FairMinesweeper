namespace Minesweeper
{
    public partial class MainWindow
    {
        
        /// <summary> Resume game after loading from a file </summary>
        /// <param name="time"> Time that passed while playing the game before saving - we want the timer to
        /// continue from that time </param>
        public void ResumeGame(int time)
        {
            PrepareGame();
            timer.Stop();
            previousGame = Game.PreviousGame.NORMAL;
            newGame = false;
            model.placeMines.InitHelpArrays();
            timer.SetStartTime(time);
            timer.Start();
        }
        
        /// <summary> Start new game by clicking on a random cell on the game field </summary>
        /// <param name="cell">Clicked cell</param>
        private void StartNewGame(Cell cell)
        {
            newGame = false;
            timer.Start();
            model.Generate(cell); //generate new game field
            open.OpenArea(cell); //open clicked cell (there will always be number 0, so area will be opened)
        }
    }
}