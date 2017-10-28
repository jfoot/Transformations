using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Transformations.Properties;
using Color = System.Drawing.Color;

namespace Transformations
{
	/// <summary>
	/// Interaction logic for Window2.xaml
	/// </summary>
	public partial class Settings : Window
	{
		public Settings()
		{
			InitializeComponent();
		}

		private void Load(object sender, RoutedEventArgs e) //Called when the window loads
		{
            if (Properties.Settings.Default.CurrentUser == "Guest")
			{
				DeleteAccount.Visibility = Visibility.Hidden;
				ChangeClass.Visibility = Visibility.Hidden;
			}

            //Changes the settings window depending upon the type of user account.
			userIDLabel.Content = Properties.Settings.Default.UserID.ToString();
			usernameLabel.Content = Properties.Settings.Default.CurrentUser.ToString();
			ClassID.Content = Properties.Settings.Default.ClassID.ToString();
			AliasName.Content = Properties.Settings.Default.AliasName;
			if (Properties.Settings.Default.IsTeacher == true)
			{
				AccountType.Content = "Teacher Account";
				TechDisc.Visibility = Visibility.Hidden;
				TeacherName.Visibility = Visibility.Hidden;
				ClassDisc.Visibility = Visibility.Hidden;
				ClassName.Visibility = Visibility.Hidden;
				ClassIDDisc.Visibility = Visibility.Hidden;
				ClassID.Visibility = Visibility.Hidden;
				ChangeClass.Visibility = Visibility.Hidden;
			}
			Version.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			try
			{
				if (Properties.Settings.Default.CurrentUser != "Guest")
				{
					using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
					{
						conn.Open();
						using (var command =
							new OleDbCommand(
								"SELECT [ClassName], [Teachers.AliasName]  FROM  Class INNER JOIN Teachers ON Class.TeacherID = Teachers.ID WHERE [Class.ID] = @ID",
								conn))
						{   //Select the class name and teacher name for the student account.
							command.Parameters.AddWithValue("@ID", Properties.Settings.Default.ClassID);
							using (OleDbDataReader reader = command.ExecuteReader())
							{
								while (reader.Read())
								{
									ClassName.Content = reader[0].ToString();
									TeacherName.Content = reader[1].ToString();
								}
							}
						}
					}
				}

			}
			catch (Exception)
			{
				MessageBox.Show(
					"Failed to retrieve your teacher and class. " + Properties.Resources.DataBaseError,
					"Database Read Error: 100 H", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
			}



			if(Properties.Settings.Default.CheckForUpdates == true)
			{
				CheckForUpdates.IsChecked = true;
			}


			//Shape Colour
			if (Properties.Settings.Default.DefaultColour == Color.Blue)
			{
				blue_shape.IsChecked = true;
				shape_example.Fill = Brushes.Blue;
			}
			else if (Properties.Settings.Default.DefaultColour == Color.Red)
			{
				red_shape.IsChecked = true;
				shape_example.Fill = Brushes.Red;
			}
			else if (Properties.Settings.Default.DefaultColour == Color.Green)
			{
				green_shape.IsChecked = true;
				shape_example.Fill = Brushes.Green;
			}
			else if (Properties.Settings.Default.DefaultColour == Color.Black)
			{
				black_shape.IsChecked = true;
				shape_example.Fill = Brushes.Black;
			}

			//Grid Colour
			if (Properties.Settings.Default.DefaultGridColour == Color.LavenderBlush)
			{
				light_gray.IsChecked = true;
			}
			else if (Properties.Settings.Default.DefaultGridColour == Color.LightPink)
			{
				red.IsChecked = true;
			}
			else if (Properties.Settings.Default.DefaultGridColour == Color.LightGray)
			{
				dark_gray.IsChecked = true;
			}
			else if (Properties.Settings.Default.DefaultGridColour == Color.DarkSlateGray)
			{
				black_dark_gray.IsChecked = true;
			}
			else if (Properties.Settings.Default.DefaultGridColour == Color.Gray)
			{
				black_light_gray.IsChecked = true;
			}
			else if (Properties.Settings.Default.DefaultGridColour == Color.GhostWhite)
			{
				black_white.IsChecked = true;
			}



			//Resolution Setting
			if (Properties.Settings.Default.DefaultResolution == false)    //Low res
			{
				low_res.IsChecked = true;
				high_res.IsChecked = false;
			}
			else if (Properties.Settings.Default.DefaultResolution == true)
			{
				high_res.IsChecked = true;
				low_res.IsChecked = false;
			}

			//Performance Setting
			if (Properties.Settings.Default.DefaultPerformance == true)    //High performance
			{
				high_prof.IsChecked = true;
				low_prof.IsChecked = false;
			}
			else if (Properties.Settings.Default.DefaultPerformance == false)
			{
				high_prof.IsChecked = false;
				low_prof.IsChecked = true;
			}

			//DataLocation
			if (Properties.Settings.Default.DatalocDefault == false)
			{
				data_defualt.IsChecked = true;
			}
			else if (Properties.Settings.Default.DatalocDefault == true)
			{
				data_custom.IsChecked = true;
			}

			size_slider.Value = (Properties.Settings.Default.DefaultHeight) / (75 / 5);
			size_label.Content = "Default Shape Size: " + size_slider.Value.ToString();
		}
        private void BlueShapeChecked(object sender, RoutedEventArgs e)   //Blue colour checked
		{
			shape_example.Fill = Brushes.Blue;
		}
		private void RedShapeChecked(object sender, RoutedEventArgs e) //Red colour checked
        {
			shape_example.Fill = Brushes.Red;
		}
		private void GreenShapeChecked(object sender, RoutedEventArgs e) //Green colour checked
        {
			shape_example.Fill = Brushes.Green;
		}
		private void BlackShapeChecked(object sender, RoutedEventArgs e)//Black colour checked
        {
			shape_example.Fill = Brushes.Black;
		}
        private void ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)   //Called upon when the size of shape slider is changed 
		{
			if (size_slider.IsLoaded)
			{
				size_label.Content = "Default Shape Size: " + size_slider.Value.ToString();
			}
		}
        private void ResetChanges(object sender, RoutedEventArgs e) //Reset settings to default button.
		{
			Properties.Settings.Default.Reset();
			this.Close();
			MessageBox.Show("Your settings have been reset back to defaults successfully. The application will now restart.");

			Process.Start(Application.ResourceAssembly.Location);
			Application.Current.Shutdown();
		}
        private void ApplyChanges(object sender, RoutedEventArgs e) //Apply setting changes 
		{
			bool valid = true;

			//Shape Colours
			if (blue_shape.IsChecked == true)
			{
				Properties.Settings.Default.DefaultColour = Color.Blue;
			}
			else if (red_shape.IsChecked == true)
			{
				Properties.Settings.Default.DefaultColour = Color.Red;
			}
			else if (green_shape.IsChecked == true)
			{
				Properties.Settings.Default.DefaultColour = Color.Green;
			}
			else if (black_shape.IsChecked == true)
			{
				Properties.Settings.Default.DefaultColour = Color.Black;
			}

			//Grid Colour
			if (light_gray.IsChecked == true)
			{
				Properties.Settings.Default.DefaultGridColour = Color.LavenderBlush;
				Properties.Settings.Default.DarkMode = false;
			}
			else if (dark_gray.IsChecked == true)
			{
				Properties.Settings.Default.DefaultGridColour = Color.LightGray;
				Properties.Settings.Default.DarkMode = false;
			}
			else if (red.IsChecked == true)
			{
				Properties.Settings.Default.DefaultGridColour = Color.LightPink;
				Properties.Settings.Default.DarkMode = false;
			}
			else if (black_dark_gray.IsChecked == true)
			{
				Properties.Settings.Default.DefaultGridColour = Color.DarkSlateGray;
				Properties.Settings.Default.DarkMode = true;
			}
			else if (black_light_gray.IsChecked == true)
			{
				Properties.Settings.Default.DefaultGridColour = Color.Gray;
				Properties.Settings.Default.DarkMode = true;
			}
			else if (black_white.IsChecked == true)
			{
				Properties.Settings.Default.DefaultGridColour = Color.GhostWhite;
				Properties.Settings.Default.DarkMode = true;
			}



			//Resolution Setting
			if (low_res.IsChecked == true)
			{
				Properties.Settings.Default.DefaultResolution = false;
			}
			else if (high_res.IsChecked == true)
			{
				Properties.Settings.Default.DefaultResolution = true;
			}

			//Performance Setting
			if (high_prof.IsChecked == true)
			{
				Properties.Settings.Default.DefaultPerformance = true;
			}
			else if (low_prof.IsChecked == true)
			{
				Properties.Settings.Default.DefaultPerformance = false;
			}

			//DataBase Location
			if (data_custom.IsChecked == true)
			{
				Properties.Settings.Default.DatalocDefault = true;
				System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
				try
				{
					conn.ConnectionString = connectionstring.Text;
					conn.Open();
				}
				catch (Exception ex)
				{

					MessageBox.Show(
						"Unable to connect to the database using the custom connection string:" + "\n" + connectionstring.Text + "\n" + "\n" + "Heres the error message, this is all we know..." + "\n" + "\n" + ex.ToString(),
						"ERROR: Failed to establish a connection", System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
					valid = false;

					MessageBox.Show(
						"Settings failed to save due to an invalid database connection string. Please enter a correct connection string or use default to save settings.",
						"ERROR: Unable to save settings.", System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
				finally
				{
					conn.Close();
				}
			}
			else if (data_defualt.IsChecked == true)
			{
				Properties.Settings.Default.DatalocDefault = false;
			}

			if (CheckForUpdates.IsChecked == true)
			{
				Properties.Settings.Default.CheckForUpdates = true;
			}
			else
			{
				Properties.Settings.Default.CheckForUpdates = false;
			}



			Properties.Settings.Default.ConnectionString = connectionstring.Text;
			Properties.Settings.Default.DefaultHeight = Convert.ToInt32(size_slider.Value * (75 / 5));

			if (valid)
			{
				Properties.Settings.Default.Save();
				this.Close();
				MessageBox.Show("Your settings have been saved successfully. The application will now restart.");

				Process.Start(Application.ResourceAssembly.Location);
				Application.Current.Shutdown();
			}
		}
        private void DeleteAccountButton(object sender, RoutedEventArgs e)  //Delete account 
		{
			try
			{
				if (Properties.Settings.Default.CurrentUser != "Guest" && Properties.Settings.Default.IsTeacher == false)
				{
					MessageBoxResult open = MessageBox.Show("Are you sure you wish to delete your account? While you can create a new one you will lose any exam results you currently have.", "Are you sure?",
						System.Windows.MessageBoxButton.YesNo,
						MessageBoxImage.Warning);

					if (open == MessageBoxResult.Yes)
					{
						using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
						{
							conn.Open();
							using (var command = new OleDbCommand("DELETE FROM [ExamResults] WHERE StudentID = @ID", conn))
							{
								command.Parameters.AddWithValue("@ID", Properties.Settings.Default.UserID);
								command.ExecuteNonQuery();
							}
							using (var command = new OleDbCommand("DELETE FROM [Users] WHERE ID = @ID", conn))
							{
								command.Parameters.AddWithValue("@ID", Properties.Settings.Default.UserID);
								command.ExecuteNonQuery();
							}
						}


						Properties.Settings.Default.CurrentUser = "Guest";
						Properties.Settings.Default.UserID = 0;
						Properties.Settings.Default.ClassID = 0;
						Properties.Settings.Default.AliasName = "Guest";
						Properties.Settings.Default.IsTeacher = false;

						Properties.Settings.Default.Save();

						MessageBox.Show("Successfully deleted your account, feel free to create another account.", "Class Deleted Successfully.",
							System.Windows.MessageBoxButton.OK,
							MessageBoxImage.Information);

						Process.Start(Application.ResourceAssembly.Location);
						Application.Current.Shutdown();
					}
				}
				if (Properties.Settings.Default.CurrentUser != "Guest" && Properties.Settings.Default.IsTeacher == true)
				{
					MessageBoxResult open = MessageBox.Show("Are you sure you wish to delete your teacher account? Deleting a teacher account will delete all your classes, all the student accounts inside of your classes and all of their exam results. This action can not be undone. Be sure to transfer a class or student to a different teacher if you wish to keep their data.", "Are you sure?",
						System.Windows.MessageBoxButton.YesNo,
						MessageBoxImage.Warning);

					if (open == MessageBoxResult.Yes)
					{
						List<string> ClassIDS = new List<string>();     //A List of all the class the teacher owns.
						List<string> StudentIDs = new List<string>();   //A list of all the students inside of their classes.

						using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
						{
							conn.Open();
							using (var command = new OleDbCommand("SELECT [ID] FROM Class WHERE TeacherID = @TeacherID", conn))
							{
								command.Parameters.AddWithValue("@TeacherID", Properties.Settings.Default.UserID);
								using (OleDbDataReader reader = command.ExecuteReader())
								{
									while (reader.Read())
									{
										ClassIDS.Add(reader[0].ToString());
									}
								}
							}

							foreach (string t in ClassIDS)
							{
								using (var command = new OleDbCommand("SELECT [ID] FROM Users WHERE ClassID = @ClassID", conn))
								{
									command.Parameters.AddWithValue("@ClassID", t);
									using (OleDbDataReader reader = command.ExecuteReader())
									{
										while (reader.Read())
										{
											StudentIDs.Add(reader[0].ToString());
										}
									}
								}
							}


							foreach (string t in StudentIDs)
							{

								using (var command = new OleDbCommand("DELETE FROM [ExamResults] WHERE StudentID = @ID", conn))
								{
									command.Parameters.AddWithValue("@ID", t);
									command.ExecuteNonQuery();
								}
								using (var command = new OleDbCommand("DELETE FROM [Users] WHERE ID = @ID", conn))
								{
									command.Parameters.AddWithValue("@ID", t);
									command.ExecuteNonQuery();
								}
							}

							foreach (string t in ClassIDS)
							{
								using (var command = new OleDbCommand("DELETE FROM [Class] WHERE ID = @ID", conn))
								{
									command.Parameters.AddWithValue("@ID", t);
									command.ExecuteNonQuery();
								}
							}

							using (var command = new OleDbCommand("DELETE FROM [Teachers] WHERE ID = @ID", conn))
							{
								command.Parameters.AddWithValue("@ID", Properties.Settings.Default.UserID);
								command.ExecuteNonQuery();
							}
						}

						Properties.Settings.Default.CurrentUser = "Guest";
						Properties.Settings.Default.UserID = 0;
						Properties.Settings.Default.ClassID = 0;
						Properties.Settings.Default.AliasName = "Guest";
						Properties.Settings.Default.IsTeacher = false;

						Properties.Settings.Default.Save();

						MessageBox.Show("Successfully deleted your account, including " + ClassIDS.Count + " Classes, containing a total of " + StudentIDs.Count + " Student accounts and their results.", "Teacher Account Deleted Successfully.",
							System.Windows.MessageBoxButton.OK,
							MessageBoxImage.Information);

						StudentIDs.Clear();
						ClassIDS.Clear();

						Process.Start(Application.ResourceAssembly.Location);
						Application.Current.Shutdown();
					}
				}
			}
			catch (Exception)
			{
				MessageBox.Show(
					"Failed to delete your user account. " + Properties.Resources.DataBaseError,
					"Database Read & Write Error: 102 D", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
        private void Download(object sender, System.Windows.Input.MouseButtonEventArgs e)   //Download dependency software
		{
			System.Diagnostics.Process.Start(Transformations.Properties.Resources.AccessDownload);
		}
        private void ChangeClassClick(object sender, RoutedEventArgs e)     //Change class 
		{
			Dialog_ComboBox Combo = new Dialog_ComboBox("Transfer User",
				"Please select below a new teacher and a new class to which you would like to transfer to.",
				"user_transfer", Properties.Settings.Default.UserID.ToString())
			{
				Owner = this
			};
			Combo.Show();
			Combo.Closed += SetContentHandler;
		}
        private void SetContentHandler(object sender, EventArgs e)  //Re-logs into your user account
		{
			try
			{
				using (var conn = new OleDbConnection {ConnectionString = DataBase.ConnectionString()})
				{
					conn.Open();
					using (var command = new OleDbCommand("SELECT [ID], [UserName], [ClassID], [AliasName]  FROM  Users", conn))
					{
						using (OleDbDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								if (reader[1].ToString() == System.Environment.UserName)
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
			}
			catch (Exception)
			{
				MessageBox.Show(
					"Failed to re-log you in, please manually logout and re-login. " + Properties.Resources.DataBaseError,
					"Database Read Error: 100 L", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
			}
			Load(sender, new RoutedEventArgs());
		}
	}
}
