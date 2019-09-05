using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;


namespace Transformations
{
    public partial class MainWindow
	{
		/// <summary>
		///             Menu items
		/// Contains all the code for the menu bar along the top including:
        /// - Restarting
        /// - Giving Feedback
        /// - Opening about window
        /// - Giving the user help
        /// - Opening the settings window
        /// - Resting the axis position
        /// -Turning on and off labels
        /// -Turning on and off grid snapping
        /// -Open a project file from start up
        /// -Open a project file in program
        /// -The opening function
        /// -Saving an image
        /// -Saving the file function
        /// -Login to a student and teacher account
        /// -Launching the exam window and the student manager.
		/// </summary>
	
		private void RestartClick(object sender, RoutedEventArgs e)    //If the user wants to restart the program.
		{
            Analytics.TrackEvent("Program Restarted");
            Process.Start(Application.ResourceAssembly.Location);
			Application.Current.Shutdown();
		}
		private void ReportBug(object sender, RoutedEventArgs e)    //If the user wants to restart the program.
		{
            Analytics.TrackEvent("Bug Reported");
            Process.Start(Properties.Strings.BugReport);
        }
		private void FeedbackClick(object sender, RoutedEventArgs e)   //If the user wants to provide feedback
		{
            Analytics.TrackEvent("Feedback Sent");
            Process.Start(Properties.Strings.FeedbackLink);
		}
       
        private void HelpClick(object sender, RoutedEventArgs e)       //If the user needs help
		{
            Analytics.TrackEvent("Help Link Clicked");
            Process.Start(Properties.Strings.HelpLink);
		}
        private void SettingsClick(object sender, RoutedEventArgs e) //Open Settings
		{
            Analytics.TrackEvent("Settings Window Opened");
            Settings settings = new Settings {Owner = this};
			settings.Show();
		}
        private void ResetAxisClick(object sender, RoutedEventArgs e) //Reset the Axis position
		{
            Analytics.TrackEvent("Axis Reset");
            XSlider.Value = 0;
			YSlider.Value = 0;
			TranslationTransformCanvas.X = 0;
			TranslationTransformCanvas.Y = 0;
			Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider, sliderSf, border);
		}
        private void LabelsChecked(object sender, RoutedEventArgs e) //Turn on Labels
		{
            Analytics.TrackEvent("Turn On Labels");

            try
            {
                Grid.Labels.ForEach(o => MyCanvas.Children.Add(o));
            }
            catch (Exception) { }
		}
        private void LabelsUnchecked(object sender, RoutedEventArgs e) //Turn off labels
		{
            Analytics.TrackEvent("Turn Off Labels");

            try
            {
                Grid.Labels.ForEach(o => MyCanvas.Children.Remove(o));
            }
            catch (Exception) { }
		}
        private void GridSnapState(object sender, RoutedEventArgs e)   //Grid snapping- snaps all of the shapes to the grid.
        {
            Analytics.TrackEvent("Turn On Grid Snapping");

            if (GridSnap.IsChecked == true)
            {
                foreach (Shapes t in MyShapes)
                {
                    double newleft = (Round.ToNearest((Canvas.GetLeft(t.MyShape)), (ScaleFactor)));
                    Canvas.SetLeft(t.MyShape, (newleft));

                    double newtop = (Round.ToNearest((Canvas.GetTop(t.MyShape)), (ScaleFactor)));
                    Canvas.SetTop(t.MyShape, (newtop));
                }
            }
            if (SelectedShape != null)
            {
                HighlightDetials();
            }
        }
        private void OpenClick(object sender, RoutedEventArgs e) //Select files to open using a dialog window.
		{
            Analytics.TrackEvent("Open File Click");

            try
            {
				MessageBoxResult open = MessageBox.Show(Properties.Strings.SaveBeforeOpening, Properties.Strings.AreYouSure,
					System.Windows.MessageBoxButton.YesNoCancel,
					MessageBoxImage.Information);

				if (open == MessageBoxResult.Yes)
				{
					SaveFileClick(this, new RoutedEventArgs());
				}
				else if (open == MessageBoxResult.Cancel)
				{
					return;
				}
				MyCanvas.Children.Clear();    //Empty the original canvas and redraw it- similar to restarting the program

				Grid = new GridLine().DrawGrid(MaxValue, ScaleFactor, MyCanvas);
				LabelsChecked(sender, e);
				ResetAxisClick(this, new RoutedEventArgs());
                //Open the file dialog box
				OpenFileDialog openFileDialog1 = new OpenFileDialog
				{
					Filter = "Shape Collection|*.shape",
					Title = "Open a project File",
					RestoreDirectory = true
				};

				openFileDialog1.ShowDialog();

				if (openFileDialog1.FileName != "")     //if the file has a name
				{
					OpenFunction(openFileDialog1.FileName);
				}
			}
			catch (Exception)   //Corrupted file format.
			{
				Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider, sliderSf, border);
                MessageBox.Show(Properties.Strings.UnableToOpen +  Properties.Strings.CriticalFailuer, Properties.Strings.EM_CriticalFailure + "400 D",
                    System.Windows.MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Process.Start(Application.ResourceAssembly.Location);   
				Application.Current.Shutdown();
			}
		}
		private void OpenFunction(string file) //Open the files selected into the project
		{
            Analytics.TrackEvent("Open File");

            try
            {   //Reads the file to open and then splits it a readable format
				string shapes = System.IO.File.ReadAllText(file);
				string[] shapeGroups = shapes.Split('!');
				string[] polygonShapes = shapeGroups[0].Split('*');
				string[] ellipsesShapes = shapeGroups[1].Split('*');

                //Imports all polygons into the program
				for (int i = 0; i < polygonShapes.Length - 1; i++)
				{
					Polygon rootElement = (Polygon) XamlReader.Parse(polygonShapes[i]);
					MyShapes.Add(new FreeForm());
					MyShapes[MyShapes.Count - 1].MyShape = rootElement;
					
					MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
					MyCanvas.Children.Add(MyShapes[MyShapes.Count - 1].MyShape);
				}

                //Import all circles into the program
				for (int i = 0; i < ellipsesShapes.Length - 1; i++)
				{
					Ellipse rootElement = (Ellipse) XamlReader.Parse(ellipsesShapes[i]);
					MyShapes.Add(new Circle());
					MyShapes[MyShapes.Count - 1].MyShape = rootElement;


					MyShapes[MyShapes.Count - 1].MyShape.MouseLeftButtonDown += new MouseButtonEventHandler(MyPolygonMouseDown);
					MyCanvas.Children.Add(MyShapes[MyShapes.Count - 1].MyShape);
				}
                //Refreshes the scaling/ screen
				Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider, sliderSf, border);
			}
			catch (Exception) //corrupt file
			{
                MessageBox.Show(Properties.Strings.UnableToOpen + Properties.Strings.CriticalFailuer, Properties.Strings.EM_CriticalFailure + "400 E",
                    System.Windows.MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Process.Start(Application.ResourceAssembly.Location);
				Application.Current.Shutdown();
			}
			finally
			{
                MyShapes.ForEach(o => Canvas.SetZIndex(o.MyShape, 2));
			}
		}
        private void SaveImageClick(object sender, RoutedEventArgs e) //Save an image of the current project
		{
            Analytics.TrackEvent("Save Image File");

            try
            {
				SaveFileDialog saveFile = new SaveFileDialog //Open a save file window
				{
					Filter = "PNG Image|*.png",
					Title = "Save an Image File",
					FilterIndex = 2
				};
				saveFile.ShowDialog();

                if (saveFile.FileName != "")
				{
					for (int x = 0; x < 2; x++)
					{
                        //Save a screen shoot of the current canvas.
                        System.IO.FileStream fs = (FileStream) saveFile.OpenFile();
;
						double dpi = 96d;
						RenderTargetBitmap rtb = new RenderTargetBitmap((int) border.ActualWidth, (int) border.ActualHeight, dpi, dpi,
							System.Windows.Media.PixelFormats.Default);

						DrawingVisual dv = new DrawingVisual();
						using (DrawingContext dc = dv.RenderOpen())
						{
							VisualBrush vb = new VisualBrush(border);
							dc.DrawRectangle(vb, null, new Rect(new Point(), border.RenderSize));
						}

						rtb.Render(dv);
						BitmapEncoder encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(rtb));
						encoder.Save(fs);
						fs.Close();

					}
                    MessageBox.Show(Properties.Strings.SaveSuccesful, Properties.Strings.SaveCompeted, System.Windows.MessageBoxButton.OK,
                           MessageBoxImage.Information);
                }
			}
			catch (Exception)
			{
                MessageBox.Show(Properties.Strings.FailedToSave + Properties.Strings.CriticalFailuer, Properties.Strings.EM_CriticalFailure + "400 F",
                    System.Windows.MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
		}
		private void SaveFileClick(object sender, RoutedEventArgs e) //Save a file containing the current project
		{
            Analytics.TrackEvent("Save Shape File");

            try
            {
				SaveFileDialog save = new SaveFileDialog
				{
					Filter = "Shape Collection|*.shape",
					Title = "Save an project file"
				};
				save.ShowDialog();


				if (save.FileName != "")
				{
					StreamWriter writer = new StreamWriter(save.OpenFile());
					foreach (Shapes t in MyShapes)  //Saves each polygon shape
					{
						if (t.MyShape.GetType().ToString() == "System.Windows.Shapes.Polygon" && !t.MyShape.Name.StartsWith("dupe"))
						{
							SaveFile = SaveFile + XamlWriter.Save(t.MyShape).ToString() + "*";
						}
					}
                    //Splits the file using a "!"
					SaveFile += "!";


					foreach (Shapes t in MyShapes)  //Saves each circle shape
					{
						if (t.MyShape.GetType().ToString() == "System.Windows.Shapes.Ellipse" && !t.MyShape.Name.StartsWith("dupe"))
						{
							SaveFile = SaveFile + XamlWriter.Save(t.MyShape).ToString() + "*";
						}
					}
                    //Saves file 
					writer.WriteLine(SaveFile);
					writer.Close();

					MessageBox.Show(Properties.Strings.SaveSuccesful, Properties.Strings.SaveCompeted, System.Windows.MessageBoxButton.OK,
						MessageBoxImage.Information);
				}
			}
			catch (Exception)
			{
                MessageBox.Show(Properties.Strings.FailedToSaveShape + Properties.Strings.CriticalFailuer, Properties.Strings.EM_CriticalFailure + "400 G",
                    System.Windows.MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
		}
        private void StudentLoginClick(object sender, RoutedEventArgs e) //Student Login to the program
		{
            Analytics.TrackEvent("Student Login");

            if (Properties.Settings.Default.CurrentUser == Properties.Strings.Guest) //If the user is a guest (currently logged out)
			{
				try
				{
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
                        using (var command = new OleDbCommand("SELECT [ID], [UserName], [ClassID], [AliasName]  FROM  Users", conn))
                        {
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[1].ToString() == System.Environment.UserName)        //If username matches a known username on the database then log them in
                                    {
                                        Properties.Settings.Default.UserID = Convert.ToInt32(reader[0]);
                                        Properties.Settings.Default.CurrentUser = reader[1].ToString();
                                        Properties.Settings.Default.ClassID = Convert.ToInt32(reader[2]);
                                        Properties.Settings.Default.AliasName = reader[3].ToString();
                                        Properties.Settings.Default.IsTeacher = false;

                                        Properties.Settings.Default.Save();
                                    }
                                }
                            }
                        }
                    }


					if (Properties.Settings.Default.CurrentUser == Properties.Strings.Guest) //If the user could not log in then they have no account.
					{
						MessageBox.Show(
							Properties.Strings.NoAccountFound,
							Properties.Strings.AccountNotFound, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

						CreateAccount account = new CreateAccount {Owner = this};   //They are asked to create a new account.
						account.Show();
					}
					else
					{
						MessageBox.Show(
							Properties.Strings.LoginSuccesfull,
							Properties.Strings.AccountFound, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                        
						Login.Header = Properties.Strings.StudentLogOutDrop;
						accountName.Content = Properties.Settings.Default.AliasName;
					}
				}
				catch (Exception ex)
				{
                    Crashes.TrackError(ex);
                    MessageBox.Show(Properties.Strings.FailToGetLoginAccounts + Properties.Strings.DataBaseError , Properties.Strings.EM_DataBaseReadError + "100 F",
                    System.Windows.MessageBoxButton.OK,
						MessageBoxImage.Error);
                }
				
			}
			else if (Properties.Settings.Default.IsTeacher == true)
			{
				MessageBox.Show(
					Properties.Strings.LoggedInAsTeacherError,
                    Properties.Strings.EM_InvalidRequestError + "301 F", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
			    accountName.Content = Properties.Strings.Guest;
                Login.Header = Properties.Strings.StudentLoginDrop;
				Properties.Settings.Default.CurrentUser = Properties.Strings.Guest;
				Properties.Settings.Default.UserID = 0;
				Properties.Settings.Default.ClassID = 0;
				Properties.Settings.Default.AliasName = Properties.Strings.Guest;
				Properties.Settings.Default.IsTeacher = false;

				Properties.Settings.Default.Save();
                MessageBox.Show(Properties.Strings.logoutSuccessful,
                    Properties.Strings.Loggedout, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}

		}
        private void TeacherLoginClick(object sender, RoutedEventArgs e) //Login into the teacher screen
		{
            Analytics.TrackEvent("Teacher Login");

            if (Properties.Settings.Default.CurrentUser == Properties.Strings.Guest)
			{
                try
                {
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
                        using (var command = new OleDbCommand("SELECT [ID], [UserName], [AliasName]  FROM  Teachers", conn))
                        {
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[1].ToString() == System.Environment.UserName)    //If the user name matches a known user name on the database then log them in.
                                    {
                                        Properties.Settings.Default.UserID = Convert.ToInt32(reader[0]);
                                        Properties.Settings.Default.CurrentUser = reader[1].ToString();
                                        Properties.Settings.Default.AliasName = reader[2].ToString();
                                        Properties.Settings.Default.IsTeacher = true;
                                        Properties.Settings.Default.Save();
                                    }
                                }
                            }
                        }
                    }


                    if (Properties.Settings.Default.CurrentUser == Properties.Strings.Guest) //If they were not logged in then they do not have an account and will be asked to make one.
                    {
                        MessageBox.Show(
                            Properties.Strings.NoAccountFound,
                            Properties.Strings.AccountNotFound, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

                        CreateTeacherAccount account = new CreateTeacherAccount() { Owner = this };
                        account.Show();
                    }
                    else
                    {
                        MessageBox.Show(
                            Properties.Strings.LoginSuccesfull,
                            Properties.Strings.AccountFound, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                        Teacher.Header = Properties.Strings.TeacherLogOutDrop;
                        accountName.Content = Properties.Settings.Default.AliasName;
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    MessageBox.Show(Properties.Strings.FailToGetLoginAccounts + Properties.Strings.DataBaseError, Properties.Strings.EM_DataBaseReadError + "100 G",
                        System.Windows.MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
           	}
			else if (Properties.Settings.Default.IsTeacher == false)
			{
				MessageBox.Show(
					Properties.Strings.LogOutOfStudentFirst,
					Properties.Strings.EM_InvalidRequestError + "301 G", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else //Else they can log out 
			{
				accountName.Content = Properties.Strings.Guest;
				Teacher.Header = Properties.Strings.TeacherLoginDrop;
				Properties.Settings.Default.CurrentUser = Properties.Strings.Guest;
				Properties.Settings.Default.UserID = 0;
				Properties.Settings.Default.ClassID = 0;
				Properties.Settings.Default.AliasName = Properties.Strings.Guest;
				Properties.Settings.Default.IsTeacher = false;

				Properties.Settings.Default.Save();
				MessageBox.Show(
					Properties.Strings.logoutSuccessful,
					Properties.Strings.Loggedout, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
        private void StudentManagerClick(object sender, RoutedEventArgs e)
		{
            Analytics.TrackEvent("Student Manager Click");

            if (Properties.Settings.Default.IsTeacher == true)  //If they are a teacher and logged in then open the class editor
			{
                ClassEditor teacherZone = new ClassEditor {  Owner = this };
                teacherZone.Show();
			}
			else
			{
				MessageBox.Show(
				    Properties.Strings.LogOutOfStudentOrGuest,
					Properties.Strings.EM_InvalidRequestError + "301 H", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
        private void TakeExamClick(object sender, RoutedEventArgs e) //Open the student portal/ exam taker
		{
            Analytics.TrackEvent("Take Exam Click");


            if (Properties.Settings.Default.CurrentUser == Properties.Strings.Guest)
			{
				MessageBox.Show(
					Properties.Strings.NotLoggedIn,
					Properties.Strings.CurrentlyUsingGuest, System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			if (Properties.Settings.Default.IsTeacher == true)  //Teachers can not launch the exam zone.
			{
				MessageBox.Show(
					Properties.Strings.LogOutOfTeacher,
					Properties.Strings.EM_InvalidRequestError + "301 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				SplashScreen splash = new SplashScreen("splash_screen.png");
				splash.Show(true, true);
				
				Transformations.TakeExam takeExam = new TakeExam();
				takeExam.Show();
				this.Close();
			}
		}

        private void TroubleShoot_Click(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent("TroubleShoot Click");

            Troubleshooting tb = new Troubleshooting();
            tb.Show();
            this.Close();
        }
    }
}