using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Media;

namespace Transformations
{
    /// <summary>
    /// Exam results is the end dialog window which appears upon completing an exam
    /// Showing the user their end results and the data which is sent to their teachers.
    /// </summary>
    public partial class ExamResults : Window
	{
		
		public bool Pass = true;
		public ExamResults(Exam Result)
		{
			InitializeComponent();
            //Sets the UI with data sent to it.
			ExamName.Content = Result.ExamName;
			Score.Content = Result.ScoreValue;
			Attempts.Content = Result.TotalAttempts;
            time.Content = Result.Timer.GetString();
            
            if (Result.ScoreValue < 5)     //Sets if the user has passed or failed an exam.
			{
				PassOrFail.Content = "Fail";
				PassOrFail.Foreground = new SolidColorBrush(Colors.Red);
				Pass = false;
			}

			
			if (Properties.Settings.Default.CurrentUser != "Guest" && Properties.Settings.Default.IsTeacher == false)   //If not a guest or teacher then save results
			{
				try
				{
					using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
					{	
						conn.Open();
						using (var command = new OleDbCommand("INSERT INTO ExamResults ([StudentID], [ExamID], [Score], [Attempts], [Time], [Pass]) VALUES (@StudentID,  @ExamID, @Score, @Attempts, @Time, @Pass)", conn))
						{   //Save the exam results into the exam results table.
							command.Parameters.AddWithValue("@StudentID", Properties.Settings.Default.UserID);
							command.Parameters.AddWithValue("@ExamID", Result.ExamID);
							command.Parameters.AddWithValue("@Score", Result.ScoreValue);
							command.Parameters.AddWithValue("@Attempts", Result.TotalAttempts);
							command.Parameters.AddWithValue("@Time", time.Content);
							command.Parameters.AddWithValue("@Pass", Pass);
							command.ExecuteNonQuery();
						}
					}
				}
				catch (Exception)
				{
					MessageBox.Show(
						"Failed to save your exam results. " + Properties.Resources.DataBaseError,
						"Database Write Error: 101 D", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
        private void Exit(object sender, RoutedEventArgs e) //Exit the exam.
		{
			TakeExam TakeExam = new TakeExam();
			TakeExam.Show();
			this.Close();
		}
	}
}
