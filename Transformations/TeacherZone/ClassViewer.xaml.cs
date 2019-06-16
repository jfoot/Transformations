using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;


namespace Transformations
{
	/// <summary>
	/// The class viewer allows the teacher to view all the students inside of a selected class.
    /// - Transfer a user
    /// - Delete a user
    /// - See all user results
    /// - see a selected user results
	/// </summary>
	public partial class ResultsViewer : Window
	{
        public DataTable TableData = new DataTable();
        public string ClassID;

		public ResultsViewer(string _id, string title)
		{
			InitializeComponent();
			titleClass.Content = Properties.Strings.Class + " : " + title;
			ClassID = _id;

			try
			{
                using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                {
                    conn.Open();
                    using (var command = new OleDbCommand("SELECT[ID], [Username], [AliasName] from  Users WHERE ClassID = @ID ORDER BY AliasName", conn))
                    {   //Retrieves the IDs, User names and alias names for all the students inside of the selected account.
                        command.Parameters.AddWithValue("@ID", _id);

                        using (var adapter = new OleDbDataAdapter(command))
                        {
                            adapter.Fill(TableData);
                        }
                    }
                }

                UserGrid.ItemsSource = TableData.DefaultView;
            }
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.FailedToGetClass + Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "100 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}
        private void GridLoaded(object sender, RoutedEventArgs e)
		{
			try
			{   //Changes the width of the columns
				UserGrid.Columns[0].Width = new DataGridLength(0, DataGridLengthUnitType.Star);
				UserGrid.Columns[1].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
				UserGrid.Columns[2].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
			}
			catch (Exception)
            { 
				
			}
		}
        private void UserResult(object sender, RoutedEventArgs e)   //See a specific user result.
		{
			ClassViewer UserResults = new ClassViewer((UserGrid.SelectedCells[2].Column.GetCellContent(UserGrid.SelectedItem) as TextBlock).Text, (UserGrid.SelectedCells[0].Column.GetCellContent(UserGrid.SelectedItem) as TextBlock).Text, "user") { Owner = this };
			UserResults.Show();
		}
        private void DeleteUser(object sender, RoutedEventArgs e)   //Delete a student account.
		{
			try
			{
				//Retrieves the ID of the selected user
				string ID = (UserGrid.SelectedCells[0].Column.GetCellContent(UserGrid.SelectedItem) as TextBlock).Text;

				//Checks the user is sure they wish to delete the account
				MessageBoxResult messageBoxResult = MessageBox.Show(
			        Properties.Strings.AreYouSureDelete + "\n " +
					Properties.Strings.StudentDelete, Properties.Strings.AreYouSure,
					System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
					{
						conn.Open();
						using (var command = new OleDbCommand("DELETE FROM [ExamResults] WHERE StudentID = @ID", conn))
						{   //Delete all the exam results the user may have.
							command.Parameters.AddWithValue("@ID", ID);
							command.ExecuteNonQuery();
						}
						using (var command = new OleDbCommand("DELETE FROM [Users] WHERE ID = @ID", conn))
						{   //Delete the user account from the user table.
							command.Parameters.AddWithValue("@ID", ID);
							command.ExecuteNonQuery();
						}
					}
					SetContentHandler(sender, e);
				}
			}
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.FailedToDeleteSelectedUser + Properties.Strings.DataBaseError,
                    Properties.Strings.DatabaseWriteError + " E", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}
        private void AllUsers(object sender, RoutedEventArgs e) //View the whole class exam results
		{
			ClassViewer ClassResults = new ClassViewer(Properties.Strings.AllUser, ClassID, "all") {Owner = this};
			ClassResults.Show();
		}
        private void TransferUser(object sender, RoutedEventArgs e) //transfer the user to a new class
		{
				Dialog_ComboBox Combo = new Dialog_ComboBox(Properties.Strings.TransferUser,
					Properties.Strings.TransferUserPrompt, "user_transfer",
					(UserGrid.SelectedCells[0].Column.GetCellContent(UserGrid.SelectedItem) as TextBlock).Text) {Owner = this};
				Combo.Show();
				Combo.Closed += SetContentHandler;
		}
        private void SetContentHandler(object sender, EventArgs e)  //Refresh the data-grid
		{
			try
			{
                TableData.Clear();
                using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                {
                    conn.Open();

                    using (var command = new OleDbCommand("SELECT[ID], [Username], [AliasName] from  Users WHERE ClassID = @ID ORDER BY AliasName", conn))
                    {
                        command.Parameters.AddWithValue("@ID", ClassID);

                        using (var adapter = new OleDbDataAdapter(command))
                        {
                            adapter.Fill(TableData);
                        }
                    }
                }

                UserGrid.ItemsSource = TableData.DefaultView;


				UserGrid.Columns[0].Width = new DataGridLength(0, DataGridLengthUnitType.Star);
				UserGrid.Columns[1].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
				UserGrid.Columns[2].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
                
			}
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.FailedToGetClass + Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "100 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}
	}
}
