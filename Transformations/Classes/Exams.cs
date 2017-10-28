using System.Windows.Threading;

namespace Transformations
{
	public class Exam       //Declares an exam object
	{
		private Time timer = new Time();    //Creates a timer object
		private int attmepts = 0;           //Counts the number of attempts
		private int totalAttempts = 0;      //Counts the number of total attempts 
		private int scoreValue = 0;        //Records the score 
		private int questionPos;           //Records the current question their on.
		private int arrayPos;              //Records the current array index.
        private int examID;                 //Records the exam ID.
        private string examName;            //Records the name of the exam.

		public Exam(int QuePos, int ArrPos, string ExamName, int ExamID)
		{
			questionPos = QuePos;
			arrayPos = ArrPos;
            examID = ExamID;
            examName = ExamName;
		}
		
		public Time Timer
		{
			get { return timer; }
			set { timer = value; }
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
		public int ArrayPos
		{
			get { return arrayPos; }
			set { arrayPos = value; }
		}
        public int ExamID
        {
            get { return examID; }
        }
        public string ExamName
        {
            get { return examName; }
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
		private DispatcherTimer dispatcherTimer = new DispatcherTimer();    //Creates a timer
		private int seconds = 0;    //Records the number of seconds
		private int minutes = 0;    //Records the number of minuets.

		public DispatcherTimer DispatcherTimer
		{
			get { return dispatcherTimer; }
			set { dispatcherTimer = value; }
		}

		public int Seconds
		{
			get { return seconds; }
			set { seconds = value; }
		}

		public int Minutes
		{
			get { return minutes; }
			set { minutes = value; }
		}
	}
}
