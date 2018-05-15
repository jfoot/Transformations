using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Transformations
{
	/// <summary>
	/// Interaction logic for UpdateChecker.xaml
	/// </summary>
	public partial class UpdateChecker : Window
	{
		public string URL;

		public UpdateChecker(string NewVerison)
		{
			InitializeComponent();
			try
			{
				var webGet = new HtmlWeb();
              
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
       | SecurityProtocolType.Tls11
       | SecurityProtocolType.Tls12
       | SecurityProtocolType.Ssl3;

                var doc = webGet.Load(Transformations.Properties.Resources.UpdatePage);

				CurrentVersionNo.Content = "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
				NewVersionNo.Content = NewVerison;
				
				HtmlNode ChangeLogData = doc.DocumentNode.SelectSingleNode("//td[@id='ChangeLog']");
				string UnFormatedChangeLog = ChangeLogData.InnerText.ToString();
				string FormatedChangeLog = UnFormatedChangeLog.Replace("&bull;", "•");
				ChangeLog.Text = FormatedChangeLog;

				HtmlNode UpdateType = doc.DocumentNode.SelectSingleNode("//td[@id='UpdateType']");
				UpdateTypeText.Content = UpdateType.InnerText.ToString();

				HtmlNode Severity = doc.DocumentNode.SelectSingleNode("//td[@id='Severity']");
				UpdateSeverity.Content = Severity.InnerText.ToString();


				HtmlNode Link = doc.DocumentNode.SelectSingleNode("//td[@id='Link']");
				URL = Link.InnerText.ToString();
			}
			catch (Exception)
			{
			}
			
			if (Properties.Settings.Default.CheckForUpdates == false)
			{
				DoNotShow.IsChecked = true;
			}
		}

		private void WebPageClick(object sender, MouseButtonEventArgs e)
		{
			System.Diagnostics.Process.Start(Transformations.Properties.Resources.UpdatePage);
		}

		private void Dissmiss(object sender, RoutedEventArgs e)
		{
			ClosingFunction();
			this.Close();
		}

		private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ClosingFunction();
		}

		public void ClosingFunction()
		{
			if (DoNotShow.IsChecked == true)
			{
				Properties.Settings.Default.CheckForUpdates = false;
				Properties.Settings.Default.Save();
			}
			else
			{
				Properties.Settings.Default.CheckForUpdates = true;
				Properties.Settings.Default.Save();
			}
		}

		private void Download(object sender, RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(URL);
			}
			catch (Exception)
			{
				System.Diagnostics.Process.Start(Transformations.Properties.Resources.UpdatePage);
			}
		}
	}
}
