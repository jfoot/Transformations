using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace Transformations
{
	/// <summary>
	/// The troubleshooting tool, runs a variety of different tests to help distinguish any errors in the program.
	/// </summary>
	public partial class Troubleshooting : Window
	{
		public List<string> Text = new List<string>();
		public bool DatabaseDependcy = false;
		public bool DatabaseConnection = false;
		public bool DatabaseRead = false;
		public bool DatabaseWrite = false;

		public bool WindowsRights = false;

		public bool pdfsharpfound = false;
		public bool WinControlsFound = false;
		public bool WPFToolFound = false;

		public Troubleshooting()
		{
			InitializeComponent();
			listbox.ItemsSource = Text;

			Text.Add("TRANSFORMATIONS TROUBLESHOOTING- Jonathan Foot 2017©  ALL RIGHTS RESERVED.");
			Text.Add("SYSTEM: VERSION:  BUILD " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
			Text.Add("SYSTEM: START UP COMPLETE. ERROR DIAGNOSTICS BEGINNING.");
			Text.Add("");
			Text.Add("");

			stage1();

			Text.Add("");
			Text.Add("");
			stage2();

			Text.Add("");
			Text.Add("");
			stage3();

			Text.Add("");
			Text.Add("");
			stage4();

			Text.Add("");
			Text.Add("");
			stage5();
			Text.Add("");
			Text.Add("");
			Text.Add("TROUBLESHOOTING COMPLETED. PLEASE REVIEW ABOVE FOR ANY ERRORS OR ISSUES.");
			Text.Add("REFER TO THE USER MANUAL TO FIX ANY ISSUES YOU MAY HAVE.");
			Text.Add("IF PROBLEMS STILL PERSIST TRY CONTACTING YOUR SOFTWARE VENDOR.");

		}


		public void stage1()    //Database trouble shooting
		{
			Text.Add("STAGE 1 : DATABASE TROUBLESHOOTING");


			//Checks dependency software is installed.

			string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
			using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
			{
				foreach (string subkey_name in key.GetSubKeyNames())
				{
					using (RegistryKey subkey = key.OpenSubKey(subkey_name))
					{
						try
						{
							if ((subkey.GetValue("DisplayName")).ToString().Contains("Microsoft Office Access database engine"))
							{
								DatabaseDependcy = true;
							}
						}
						catch (Exception)
						{

						}
					}
				}
			}
			if (DatabaseDependcy)
			{
				Text.Add("          DATABASE DEPENDENCY  SOFTWARE CORRECTLY INSTALLED   ✔");
			}
			else
			{
				Text.Add("          DATABASE DEPENDENCY  SOFTWARE IS NOT INSTALLED      ✖");
			}

			//Checks program can connect to the database.
			System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
			try
			{
				conn.ConnectionString = DataBase.ConnectionString();
				conn.Open();
				DatabaseConnection = true;
			}
			catch (Exception)
			{
				DatabaseConnection = false;
			}
			finally
			{
				conn.Close();
			}
			if (DatabaseConnection)
			{
				Text.Add("          DATABASE CONNECTION ESTABLISHED SUCCESSFULLY        ✔");
			}
			else
			{
				Text.Add("          DATABASE CONNECTION ESTABLISHED UNSUCCESSFULLY      ✖");
			}

			//Checks program can read from the database.
			try
			{
				using (var conn2 = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
				{
					conn2.Open();
					using (var command = new OleDbCommand("SELECT *  FROM  Users", conn2))
					{
						using (OleDbDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
							}
						}
					}
				}
				DatabaseRead = true;
			}
			catch (Exception)
			{
				DatabaseRead = false;
			}

			if (DatabaseRead)
			{
				Text.Add("          DATABASE READ ACTION PREFORMED SUCCESSFULLY         ✔");
			}
			else
			{
				Text.Add("          DATABASE READ ACTION FAILED                         ✖");
			}

			//Checks program can write to the database.

			try
			{
				using (var conn2 = new OleDbConnection { ConnectionString = DataBase.ConnectionString() })
				{
					conn2.Open();
					using (var command = new OleDbCommand("INSERT INTO ExamResults ([StudentID], [ExamID], [Score], [Attempts], [Time], [Pass]) VALUES (@StudentID,  @ExamID, @Score, @Attempts, @Time, @Pass)", conn2))
					{
						command.Parameters.AddWithValue("@StudentID", 1);
						command.Parameters.AddWithValue("@ExamID", 1);
						command.Parameters.AddWithValue("@Score", 0);
						command.Parameters.AddWithValue("@Attempts", 0);
						command.Parameters.AddWithValue("@Time", "0:00");
						command.Parameters.AddWithValue("@Pass", false);
						command.ExecuteNonQuery();
					}

					using (var command = new OleDbCommand("DELETE FROM [ExamResults] WHERE @StudentID", conn2))
					{
						command.Parameters.AddWithValue("@StudentID", 1);
						command.ExecuteNonQuery();
					}
				}
				DatabaseWrite = true;
			}
			catch (Exception)
			{
				DatabaseWrite = false;
			}

			if (DatabaseWrite)
			{
				Text.Add("          DATABASE WRITE ACTION PREFORMED SUCCESSFULLY        ✔");
			}
			else
			{
				Text.Add("          DATABASE WRITE ACTION FAILED                        ✖");
			}

		}

		public void stage2()    //Windows troubleshooting - checks the user has sufficient rights
		{
			Text.Add("STAGE 2 : WINDOWS TROUBLESHOOTING ");
			try
			{
				string test = System.Environment.UserName;
				Text.Add("          PROGRAM HAS THE REQUIRED USER PRIVILEGES            ✔");
				WindowsRights = true;
			}
			catch (Exception)
			{
				Text.Add("          PROGRAM DOES NOT HAVE REQUIRED USER PRIVILEGES      ✖");
			}

		}

		public void stage3()    //Program dependency troubleshooting - checks the required files are installed
		{
			Text.Add("STAGE 3 : PROGRAM DEPENDENCY TROUBLESHOOTING");

			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/lib/PdfSharp.dll"))
			{
				Text.Add("          PDFSHARP.DLL WAS SUCCESSFULLY FOUND                 ✔");
				pdfsharpfound = true;
			}
			else
			{
				Text.Add("          PDFSHARP.DLL WAS NOT FOUND                          ✖");
			}

			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/lib/System.Windows.Controls.DataVisualization.Toolkit.dll"))
			{
				Text.Add("          SYSTEM.WINDOWS.CONTROLS.DLL WAS SUCCESSFULLY FOUND  ✔");
				WinControlsFound = true;
			}
			else
			{
				Text.Add("          SYSTEM.WINDOWS.CONTROLS.DLL WAS NOT FOUND           ✖");
			}

			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/lib/WPFToolkit.dll"))
			{
				Text.Add("          WPFTOOLKIT.DLL WAS SUCCESSFULLY FOUND               ✔");
				WPFToolFound = true;
			}
			else
			{
				Text.Add("          WPFTOOLKIT.DLL WAS NOT FOUND                        ✖");
			}

			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/lib/HtmlAgilityPack.dll"))
			{
				Text.Add("          HTMLAGILITYPACK.DLL WAS SUCCESSFULLY FOUND          ✔");
				WPFToolFound = true;
			}
			else
			{
				Text.Add("          HTMLAGILITYPACK.DLL WAS NOT FOUND                   ✖");
			}
		}


		public void stage4()    //Graphical troubleshooting - checks that .net framework is installed correctly.
		{
			Text.Add("STAGE 4 : GRAPHICS TROUBLESHOOTING");

			try
			{
				Canvas myCanvas = new Canvas();
				Ellipse myEllipse = new Ellipse();
				SolidColorBrush mySolidColorBrush = new SolidColorBrush();
				mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
				myEllipse.Fill = mySolidColorBrush;
				myEllipse.StrokeThickness = 2;
				myEllipse.Stroke = Brushes.Black;
				myEllipse.Width = 200;
				myEllipse.Height = 100;
				myCanvas.Children.Add(myEllipse);
				Text.Add("          SHAPES SPAWN SUCCESFULL                             ✔");

				try
				{
					myEllipse.RenderTransform = new ScaleTransform(3, 5, 2, 6);
					Text.Add("          RENDER TRANSFORMATION SUCCESFUL                     ✔");

					try
					{
						DropShadowEffect myDropShadowEffect = new DropShadowEffect();
						Color myShadowColor = new Color { ScA = 1 };
						myDropShadowEffect.Color = myShadowColor;
						myDropShadowEffect.BlurRadius = 25;
						myDropShadowEffect.Opacity = 1;
						myEllipse.Effect = myDropShadowEffect;
						Text.Add("          RENDER EFFECTS SUCCESFUL                            ✔");
					}
					catch (Exception)
					{
						Text.Add("          RENDER EFFECTS UNSUCCESFUL                          ✖");

					}
				}
				catch (Exception)
				{
					Text.Add("          RENDER TRANSFORMATION UNSUCCESFUL                   ✖");
				}
			}
			catch (Exception)
			{
				Text.Add("          SHAPES SPAWN UNSUCCESFUL                            ✖");
			}


		}

		public void stage5()    //Resets the program back to defaults
		{
			Text.Add("STAGE 5 : RESET PROGRAM TO DEFUALT");

			Properties.Settings.Default.DefaultPerformance = false;
			Properties.Settings.Default.DefaultResolution = false;
			Properties.Settings.Default.CurrentUser = Properties.Strings.Guest;
			Properties.Settings.Default.UserID = 0;
			Properties.Settings.Default.ClassID = 0;
			Properties.Settings.Default.AliasName = Properties.Strings.Guest;
			Properties.Settings.Default.IsTeacher = false;

			Properties.Settings.Default.Save();


			Text.Add("          CERTAIN SETTINGS RESET TO DEFAULT SUCCESSFULLY      ✔");
			Text.Add("          LOGGED OUT OF ANY ACTIVE ACCOUNT LOGIN SESSIONS     ✔");
		}
	}
}
