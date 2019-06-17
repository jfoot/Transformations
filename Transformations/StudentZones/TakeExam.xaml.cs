using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
		private PdfDocument PDF;
        private PdfPage PDFPage;
        private XGraphics Graph;
	
		public TakeExam()
		{
			InitializeComponent();
			if (Properties.Settings.Default.CurrentUser != Properties.Strings.Guest) //If user is logged into their student account
			{
				GuestWarning.Visibility = Visibility.Hidden;
				try
				{
					//Retrieve the number of hard and easy exams the user has taken
					List<KeyValuePair<string, int>> ExamDifficultyData = new List<KeyValuePair<string, int>> //Find the number of hard and easy exams taken
					{
						new KeyValuePair<string, int>("Easy",
							DataBase.Counter(
								"SELECT * FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.Difficulty = 'Easy'")),
						new KeyValuePair<string, int>("Hard",
							DataBase.Counter(
								"SELECT * FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.Difficulty = 'Hard'"))
					};
					ExamDifficulty.DataContext = ExamDifficultyData;

					//Retrieve the data for the type of exam they have taken
					List<KeyValuePair<string, int>> ExamTypeData = new List<KeyValuePair<string, int>>  //find the number of exams taken per topic
					{
						new KeyValuePair<string, int>("Translation",
							DataBase.Counter(
								"SELECT * FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Translation'")),
						new KeyValuePair<string, int>("Reflection",
							DataBase.Counter(
								"SELECT * FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Reflection'")),
						new KeyValuePair<string, int>("Rotation",
							DataBase.Counter(
								"SELECT * FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Rotation'")),
						new KeyValuePair<string, int>("Enlargement",
							DataBase.Counter(
								"SELECT * FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Enlargement'"))
					};
					ExamType.DataContext = ExamTypeData;

					//Retrieve the data for their most recent exam results
					List<KeyValuePair<string, int>> RecentResultsData = new List<KeyValuePair<string, int>> //Find the users most recent exam result in each exam topic.
					{
						new KeyValuePair<string, int>("Translation",
							DataBase.Recent(
								"SELECT ExamResults.Score FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Translation'")),
						new KeyValuePair<string, int>("Reflection",
							DataBase.Recent(
								"SELECT ExamResults.Score FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Reflection'")),
						new KeyValuePair<string, int>("Rotation",
							DataBase.Recent(
								"SELECT ExamResults.Score FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Rotation'")),
						new KeyValuePair<string, int>("Enlargement",
							DataBase.Recent(
								"SELECT ExamResults.Score FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID AND Exams.ExamTopic = 'Enlargement'"))
					};
					RecentResults.DataContext = RecentResultsData;
				}
				catch (Exception)
				{
                    MessageBox.Show(
                        Properties.Strings.FailedToGetExamResults + Properties.Strings.DataBaseError,
                        Properties.Strings.EM_DataBaseReadError + "100 J", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
			}
			else
			{	//If user is a guest blur the graphs as they will be empty.
				BlurEffect myBlurEffect = new BlurEffect {Radius = 10};
				HomeGrid.Effect = myBlurEffect;
                tab_control.SelectedIndex = 1;
            }
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
            Translation_EasyExam exam = new Translation_EasyExam();
            exam.Show();
			this.Close();
		}
		private void TranslationHard(object sender, RoutedEventArgs e) //Start an hard translation exam
        {
			Translation_HardExam exam = new Translation_HardExam();
			exam.Show();
			this.Close();
		}
        private void enlargementEasy(object sender, RoutedEventArgs e) //Start an enlargement easy exam
        {
			Enlargement_EasyExam exam = new Enlargement_EasyExam();
			exam.Show();
			this.Close();
		}
        private void enlargementHard(object sender, RoutedEventArgs e)  //Start an enlargement hard exam
        {
			Enlargement_HardExam exam = new Enlargement_HardExam();
			exam.Show();
			this.Close();
		}
        private void ReflectionEasy(object sender, RoutedEventArgs e)  //Start an reflection easy exam
        {
			Reflection_EasyExam exam = new Reflection_EasyExam();
			exam.Show();
			this.Close();
		}
        private void ReflectionHard(object sender, RoutedEventArgs e)  //Start an reflection hard exam
        {
			Reflection_HardExam exam = new Reflection_HardExam();
			exam.Show();
			this.Close();
		}
        private void RotationHard(object sender, RoutedEventArgs e)    //Start an rotation hard exam
        {
			Rotation_HardExam exam = new Rotation_HardExam();
			exam.Show();
			this.Close();
		}
        private void RotationEasy(object sender, RoutedEventArgs e)    //Start an rotation easy exam
        {
			Rotation_EasyExam exam = new Rotation_EasyExam();
			exam.Show();
			this.Close();
		}
        private void Help(object sender, RoutedEventArgs e)     //Opens the help link
        {
			System.Diagnostics.Process.Start(Properties.Strings.HelpLink);
		}
        private void SavePDF(object sender, RoutedEventArgs e)  //Saves the users exam results into a PDF file.
        {
            if (Properties.Settings.Default.CurrentUser != Properties.Strings.Guest)        //If user is not a guest - Hence has results
            {
                try
                {
                    PDF = new PdfDocument();                    //Create a new PDF Document
                    PDFPage = PDF.AddPage();                    //Add a page to this document
                    Graph = XGraphics.FromPdfPage(PDFPage);     //Creates a graphic (the graphs) to add to the page

                    this.WindowState = WindowState.Maximized;

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog     //Creates a save file dialog box
                    {
                        Filter = "PDF Exam Results|*.pdf",
                        Title = "Save PDF File",
                        FilterIndex = 1
                    };
                    saveFileDialog1.ShowDialog();
                    string pdfFilename = saveFileDialog1.FileName;
        
                    if (saveFileDialog1.FileName != "")         //If the user has entered a save name
                    {
                        XImage image = XImage.FromGdiPlusImage(Properties.Strings.PDFBack);   //Adds the background
                        Graph.DrawImage(image, 0, 0, 612, 792);

                        PDF.Info.Title = "Exam Results";                            //Adds the title to the document
                        XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

                        Graph.DrawString(Properties.Settings.Default.AliasName.ToString() + "'s Exam Results", font, XBrushes.White,
                            new XRect(0, 50, PDFPage.Width.Point, PDFPage.Height.Point), XStringFormats.TopCenter);

                        //Adds the recent results graph
                        XImage image1 = XImage.FromGdiPlusImage((TempImages(RecentResults)));
                        Graph.DrawImage(image1, 96, 90, 404, 200);
                        //Adds the exam difficulty graph
                        XImage image2 = XImage.FromGdiPlusImage((TempImages(ExamDifficulty)));
                        Graph.DrawImage(image2, 96, 310, 404, 200);
                        //Adds the exam types graph
                        XImage image3 = XImage.FromGdiPlusImage((TempImages(ExamType)));
                        Graph.DrawImage(image3, 96, 530, 404, 200);

                        //Saves the PDF and then opens the PDF once completed.
                        PDF.Save(pdfFilename);
                        Process.Start(pdfFilename);

                        MessageBox.Show(Properties.Strings.PDFSaved, Properties.Strings.SaveCompeted,
                            System.Windows.MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        Properties.Strings.PDFFailed + Properties.Strings.CriticalFailuer,
                        Properties.Strings.EM_CriticalFailure + "400 N", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else    //The user was logged in as a guest account.
            {
                MessageBox.Show(
                    Properties.Strings.GuestExportError,
                    Properties.Strings.EM_InvalidRequestError + "301 J", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private Bitmap TempImages(Chart graph)  //Used to take a screen-shoot of the graph to insert into the PDF.
		{
			Rect bounds = VisualTreeHelper.GetDescendantBounds(graph);	//Takes the region around the graph
			double dpi = 96d;
			//Renders the region 
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi,
				System.Windows.Media.PixelFormats.Default);

			DrawingVisual dv = new DrawingVisual();
			using (DrawingContext dc = dv.RenderOpen())
			{
				VisualBrush vb = new VisualBrush(graph);
				dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
			}

			rtb.Render(dv);
			//Saves the rendered image into a bitmap
			MemoryStream stream = new MemoryStream();
			BitmapEncoder encoder = new BmpBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(rtb));
			encoder.Save(stream);

			Bitmap bitmap = new Bitmap(stream);
			//Returns this bitmap to be saved in the PDF
			return bitmap;
		}
	}
}

