using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper
{
    public partial class MainWindow
    {
        /// <summary> left mouse button is pressed down </summary>
        private void BtnMouseDown(object sender, MouseEventArgs e)
        {
            BtnMouseEnter(sender, e);
            Cell cell = Game.FindCell((Button) sender); //mouse cursor is over this cell
            if (cell.value > 0 && cell.value - Neighbours.CountMarkedMines(cell) == 0 && Neighbours.AllAreKnown(cell))
                Neighbours.PressDown(cell);
        }

        /// <summary> left mouse button is released </summary>
        private void BtnMouseUp(object sender, MouseEventArgs e)
        {
            OpenBtn((Button) sender);
            if (previousGame != Game.PreviousGame.LOSE)
                BtnMouseLeave(sender, e);

            Cell cell = Game.FindCell((Button) sender); //mouse cursor is over this cell
            if (cell.value > 0 && cell.value - Neighbours.CountMarkedMines(cell) == 0 && Neighbours.AllAreKnown(cell))
                Neighbours.ReleaseUp(cell);
        }

        private void BtnMouseEnter(object sender, MouseEventArgs e)
        {
            Cell cell = Game.FindCell((Button) sender);
            if (!cell.isOpened && !cell.IsMarked())
                cell.SetImage(Img.EmptyMouseOver);
        }

        private void BtnMouseLeave(object sender, MouseEventArgs e)
        {
            Cell cell = Game.FindCell((Button) sender);
            Img.UpdateUnopened(cell);
        }


        /// <summary> right mouse button is clicked </summary>
        private void RightClick(object sender, RoutedEventArgs e)
        {
            Cell cell = Game.FindCell((Button) sender);
            PlaceFlag(cell);
            CheckWin();
        }
    }
}