using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Transformations
{
    /// <summary>
    /// Class editor is the first screen shown upon starting up the student manger here teachers can:
    /// - Create new classes
    /// - View their own classes and all the classes
    /// - Transfer ownership of class
    /// - Delete a class
    /// - Rename Class
    /// - See the results of the selected class
    /// </summary>
    public partial class ClassEditor : Window
    {
        public bool All = false;
        public DataTable TableData = new DataTable();

        public ClassEditor()
        {
            InitializeComponent();
        }

        private void RefreshGrid()  //used to refresh the data-grid when changes are made to it.
        {
            try
            {
                TableData.Clear();
                using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                {
                    conn.Open();
                    if (All == false)
                    {
                        using (var command = new OleDbCommand("SELECT [ID],[ClassName] FROM Class WHERE TeacherID = @TeacherID ORDER BY ClassName", conn))
                        {   //Retrieves all the classes which the teacher owns and their IDs.
                            command.Parameters.AddWithValue("@TeacherID", Properties.Settings.Default.UserID);
                            using (var adapter = new OleDbDataAdapter(command))
                            {
                                adapter.Fill(TableData);
                            }
                        }
                        _ClassGrid.ItemsSource = TableData.DefaultView;

                    }
                    else
                    {
                        using (var command = new OleDbCommand("SELECT Class.ID,[ClassName],Teachers.AliasName FROM Class INNER JOIN Teachers ON Class.TeacherID = Teachers.ID ORDER BY ClassName", conn))
                        {   //Retrieves all the classes on the data base, including their class ID and teacher name.
                            using (var adapter = new OleDbDataAdapter(command))
                            {
                                adapter.Fill(TableData);
                            }
                        }
                        CollectionViewSource mycollection = new CollectionViewSource { Source = TableData };
                        mycollection.GroupDescriptions.Add(new PropertyGroupDescription("AliasName"));
                        _ClassGrid.ItemsSource = mycollection.View;
                    }
                }

                try
                {
                    _ClassGrid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);   //Sets the width of column 2 to be the widest
                    _ClassGrid.Columns[2].Visibility = Visibility.Collapsed;    //Hides the last column containing teacher name.
                }
                catch (Exception) { }

                _ClassGrid.UnselectAllCells();
            }
            catch (Exception)
            {
                MessageBox.Show(
                   Properties.Strings.FailedToGetDataBase + Properties.Strings.DataBaseError,
                    Properties.Strings.Error100, System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CreateClass(object sender, RoutedEventArgs e)      //used to create a new class
        {
            Analytics.TrackEvent("Attempted To Create A Class");

            try
            {
                using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                {
                    conn.Open();
                    using (var command = new OleDbCommand("INSERT INTO Class(ClassName, TeacherID) VALUES (@ClassName,  @TeacherID)", conn))
                    {   //Create a new class, by inserting the class name and the teacher ID into the class table.
                        command.Parameters.AddWithValue("@ClassName", classname.Text.ToString());
                        command.Parameters.AddWithValue("@TeacherID", Properties.Settings.Default.UserID);
                        command.ExecuteNonQuery();
                    }
                }

                RefreshGrid();
                MessageBox.Show(
                    Properties.Strings.NewClassCalled + classname.Text.ToString() + Properties.Strings.NewClassCalled2,
                    Properties.Strings.ClassCreated, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                Analytics.TrackEvent("New Class Created");
                classname.Text = "";
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                MessageBox.Show(
                    Properties.Strings.FailedToMakeNewClass + Properties.Strings.DataBaseError,
                    Properties.Strings.DatabaseWriteError + "A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GridLoaded(object sender, RoutedEventArgs e)       //Called upon when the grid is loaded
        {
            RefreshGrid();  //Refresh the grid
        }
        private void DeleteClass(object sender, RoutedEventArgs e)  //Used to delete a class
        {
            Analytics.TrackEvent("Attempted To Delete A Class");

            IsOwner();
            try
            {
                string ID = (_ClassGrid.SelectedCells[0].Column.GetCellContent(_ClassGrid.SelectedItem) as TextBlock).Text; //The currently selected column on the data grid.
                                                                                                                            //Asks the user are they sure?
                MessageBoxResult open = MessageBox.Show(
                    Properties.Strings.ConfirmClassDelete,
                    Properties.Strings.AreYouSure,
                    System.Windows.MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (open == MessageBoxResult.Yes)
                {
                    List<string> StudentIDs = new List<string>();

                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
                        using (var command = new OleDbCommand("SELECT [ID] FROM Users WHERE ClassID = @ClassID", conn))
                        {   //Retrieves all the user IDs of users inside of the selected class.
                            command.Parameters.AddWithValue("@ClassID", ID);
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    StudentIDs.Add(reader[0].ToString());
                                }
                            }
                        }

                        foreach (string t in StudentIDs)
                        {
                            using (var command = new OleDbCommand("DELETE FROM [ExamResults] WHERE StudentID = @ID", conn))
                            {   //Deletes every users exam results from the class account.
                                command.Parameters.AddWithValue("@ID", t);
                                command.ExecuteNonQuery();
                            }
                            using (var command = new OleDbCommand("DELETE FROM [Users] WHERE ID = @ID", conn))
                            {   //Deletes all the user accounts from the class account.
                                command.Parameters.AddWithValue("@ID", t);
                                command.ExecuteNonQuery();
                            }
                        }

                        using (var command = new OleDbCommand("DELETE FROM [Class] WHERE ID = @ID", conn))
                        {   //Finally delete the class from the database.
                            command.Parameters.AddWithValue("@ID", ID);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show(Properties.Strings.SuccDeletedClass + StudentIDs.Count + Properties.Strings.SuccDeletedClass2,
                        Properties.Strings.DeletedSuccessfully,
                        System.Windows.MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    RefreshGrid();
                    StudentIDs.Clear();
                    Analytics.TrackEvent("Class Deleted");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                MessageBox.Show(
                    Properties.Strings.FailedToDeleteclass + Properties.Strings.DataBaseError,
                    Properties.Strings.EM_DataBaseReadError + "102 A", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SeeClassData(object sender, RoutedEventArgs e)     //See all the class data
        {
            Analytics.TrackEvent("See Class Data");
            ResultsViewer edit = new ResultsViewer(((_ClassGrid.SelectedCells[0].Column.GetCellContent(_ClassGrid.SelectedItem) as TextBlock).Text), ((_ClassGrid.SelectedCells[1].Column.GetCellContent(_ClassGrid.SelectedItem) as TextBlock).Text)) { Owner = this };
            edit.Show();
        }
        private void TransferClass(object sender, RoutedEventArgs e)    //Transfer a class to a different user.
        {
            Analytics.TrackEvent("Transfer Class");
            IsOwner();
            Dialog_ComboBox Combo = new Dialog_ComboBox(Properties.Strings.ClassTransfer,Properties.Strings.TransferDesc, "class_transfer", (_ClassGrid.SelectedCells[0].Column.GetCellContent(_ClassGrid.SelectedItem) as TextBlock).Text) { Owner = this };
            Combo.Closed += SetContentHandler;
            Combo.Show();
        }
        private void RenameClass(object sender, RoutedEventArgs e)  //Rename a class
        {
            Analytics.TrackEvent("Rename Class");
            IsOwner();
            Dialog_TextBox TextBox = new Dialog_TextBox(Properties.Strings.ClassRename,
                    Properties.Strings.RenameDesc, "class_rename",
                    (_ClassGrid.SelectedCells[0].Column.GetCellContent(_ClassGrid.SelectedItem) as TextBlock).Text)
            { Owner = this };
            TextBox.Closed += SetContentHandler;
            TextBox.Show();
        }
        private void SetContentHandler(object sender, EventArgs e)  //Triggers the data grid to refresh.
        {
            RefreshGrid();
        }
        private void ViewSpecific(object sender, RoutedEventArgs e) //View only classes owned by a teacher
        {
            Analytics.TrackEvent("View Specific Class by Teacher");
            All = false;
            RefreshGrid();
        }
        private void ViewAllClass(object sender, RoutedEventArgs e) //View all classes on the database
        {
            Analytics.TrackEvent("View All Classes");
            All = true;
            RefreshGrid();
        }
        private void IsOwner()  //Checks if the teacher is the owner of the class or not.
        {
            if (All)
            {
                bool check = false;
                try
                {
                    using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
                    {
                        conn.Open();
                        using (var command = new OleDbCommand("SELECT[ID],[ClassName] FROM Class WHERE TeacherID = @TeacherID", conn))
                        {
                            command.Parameters.AddWithValue("@TeacherID", Properties.Settings.Default.UserID);
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[0].ToString() == (_ClassGrid.SelectedCells[0].Column.GetCellContent(_ClassGrid.SelectedItem) as TextBlock).Text)
                                    {
                                        check = true;
                                    }
                                }
                            }
                        }
                    }

                    if (check == false)
                    {
                        MessageBox.Show(
                            Properties.Strings.AreYourSureClassTransfer,
                            Properties.Strings.Attention, System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    MessageBox.Show(
                        Properties.Strings.FailedToGetClasses + Properties.Strings.DataBaseError,
                        Properties.Strings.EM_DataBaseReadError + "100 K", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
