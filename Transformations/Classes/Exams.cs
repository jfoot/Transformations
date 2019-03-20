using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Transformations
{
	public class Exam       //Declares an exam object
	{
		private int attmepts = 0;           //Counts the number of attempts
		private int totalAttempts = 0;      //Counts the number of total attempts 
		private int scoreValue = 0;        //Records the score 
		private int questionPos;           //Records the current question their on.
		public int ArrayPos { get; set; }              //Records the current array index.
        public readonly int ExamID;                 //Records the exam ID.
        public readonly string ExamName;            //Records the name of the exam.
        public Time Timer { get; set; }    //Creates a timer object


        public Exam(int quePos, int arrPos, string examName, int examID, Label label)
		{
			questionPos = quePos;
			ArrayPos = arrPos;
            ExamID = examID;
            ExamName = examName;
            Timer = new Time(label);
            Timer.Start();
		}
		
		public int Attmepts
		{
			get { return attmepts; }
		}
		public int TotalAttempts
		{
			get { return totalAttempts; }
		}
		public int ScoreValue
		{
			get { return scoreValue; }
		}
		public int QuestionPos
		{
			get { return questionPos; }
		}

		public void AddAttempt()
		{
			attmepts++;
			totalAttempts++;
		}
		public void ResetAttempts()
		{
			attmepts = 0;
		}
		public void AddQuesPos()
		{
			questionPos++;
		}
		public void AddPoint()
		{
			scoreValue++;
		}
	}


	public class Time   //Declares a timer object
	{
		private DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();    //Creates a timer
        private int seconds;
        private Label Label;
        
        //   int Seconds { get; set; } = 0;    //Records the number of seconds
		//private int Minutes { get; set; } = 0;    //Records the number of minuets.

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
