using System.Windows;
using System.Windows.Controls;

namespace Minesweeper
{
    public partial class MainWindow
    {
        
        /// <summary> check if player has already won the game - either all mines are marked with a flag or the number of
        /// unopened cells is equal to number of mines in game </summary>
        public void CheckWin()
        {
            if ((solver.CountIncorrectFlags() == 0 && Game.flagsLeft == 0 && Game.unknownMinesLeft == 0) ||
                Game.unopenedLeft == Game.mines)
                Win();
        }

        private void Win()
        {
            timer.Stop();
            open.OpenEverything();
            previousGame = Game.PreviousGame.WIN;
            foreach (Cell c in Game.cells)
            {
                DisableButton(c.btn);
                if (c.IsMine())
                    c.SetImage(Img.Flag);
            }
            MinesLeftLabel.Content = 0;
            Img.Set(NewGameButton, Img.Win);
            
            Win winWindow = new Win(timer.TimePassed());
            winWindow.Show();
        }

        public void DisableButton(Button btn)
        {
            btn.MouseRightButtonUp -= RightClick;
            btn.MouseEnter -= BtnMouseEnter;
            btn.MouseLeave -= BtnMouseLeave;
            btn.PreviewMouseLeftButtonDown -= BtnMouseDown;
            btn.PreviewMouseLeftButtonUp -= BtnMouseUp;
        }

    }
}