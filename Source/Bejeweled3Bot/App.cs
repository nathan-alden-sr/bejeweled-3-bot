using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace NathanAlden.Bejeweled3Bot
{
	public partial class App
	{
		public static Process Bejeweled3Process
		{
			get;
			private set;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Bejeweled3Process = Process.GetProcesses().SingleOrDefault(arg => arg.MainWindowTitle == "Bejeweled 3");

			if (Bejeweled3Process == null)
			{
				MessageBox.Show("An instance of Bejeweled 3 must be running.", "Bejeweled 3 Bot", MessageBoxButton.OK, MessageBoxImage.Error);
				Shutdown(1);
			}
			else
			{
				StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
			}
		}
	}
}