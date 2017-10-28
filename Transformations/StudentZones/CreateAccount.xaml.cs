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
                   "Failed to retrieve your Windows User name. " + Properties.Resources.WindowsError,
                   "Insufficient Privileges Error : 200 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
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
                    "Failed to retrieve teacher names. " + Properties.Resources.DataBaseError,
                    "DataBase Read Error : 100 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
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
                    "Failed to retrieve classes owned by selected teacher. " + Properties.Resources.DataBaseError,
                    "DataBase Read Error : 100 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
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
                    "User account successfully created, the application will now restart and log you in.",
                    "Account Created.", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

                    //Restart the program.
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                    
                }
				catch (Exception)
				{
                    MessageBox.Show(
                 "Failed to create a new user account. " + Properties.Resources.DataBaseError,
                 "Database Read & Write Error : 102 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
			}
			else
			{
				MessageBox.Show(
					"Teacher or class selection has been left blank or invalid. " + Properties.Resources.UserError,
					"Field Empty Error : 300 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}
