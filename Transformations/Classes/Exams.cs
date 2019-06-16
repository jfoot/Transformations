using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Animation;


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

        public void Refresh(Label question_no, Label score, Label attempts)
        {
            question_no.Content = Properties.Strings.Question + ": " + QuestionPos.ToString() + "/6";
            score.Content = Properties.Strings.Score + ": " + ScoreValue.ToString() + "/6";
            attempts.Content = Properties.Strings.Attempts + ": " + Attmepts.ToString() + "/2";
        }

        public void Exit(Window win)
        {
            MessageBoxResult exit = MessageBox.Show(Properties.Strings.LeaveExam, Properties.Strings.AreYouSure,
                 MessageBoxButton.OKCancel,
                 MessageBoxImage.Warning);

            if (exit == MessageBoxResult.OK)
            {
                TakeExam exam = new TakeExam();
                exam.Show();
                win.Close();
            }
        }

        public void Show(Border type) //Shows the correct, incorrect or skip answer method.
        {
            type.Visibility = System.Windows.Visibility.Visible;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(1),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, type);
            Storyboard.SetTargetProperty(a, new PropertyPath(Window.OpacityProperty));
            storyboard.Completed += delegate { type.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();
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
