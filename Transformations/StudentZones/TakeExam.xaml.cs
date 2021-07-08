using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;

namespace Transformations
{
    /// <summary>
    /// This window allows a student to see their past exam results on a variety of charts and to start a new exam of choice.
    /// </summary>
    public partial class TakeExam : Window
	{
	
		public TakeExam()
		{
			InitializeComponent();
        }

		private void Return(object sender, RoutedEventArgs e)   //Return to the main window
		{
			SplashScreen splash = new SplashScreen("splash_screen.png");
			splash.Show(true, true);

			Transformations.MainWindow mainWindow = new MainWindow();
			mainWindow.Show();
			this.Close();
		}
		private void TranslationEasy(object sender, RoutedEventArgs e) //Start an easy translation exam
		{
            Analytics.TrackEvent("Translation Easy Exam");
            Translation_EasyExam exam = new Translation_EasyExam();
            exam.Show();
			this.Close();
		}
		private void TranslationHard(object sender, RoutedEventArgs e) //Start an hard translation exam
        {
            Analytics.TrackEvent("Translation Hard Exam");
            Translation_HardExam exam = new Translation_HardExam();
			exam.Show();
			this.Close();
		}
        private void enlargementEasy(object sender, RoutedEventArgs e) //Start an enlargement easy exam
        {
            Analytics.TrackEvent("Enlargment Easy Exam");
            Enlargement_EasyExam exam = new Enlargement_EasyExam();
			exam.Show();
			this.Close();
		}
        private void enlargementHard(object sender, RoutedEventArgs e)  //Start an enlargement hard exam
        {
            Analytics.TrackEvent("Enlargment Hard Exam");
            Enlargement_HardExam exam = new Enlargement_HardExam();
			exam.Show();
			this.Close();
		}
        private void ReflectionEasy(object sender, RoutedEventArgs e)  //Start an reflection easy exam
        {
            Analytics.TrackEvent("Reflection Easy Exam");
            Reflection_EasyExam exam = new Reflection_EasyExam();
			exam.Show();
			this.Close();
		}
        private void ReflectionHard(object sender, RoutedEventArgs e)  //Start an reflection hard exam
        {
            Analytics.TrackEvent("Reflection Hard Exam");
            Reflection_HardExam exam = new Reflection_HardExam();
			exam.Show();
			this.Close();
		}
        private void RotationHard(object sender, RoutedEventArgs e)    //Start an rotation hard exam
        {
            Analytics.TrackEvent("Rotation Hard Exam");
            Rotation_HardExam exam = new Rotation_HardExam();
			exam.Show();
			this.Close();
		}
        private void RotationEasy(object sender, RoutedEventArgs e)    //Start an rotation easy exam
        {
            Analytics.TrackEvent("Rotation Easy Exam");
            Rotation_EasyExam exam = new Rotation_EasyExam();
			exam.Show();
			this.Close();
		}
        private void Help(object sender, RoutedEventArgs e)     //Opens the help link
        {
            Analytics.TrackEvent("Help Clicked");
            System.Diagnostics.Process.Start(Properties.Strings.HelpLink);
		}
      
	}
}

