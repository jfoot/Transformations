using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
	{
		public List<string> TeacherLists = new List<string>();
		public List<int> TeacherID = new List<int>();

		public BindingList<string> ClassList = new BindingList<string>();
		public BindingList<int> ClassID = new BindingList<int>();

		public CreateAccount()
		{
			InitializeComponent();
          
            try
            {
                username.Content = System.Environment.UserName;
            }
            catch (Exception)
            {
                MessageBox.Show(
                   Properties.Strings.WinUserNameFail +  Properties.Strings.WindowsError,
                   Properties.Strings.EM_InsuffientPrivileges + "200 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                {
                    conn.Open();
                    using (var command = new OleDbCommand("SELECT [AliasName],[ID] FROM Teachers ORDER BY [AliasName]", conn))
                    {   //Select all the teacher names and teacher id, from the teacher table and order them in alphabetical order.
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

                teacher.ItemsSource = TeacherLists;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Properties.Strings.TeacherNameFail+ Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "100 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}
        private void TeacherSelected(object sender, SelectionChangedEventArgs e)
		{
			try
			{
                ClassCombo.SelectedIndex = -1;
                ClassList.Clear();
                ClassCombo.ItemsSource = ClassList;
                
                using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                {
                    conn.Open();
                    using (var command = new OleDbCommand("SELECT [ClassName],[ID] FROM Class WHERE [TeacherID] = @ID ORDER BY [ClassName]", conn))
                    {   //Select all the class names and class IDs of the selected teacher above.
                        command.Parameters.AddWithValue("@ID", TeacherID[teacher.SelectedIndex]);
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
                
				ClassCombo.ItemsSource = ClassList;
            }
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.ClassOwnedFail + Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "100 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CreateAccountButton(object sender, RoutedEventArgs e)
		{
			if (name.Text != "" && teacher.SelectedIndex > -1 && ClassCombo.SelectedIndex > -1) //Checks that the user has both a teacher and a class selected and that their name is not blank.
			{
				try
				{
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
                        using (var command = new OleDbCommand("INSERT INTO Users(Username, ClassID, AliasName) VALUES (@Username,  @ClassID, @AliasName)", conn))
                        {   //Create a new account by inserting the username, classID and alias-name into the database; their ID will be automatically assigned to them.
                            command.Parameters.AddWithValue("@Username", System.Environment.UserName);
                            command.Parameters.AddWithValue("@ClassID", ClassID[ClassCombo.SelectedIndex]);
                            command.Parameters.AddWithValue("@AliasName", name.Text);
                            command.ExecuteNonQuery();
                        }
                    
                                          
                        using (var command = new OleDbCommand("SELECT [ID], [UserName], [ClassID], [AliasName]  FROM  Users", conn))
                        {   //Then login the user, by retrieving their, user ID, username, class ID and alias name.
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

                                        Properties.Settings.Default.Save();
                                    }
                                }
                            }
                        }
                    }

                    MessageBox.Show(
                    Properties.Strings.AccountCreated,
                    Properties.Strings.AccountCreatedHeader, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

                    //Restart the program.
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                    
                }
				catch (Exception)
				{
                    MessageBox.Show(
                    Properties.Strings.FailedToCreateAccount + Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "102 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
			}
			else
			{
                MessageBox.Show(
                    Properties.Strings.TeacherOrClassBlank +  Properties.Strings.UserError,
                    Properties.Strings.EM_FieldEmpty + "300 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
            }
		}
	}
}
