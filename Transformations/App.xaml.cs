using System;
using System.Globalization;
using System.IO;
using System.Windows;
using WPFLocalizeExtension.Engine;
using Transformations.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Transformations
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public FileInfo file;
		public string file_path = null;

        public App()
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
        }


        private void Application_Startup(object sender, StartupEventArgs e)
		{
            AppCenter.SetCountryCode(RegionInfo.CurrentRegion.TwoLetterISORegionName);
            AppCenter.Start(Classes.AppCenterAPI.Secret, typeof(Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));


            if (e.Args.Length == 1) //If a startup argument is sent to the program.
			{
                //If the start up argument matches a known start up command then preform the command.
                if (e.Args[0] == "-Exam")
				{
					StartupUri = new Uri("StudentZones\\TakeExam.xaml", UriKind.Relative);
				}
				else if (e.Args[0] == "-Settings")
				{
					StartupUri = new Uri("Settings.xaml", UriKind.Relative);
				}
                else if (e.Args[0] == "-troubleshoot" || e.Args[0] == "-fix")
                {
                    StartupUri = new Uri("Troubleshooting.xaml", UriKind.Relative);
                }
                else
				{
					file = new FileInfo(e.Args[0]);
					if (!file.Exists)
					{
						file = null;
					}
					else
					{
						file_path = file.ToString();
					}
                    StartupUri = new Uri("MainWindow\\MainWindow.xaml", UriKind.Relative);
                }
			}
			else //else just start up the program normally and launch the main window.
			{
               StartupUri = new Uri("MainWindow\\MainWindow.xaml", UriKind.Relative);
            }

        }
	}
}
