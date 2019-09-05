using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.Windows;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for CreateTeacherAccount.xaml
    /// </summary>
    public partial class CreateTeacherAccount : Window
    {
	  	public CreateTeacherAccount()
        {
            InitializeComponent();
            try
            {
                username.Content = System.Environment.UserName; //Retrieves the current windows user name.
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                MessageBox.Show(
                   Properties.Strings.WinUserNameFail + Properties.Strings.WindowsError,
                   Properties.Strings.EM_InsuffientPrivileges + "200 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
    	}
        private void CreateTeacherAccountButton(object sender, RoutedEventArgs e)
		{
            Analytics.TrackEvent("Attempted To Create A Teacher Account");

            if (passbox.Password == "Transformation17" && name.Text != "")    //If the password equals the correct password and the name is not blank.
			{
                try
                {
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
                        using (var command = new OleDbCommand("INSERT INTO Teachers(UserName, AliasName) VALUES (@Username,  @AliasName)", conn))
                        {   //Creates a new teacher account, by inserting the username and alias name into the teacher table, ID will be assigned later.
                            command.Parameters.AddWithValue("@Username", System.Environment.UserName);
                            command.Parameters.AddWithValue("@AliasName", name.Text.ToString());
                            command.ExecuteNonQuery();
                        }
                                                
                        using (var command = new OleDbCommand("SELECT [ID], [UserName], [AliasName]  FROM  Teachers", conn))
                        {    //Logs in the teacher to their account.
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[1].ToString() == System.Environment.UserName)
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
                    MessageBox.Show(
                     Properties.Strings.TeacherCreated,
                     Properties.Strings.AccountCreatedHeader, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    Analytics.TrackEvent("Teacher Account Created");

                    //Restarts the application
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    MessageBox.Show(
                      Properties.Strings.FailedToMakeTeacher + Properties.Strings.DataBaseError,
                      Properties.Strings.EM_DataBaseReadError + "102 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
			{   //if the username is blank or if the password is incorrect
                Analytics.TrackEvent("UserName Blank Or Password Wrong");

                MessageBox.Show(
					Properties.Strings.IncorrectPassword,
                    Properties.Strings.EM_FieldEmpty + "300 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
	}
}
