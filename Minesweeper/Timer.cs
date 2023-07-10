using System;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary> class for calculating time that passed from start </summary>
    public class Timer
    {
        private readonly DispatcherTimer timer;
        private DateTime TimerStart;
        private readonly MainWindow mainWindow;
        private int startingTime = 0;

        public Timer(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            timer = new DispatcherTimer();
        }

        public void Start()
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += TimerTick;
            timer.Start();
            TimerStart = DateTime.Now;
        }

        public void Stop()
        {
            if(timer.IsEnabled)
                timer.Stop();
        }
        
        //increment TimeLabel every second
        private void TimerTick(object sender, EventArgs e)
        {
            int seconds = TimePassed();
            if(seconds < 1000)
                mainWindow.TimeLabel.Content = seconds.ToString("D3");
            else
                mainWindow.TimeLabel.Content = "999";
        }

        public int TimePassed()
        {
            TimeSpan timePassed = DateTime.Now - TimerStart;
            int seconds = (int)timePassed.TotalSeconds + startingTime;
            return seconds;
        }

        public void SetStartTime(int startTime)
        {
            startingTime = startTime;
        }
    }
}