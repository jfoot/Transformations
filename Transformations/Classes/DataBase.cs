using System;
using System.Configuration;
using System.Data.OleDb;

namespace Transformations
{
    class DataBase
    {
        public static string ConnectionString() //used to return the connection string being used by the user.
	    {
		    if (Properties.Settings.Default.DatalocDefault)    //If they are using the default connection string then.
		        return ConfigurationManager.ConnectionStrings["Transformations.Properties.Settings.DatabaseConnectionString"].ConnectionString;
            return  Properties.Settings.Default.ConnectionString;
		}

        public static int Counter(string SQL)   //Used to count the number of hard and easy exams the user has taken.
	    {
		    int total = 0;
            using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
            {
                conn.Open();
                using (var command = new OleDbCommand(SQL, conn))
                {
                    command.Parameters.AddWithValue("@StudentID", Properties.Settings.Default.UserID);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            total++;
                        }
                    }
                }
            }
		    return total;
	    }
        public static int Recent(string SQL)    //Used to retrieve all the recent exam results for each exam by topic.
	    {
		    int recent = 0;
            using (var conn = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
            {
                conn.Open();
                using (var command = new OleDbCommand(SQL, conn))
                {
                    command.Parameters.AddWithValue("@StudentID", Properties.Settings.Default.UserID);
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recent = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
		    return recent;
	    }
	}
}
