using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Transformations
{
	public class Exam       //Declares an exam object
	{
		public int Attmepts { get; private set; } = 0;           //Counts the number of attempts
        public int TotalAttempts { get; private set; } = 0;      //Counts the number of total attempts 
        public int ScoreValue { get; private set; } = 0;        //Records the score 
        public int QuestionPos { get; private set; }           //Records the current question their on.
		public int ArrayPos { get; set; }              //Records the current array index.
        public Time Timer { get; set; }    //Creates a timer object
        public readonly int ExamID;                 //Records the exam ID.
        public readonly string ExamName;            //Records the name of the exam.

        public Exam(int quePos, int arrPos, string examName, int examID, Label label)
		{
			QuestionPos = quePos;
			ArrayPos = arrPos;
            ExamID = examID;
            ExamName = examName;
            Timer = new Time(label);
            Timer.Start();
		}
		
		public void AddAttempt()
		{
			Attmepts++;
			TotalAttempts++;
		}
		public void ResetAttempts()
		{
			Attmepts = 0;
		}
		public void AddQuesPos()
		{
			QuestionPos++;
		}
		public void AddPoint()
		{
			ScoreValue++;
		}
	}


	public class Time   //Declares a timer object
	{
		private DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();    //Creates a timer
        private int seconds;
        private Label Label;
        
        public Time(Label ls)
        {
            Label = ls;
            DispatcherTimer.Tick += new EventHandler(TimerTick);
            DispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        public void Start()
        {
            DispatcherTimer.Start();
        }

        public void Stop()
        {
            DispatcherTimer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            seconds++;
            Label.Content = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        }

        public string GetString()
        {
            return (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        }
    }
}
