using System;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Data;
using System.Windows.Data;
using MessageBox = System.Windows.MessageBox;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for DataGrid.xaml
    /// </summary>
    public partial class ClassViewer : Window
    {
        public DataTable TableData = new DataTable();
        public string ID;
        public string Type;

        public ClassViewer(string _name, string _id, string _type)
        {
            InitializeComponent();
            try
            {
                Title.Content = _name + "'s Results";
                ID = _id;
                Type = _type;
                FillData();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "User/Class data request was invalid. " + Properties.Resources.UserError,
                    "Invalid Request Error: 301 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FillData()
        {
            try
            {
                TableData.Clear();
                if (Type == "all")
                {
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
						//Retrieves all the users exam results for the selected class
                        using (var command = new OleDbCommand("SELECT [ExamID],[AliasName],[ExamTopic],[Difficulty],[Score],[Attempts],[Time],[Pass] FROM (Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID) INNER JOIN Users ON Users.ID = ExamResults.StudentID WHERE ClassID = @ClassID ORDER BY [Username]", conn))
                        {
                            command.Parameters.AddWithValue("@ClassID", ID);
                            using (var adapter = new OleDbDataAdapter(command))
                            {
                                adapter.Fill(TableData);
                            }
                        }
                    }

                    CollectionViewSource mycollection = new CollectionViewSource { Source = TableData };
                    mycollection.GroupDescriptions.Add(new PropertyGroupDescription("AliasName"));
                    UserGrid.ItemsSource = mycollection.View;
                }
                else if (Type == "user")
                {
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
						//Retrieves only a single users exam results 
                        conn.Open();
                        using (var command = new OleDbCommand("SELECT [ExamID],[ExamTopic],[Difficulty], [Score], [Attempts], [Time], [Pass]  FROM Exams INNER JOIN ExamResults ON Exams.ID = ExamResults.ExamID WHERE ExamResults.StudentID = @StudentID ", conn))
                        {
                            command.Parameters.AddWithValue("@StudentID", ID);
                            using (var adapter = new OleDbDataAdapter(command))
                            {
                                adapter.Fill(TableData);
                            }
                        }
                    }

                    UserGrid.ItemsSource = TableData.DefaultView;
                }

            }
            catch (Exception)
            {
                MessageBox.Show(
                                "Failed to retrieve  user/class exam results. " + Properties.Resources.DataBaseError,
                                "Database Read Error: 100 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GirdLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Type == "all")
                {
                    UserGrid.Columns[1].Visibility = Visibility.Collapsed;
                    UserGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                else if (Type == "user")
                {
                    UserGrid.Columns[0].Visibility = Visibility.Collapsed;
                }

                foreach (DataGridColumn t in UserGrid.Columns)
                {
                    t.Width = new DataGridLength(0.2, DataGridLengthUnitType.Star);
                }

            }
            catch (Exception)
            {
                MessageBox.Show(
                    "This user does not have any exam results yet. When they take an exam, their results will appear here.",
                    "Information", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }
        private void Export(object sender, RoutedEventArgs e)	//Used to export the data grid to an excel document
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog		//Save file dialog box appears
                {
                    Filter = "Microsoft Excel Comma Separated Values|*.csv",
                    Title = "Save an project file"
                };
                save.ShowDialog();

				if (save.FileName != "")	//Ensure the user has a filename
				{
					//Allows the program to select multiple cells and that all columns are visible
					UserGrid.SelectionMode = DataGridSelectionMode.Extended;
					UserGrid.Columns[1].Visibility = Visibility.Visible;

					//Selects all the cells
					UserGrid.SelectAllCells();
					UserGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
					//Copies the whole of the data grid as a CSV file
					ApplicationCommands.Copy.Execute(null, UserGrid);
					String CSVData = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);
					UserGrid.UnselectAllCells();
					
					//Creates a new stream writer and saves to a file
					StreamWriter writer = new StreamWriter(save.OpenFile());
					writer.WriteLine(CSVData);
					writer.Close();
				}

				if (Type == "all")
                {
                    UserGrid.Columns[1].Visibility = Visibility.Collapsed;
                }
				UserGrid.SelectionMode = DataGridSelectionMode.Single;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Results exportation failed. " + Properties.Resources.CriticalFailuer,
                    "Critical Program Failure: 400 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteResult(object sender, RoutedEventArgs e)	//Delete an exam result
        {
            try
            {
				//Retrieves the result ID selected
                string ID = (UserGrid.SelectedCells[0].Column.GetCellContent(UserGrid.SelectedItem) as TextBlock).Text;
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "Are you sure you want to delete this exam result?" + "\n" + "This action can not be undone.",
                    "Delete Confirmation",
                    System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.Yes)	//Confirm the user is sure
                {
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
						// Delete the exam result from the database
                        using (var command = new OleDbCommand("DELETE FROM [ExamResults] WHERE ExamID = @ID", conn))
                        {
                            command.Parameters.AddWithValue("@ID", ID);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                FillData();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Unable to delete this exam result. This result could already be deleted. " + Properties.Resources.UserError,
                    "Invalid Request Error: 301 D", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
