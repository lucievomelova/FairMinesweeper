using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Minesweeper
{       
    public static class Img
    {
        public static BitmapImage EmptyMouseOver = new BitmapImage(new Uri("pic/0.png", UriKind.Relative));
        public static BitmapImage Empty = new BitmapImage(new Uri("pic/blank.png", UriKind.Relative));
        public static BitmapImage Known = new BitmapImage(new Uri("pic/pink_blank.png", UriKind.Relative));
        
        public static BitmapImage Blue = new BitmapImage(new Uri("pic/blue_blank.png", UriKind.Relative));
        public static BitmapImage Purple = new BitmapImage(new Uri("pic/purple_blank.png", UriKind.Relative));
        
        public static BitmapImage QuestionMark = new BitmapImage(new Uri("pic/question_mark.png", UriKind.Relative));
        public static BitmapImage Flag = new BitmapImage(new Uri("pic/flag.png", UriKind.Relative));
        
        public static BitmapImage Mine = new BitmapImage(new Uri("pic/mine.png", UriKind.Relative));
        public static BitmapImage WrongMine = new BitmapImage(new Uri("pic/wrong_mine.png", UriKind.Relative));
        public static BitmapImage MineClicked = new BitmapImage(new Uri("pic/mine_clicked.png", UriKind.Relative));

        
        public static BitmapImage Win = new BitmapImage(new Uri("pic/win.png", UriKind.Relative));
        public static BitmapImage WinMouseOver = new BitmapImage(new Uri("pic/win_down.png", UriKind.Relative));
        
        public static BitmapImage NewGame = new BitmapImage(new Uri("pic/smiley.png", UriKind.Relative));
        public static BitmapImage NewGameMouseOver = new BitmapImage(new Uri("pic/smiley_down.png", UriKind.Relative));
        
        public static BitmapImage GameOver = new BitmapImage(new Uri("pic/game_over.png", UriKind.Relative));
        public static BitmapImage GameOverMouseOver = new BitmapImage(new Uri("pic/game_over_down.png", UriKind.Relative));


        /// <returns> Image of number *n*  </returns>
        public static BitmapImage Number(int n)
        {
            return new BitmapImage(new Uri("pic/" + n + ".png", UriKind.Relative));
        }

        /// <summary> Set content of button *btn* to Image *img* </summary>
        public static void Set(Button btn, BitmapImage img)
        {
            btn.Content = new Image {Source = img};
        }
    }
}