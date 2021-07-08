﻿using Microsoft.AppCenter.Analytics;
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
                PassOrFail.Content = Properties.Strings.Fail;
				PassOrFail.Foreground = new SolidColorBrush(Colors.Red);
				Pass = false;
			}

            //Without storing personally identifiable data track general user exam performance to assess if they are too hard or easy.
            Analytics.TrackEvent("Completed Exam", new System.Collections.Generic.Dictionary<string, string> {
                    { "ExamID",  Result.ExamID.ToString() },
                    { "Score",  Result.ScoreValue.ToString()},
                    { "Attempts", Result.TotalAttempts.ToString() },
                    { "Time", time.Content.ToString() },
                    { "Pass", Pass.ToString() }
            });
        }
        private void Exit(object sender, RoutedEventArgs e) //Exit the exam.
		{
			TakeExam TakeExam = new TakeExam();
			TakeExam.Show();
			this.Close();
		}
	}
}
