using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;

namespace Transformations
{
	/// <summary>
	/// Interaction logic for Dialog_ComboBox.xaml
    /// The dialog combo box is a multipurpose dialog window, which can be used for a variety of user situations.
    /// Depending on the data sent to the window depends on the task it can be completed.
	/// </summary>
	public partial class Dialog_ComboBox : Window
	{
		public string Command;     //The command sent to the window, depends what the window will do.
		public string Selected;    //The selected object from the previous window, normally an ID.

        public List<string> TeacherLists = new List<string>();  //All the teacher names
		public List<int> TeacherID = new List<int>();           //All the teacher ID
        public BindingList<string> ClassList = new BindingList<string>();   //Contents/ Class Name of the second ComboBox
		public BindingList<int> ClassID = new BindingList<int>();           //ID values for the Class Names

        public Dialog_ComboBox(string groupheader, string textbox, string _command, string _selected)
		{
			InitializeComponent();

			try
			{
                //Fills the UI with the information sent to the form.
				UserGroupBox.Header = groupheader;
				UserTextBox.Text = textbox;
				UserButton.Content = groupheader;
				Command = _command;
				Selected = _selected;
			}
			catch (Exception ex)
            {
                Crashes.TrackError(ex);
                MessageBox.Show(
                    Properties.Strings.UserClassInvalid + Properties.Strings.UserError,
                    Properties.Strings.EM_InvalidRequestError + "301 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }


			if (Command == "class_transfer" || Command == "user_transfer")    //If the user is trying to transfer a user or transfer a class
			{
				try
				{
					using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
					{
						conn.Open();
						using (var command = new OleDbCommand("SELECT [AliasName],[ID] FROM Teachers ORDER BY [AliasName]", conn))
						{   //Retrieve teacher names and ID from the database.
							command.Parameters.AddWithValue("@ID", Properties.Settings.Default.ClassID);
							using (OleDbDataReader reader = command.ExecuteReader())
							{
								while (reader.Read())
								{
									TeacherLists.Add(reader[0].ToString());
									TeacherID.Add(Convert.ToInt32(reader[1]));
								}
							}
						}
					}

					UserCombo.ItemsSource = TeacherLists;  //Fills the drop-down menu with the teacher list.
				}
				catch (Exception ex)
				{
                   Crashes.TrackError(ex);
                    MessageBox.Show(
                   Properties.Strings.TeacherNameFail + Properties.Strings.DataBaseError,
                   Properties.Strings.EM_DataBaseReadError + "100 D", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
			}
			if (Command == "user_transfer")    //if user transfer, make two drop-down menus visible.
			{
				SecondUserCombo.IsEnabled = true;
				SecondUserCombo.Visibility = Visibility.Visible;
			}
		}
        private void Execute(object sender, RoutedEventArgs e)
		{
			try
			{

				if (Command == "class_transfer" && UserCombo.SelectedIndex > -1)      //If class transfer and the user has a value selected.
				{
					using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
					{
						conn.Open();
						using (var command = new OleDbCommand("UPDATE Class SET TeacherID = @NEWID WHERE ID = @ID", conn))
						{   //Update/ change the class teacher ID to a new teacher ID (owner)
							command.Parameters.AddWithValue("@NEWID", TeacherID[UserCombo.SelectedIndex]);
							command.Parameters.AddWithValue("@ID", Selected);
							command.ExecuteNonQuery();
						}
					}

					this.Close();
					MessageBox.Show(
						Properties.Strings.ClassTransferedSuccesful,
						Properties.Strings.SuccessfulMove, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

				}
				else if (Command == "user_transfer" && UserCombo.SelectedIndex > -1)
				{
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
					{
						conn.Open();
						using (var command = new OleDbCommand("UPDATE Users SET ClassID = @NEWID WHERE ID = @ID", conn))
						{   //Changes the users class ID - to a new class.
							command.Parameters.AddWithValue("@NEWID", ClassID[SecondUserCombo.SelectedIndex]);
							command.Parameters.AddWithValue("@ID", Selected);
							command.ExecuteNonQuery();
						}
					}

					this.Close();
					MessageBox.Show(
						Properties.Strings.UserTransferedSuccessful,
                        Properties.Strings.SuccessfulMove, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
                    MessageBox.Show(
                    Properties.Strings.OwnerBlank + Properties.Strings.DataBaseError ,
                    	Properties.Strings.EM_FieldEmpty + "300 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
			catch (Exception ex)
			{
                Crashes.TrackError(ex);
                MessageBox.Show(
                        Properties.Strings.TransferFailed + Properties.Strings.DataBaseError,
                         Properties.Strings.DatabaseWriteError + " B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);

            }
	   }
        private void TeacherSelected(object sender, SelectionChangedEventArgs e)
		{

			SecondUserCombo.SelectedIndex = -1; //Ensures that the user has no value selected.
			ClassList.Clear(); //Clears out any old values.
			SecondUserCombo.ItemsSource = ClassList; //Re-defines the source of combo-box- Just to be safe :)

			try
			{
				using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
				{
					conn.Open();
					using (var command = new OleDbCommand("SELECT [ClassName],[ID] FROM Class WHERE [TeacherID] = @ID ORDER BY [ClassName]", conn))
					{   //Retrieve all the class names and IDIs owned by a teacher.
						command.Parameters.AddWithValue("@ID", TeacherID[UserCombo.SelectedIndex]); //Teacher.SelectedIdex is the value of the item selected in CB 1.
						using (OleDbDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								ClassList.Add(reader[0].ToString());
								ClassID.Add(Convert.ToInt32(reader[1]));
							}
						}
					}
				}
			   SecondUserCombo.ItemsSource = ClassList; //Fills Combo-box 2 with data.
			}
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.ClassOwnedFail + Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "100 E", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);

            }
		}
	}
}
