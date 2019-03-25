using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
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
			Process.Start(Application.ResourceAssembly.Location);
			Application.Current.Shutdown();
		}
		private void ReportBug(object sender, RoutedEventArgs e)    //If the user wants to restart the program.
		{
			System.Diagnostics.Process.Start(LocalizationProvider.GetLocalizedValue<string>("BugReport"));

        }
		private void FeedbackClick(object sender, RoutedEventArgs e)   //If the user wants to provide feedback
		{
			System.Diagnostics.Process.Start(LocalizationProvider.GetLocalizedValue<string>("FeedbackLink"));
		}
       
        private void HelpClick(object sender, RoutedEventArgs e)       //If the user needs help
		{
			System.Diagnostics.Process.Start(LocalizationProvider.GetLocalizedValue<string>("HelpLink"));
		}
        private void SettingsClick(object sender, RoutedEventArgs e) //Open Settings
		{
			Settings settings = new Settings {Owner = this};
			settings.Show();
		}
        private void ResetAxisClick(object sender, RoutedEventArgs e) //Reset the Axis position
		{
			XSlider.Value = 0;
			YSlider.Value = 0;
			TranslationTransformCanvas.X = 0;
			TranslationTransformCanvas.Y = 0;
			Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider, sliderSf, border);
		}
        private void LabelsChecked(object sender, RoutedEventArgs e) //Turn on Labels
		{
			try
			{
				foreach (Label t in Grid.Labels)
				{
					MyCanvas.Children.Add(t);
				}
			}
			catch (Exception)
			{
		
			}
		}
        private void LabelsUnchecked(object sender, RoutedEventArgs e) //Turn off labels
		{
			try
			{
				foreach (Label t in Grid.Labels)
				{
					MyCanvas.Children.Remove(t);
				}
			}
			catch (Exception)
			{
			}
		}
        private void GridSnapState(object sender, RoutedEventArgs e)   //Grid snapping- snaps all of the shapes to the grid.
        {
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
        private void ProgramLoaded(object sender, RoutedEventArgs e)    //When the program loads open a file if the program launched from a file.
		{
			Labels.IsChecked = true;
			if ((((App) Application.Current).file) != null)
			{
				OpenFunction(((App) Application.Current).file_path);
			}
		}
        private void OpenClick(object sender, RoutedEventArgs e) //Select files to open using a dialog window.
		{
			try
			{
				MessageBoxResult open = MessageBox.Show("Do you wish to save before opening a new file?", "Are you sure?",
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
				MessageBox.Show("Unable to open file. The selected file could be corrupted or not compatible with the current version of this program. " + LocalizationProvider.GetLocalizedValue<string>("CriticalFailuer"), "Critical Program Failure: 400 D",
					System.Windows.MessageBoxButton.OK,
					MessageBoxImage.Error);
				Process.Start(Application.ResourceAssembly.Location);   
				Application.Current.Shutdown();
			}
		}
		private void OpenFunction(string file) //Open the files selected into the project
		{
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
				MessageBox.Show("Unable to open file. The selected file could be corrupted or not compatible with the current version of this program. " + LocalizationProvider.GetLocalizedValue<string>("CriticalFailuer"), "Critical Program Failure: 400 E",
					System.Windows.MessageBoxButton.OK,
					MessageBoxImage.Error);
				Process.Start(Application.ResourceAssembly.Location);
				Application.Current.Shutdown();
			}
			finally
			{
				foreach (Shapes t in MyShapes)  //make every shape on top of the canvas
				{
					Canvas.SetZIndex(t.MyShape, 2);
				}
			}
		}
        private void SaveImageClick(object sender, RoutedEventArgs e) //Save an image of the current project
		{
			try
			{
				SaveFileDialog saveFile = new SaveFileDialog //Open a save file window
				{
					Filter = "PNG Image - Transparent Background|*.png| PNG Image - White Background|*.png",
					Title = "Save an Image File",
					FilterIndex = 2
				};
				saveFile.ShowDialog();
                //Change background color accordingly 
				if (saveFile.FilterIndex == 1)
				{
					border.Background = new SolidColorBrush(Colors.Transparent);
				}
				else if (saveFile.FilterIndex == 2)
				{
					border.Background = new SolidColorBrush(Colors.White);
				}

				if (saveFile.FileName != "")
				{
					for (int x = 0; x < 2; x++)
					{
                        //Save a screen shoot of the current canvas.
						System.IO.FileStream fs = (System.IO.FileStream) saveFile.OpenFile();

						Rect bounds = VisualTreeHelper.GetDescendantBounds(MyCanvas);
						double dpi = 96d;

						RenderTargetBitmap rtb = new RenderTargetBitmap((int) bounds.Width, (int) bounds.Height, dpi, dpi,
							System.Windows.Media.PixelFormats.Default);

						DrawingVisual dv = new DrawingVisual();
						using (DrawingContext dc = dv.RenderOpen())
						{
							VisualBrush vb = new VisualBrush(border);
							dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
						}

						rtb.Render(dv);
						BitmapEncoder encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(rtb));
						encoder.Save(fs);
						fs.Close();

					}
					MessageBox.Show("The file save successful.", "Save Completed", System.Windows.MessageBoxButton.OK,
						MessageBoxImage.Information);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Failed to save the image. " + LocalizationProvider.GetLocalizedValue<string>("CriticalFailuer"), "Critical Program Failure: 400 F",
					System.Windows.MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}
		private void SaveFileClick(object sender, RoutedEventArgs e) //Save a file containing the current project
		{
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

					MessageBox.Show("The file save successful.", "Save Completed", System.Windows.MessageBoxButton.OK,
						MessageBoxImage.Information);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Failed to save the shape file. " + LocalizationProvider.GetLocalizedValue<string>("CriticalFailuer"), "Critical Program Failure: 400 G",
					System.Windows.MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}
        private void StudentLoginClick(object sender, RoutedEventArgs e) //Student Login to the program
		{
			if (Properties.Settings.Default.CurrentUser == "Guest") //If the user is a guest (currently logged out)
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


					if (Properties.Settings.Default.CurrentUser == "Guest") //If he user could not log in then they have no account.
					{
						MessageBox.Show(
							"No user account could be found, please make a new user account.",
							"Account Not Found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

						CreateAccount account = new CreateAccount {Owner = this};   //They are asked to create a new account.
						account.Show();
					}
					else
					{
						MessageBox.Show(
							"Login Successful, you have now been logged into your account.",
							"Account Found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                        
						Login.Header = "Student Log Out";
						accountName.Content = Properties.Settings.Default.AliasName;
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Failed to retrieve user login details and accounts. " + LocalizationProvider.GetLocalizedValue<string>("DataBaseError"), "Database Read Error: 100 F",
						System.Windows.MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
				
			}
			else if (Properties.Settings.Default.IsTeacher == true)
			{
				MessageBox.Show(
					"You are currently logged in as a teacher, please first log out of teacher login, before trying to log in into the student login.",
					"Invalid Request Error: 301 F", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				accountName.Content = "Guest";
				Login.Header = "Student Login";
				Properties.Settings.Default.CurrentUser = "Guest";
				Properties.Settings.Default.UserID = 0;
				Properties.Settings.Default.ClassID = 0;
				Properties.Settings.Default.AliasName = "Guest";
				Properties.Settings.Default.IsTeacher = false;

				Properties.Settings.Default.Save();
				MessageBox.Show(
					"You have been Successful logged out.",
					"Logged out.", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}

		}
        private void TeacherLoginClick(object sender, RoutedEventArgs e) //Login into the teacher screen
		{
			if (Properties.Settings.Default.CurrentUser == "Guest")
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
                                    if (reader[1].ToString() == System.Environment.UserName)    //If the username matches a known username on the database then log them in.
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


                    if (Properties.Settings.Default.CurrentUser == "Guest") //If they were not logged in then they do not have an account and will be asked to make one.
                    {

                        MessageBox.Show(
                            "No user account could be found, please make a new user account.",
                            "Account Not Found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

                        CreateTeacherAccount account = new CreateTeacherAccount() { Owner = this };
                        account.Show();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Login Successful, you have now been logged into your account.",
                            "Account Found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                        Teacher.Header = "Teacher Log Out";
                        accountName.Content = Properties.Settings.Default.AliasName;


                    }
                }
                catch (Exception)
                {
					MessageBox.Show("Failed to retrieve Teacher login details and accounts. " + LocalizationProvider.GetLocalizedValue<string>("DataBaseError"), "Database Read Error: 100 G",
						System.Windows.MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
           	}
			else if (Properties.Settings.Default.IsTeacher == false)
			{
				MessageBox.Show(
					"You are currently logged in as a student, please first log out of student login, before trying to log in into the teacher login.",
					"Invalid Request Error: 301 G", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else //Else they can log out 
			{
				accountName.Content = "Guest";
				Teacher.Header = "Teacher Login";
				Properties.Settings.Default.CurrentUser = "Guest";
				Properties.Settings.Default.UserID = 0;
				Properties.Settings.Default.ClassID = 0;
				Properties.Settings.Default.AliasName = "Guest";
				Properties.Settings.Default.IsTeacher = false;

				Properties.Settings.Default.Save();
				MessageBox.Show(
					"You have been Successful logged out.",
					"Logged out.", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
        private void StudentManagerClick(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.IsTeacher == true)  //If they are a teacher and logged in then open the class editor
			{
                ClassEditor teacherZone = new ClassEditor {  Owner = this };
                teacherZone.Show();
			}
			else
			{
				MessageBox.Show(
					"You are currently logged in as a student or guest, please login to the teacher login.",
					"Invalid Request Error: 301 H", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
        private void TakeExamClick(object sender, RoutedEventArgs e) //Open the student portal/ exam taker
		{
			if (Properties.Settings.Default.CurrentUser == "Guest")
			{
				MessageBox.Show(
					"You are currently not logged in. No progress will be saved or recorded while using a Guest login.",
					"Currently using a guest account.", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			if (Properties.Settings.Default.IsTeacher == true)  //Teachers can not launch the exam zone.
			{
				MessageBox.Show(
					"You are currently logged in as a teacher, please log out of a teacher account and login into a student account to access exams.",
					"Invalid Request Error: 301 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
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
	}
}