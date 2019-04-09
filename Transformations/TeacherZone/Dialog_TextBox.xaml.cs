using System.Data.OleDb;
using System.Windows;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for Dialog_TextBox.xaml
    /// The dialog text box is a multipurpose text box dialog, which will change depending upon the command a user wishes to preform.
    /// </summary>
    public partial class Dialog_TextBox : Window
	{
		public string Command;
		public string Selected;
	
        public Dialog_TextBox(string groupheader, string textbox, string _command, string _selected)
		{
            InitializeComponent();
            try
            {   //Updates the user UI 
                UserGroupBox.Header = groupheader;
                UserTextBox.Text = textbox;
                UserButton.Content = groupheader;
                Command = _command;
                Selected = _selected;
            }
            catch (System.Exception)
            {
                ////MessageBox.Show(
                ////    "Class rename request was invalid. " + Properties.Strings.UserError,
                ////    Properties.Strings.EM_InvalidRequestError + "301 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
		private void Execute(object sender, RoutedEventArgs e)
		{
            try
            {
	            if (Command == "class_rename") //if the user is trying to rename a class.
	            {
		            using (var conn = new OleDbConnection {ConnectionString = DataBase.ConnectionString()})
		            {
			            conn.Open();
			            using (var command = new OleDbCommand("UPDATE Class SET ClassName = @newname WHERE ID = @ID", conn))
			            {   //Update the class name of the selected class to a new name.
				            command.Parameters.AddWithValue("@newname", User_textbox.Text.ToString());
				            command.Parameters.AddWithValue("@ID", Selected);
				            command.ExecuteNonQuery();
			            }
		            }
		            this.Close();
	            }
	        }
            catch (System.Exception)
            {
                //MessageBox.Show(
                //       "Failed to rename the class. " + Properties.Strings.DataBaseError ,
                //       "Database Write Error: 101 C", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
	}
}
