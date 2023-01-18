using SQLWriter.Exceptions;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SQLWriter
{
	/// <summary>
	/// Interaction logic for RdbmsTableSettingView
	/// </summary>
	public partial class SQLWriterTableSettingView : UserControl
	{
		public SQLWriterTableSettingView()
		{
			this.Dispatcher.UnhandledException += App_DispatcherUnhandledException;
			//AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
			//{
			//	ReportException("FirstChanceException.log", sender, args.Exception);
			//};

			InitializeComponent();
		}
		//private static void ReportException(string fileName, object sender, Exception exception)
		//{
		//	const string reportFormat =
		//	"===========================================================\r\n" +
		//	"ERROR Date = {0}, Sender = {1}, \r\n" +
		//	"{2}\r\n\r\n";

		//	var reportText = string.Format(reportFormat, DateTimeOffset.Now, sender, exception);
		//	File.AppendAllText(fileName, reportText);
		//}
		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBoxImage icon = MessageBoxImage.Error;
			string caption = "エラー";
			var exceptionBase = e.Exception as ExceptionBase;
			if (exceptionBase != null)
			{
				if (exceptionBase.Kind == ExceptionBase.ExceptionKind.Info)
				{
					icon = MessageBoxImage.Information;
					caption = "情報";
				}
				else if (exceptionBase.Kind == ExceptionBase.ExceptionKind.Warning)
				{
					icon = MessageBoxImage.Warning;
					caption = "警告";
				}
			}
			MessageBox.Show(e.Exception.Message, caption, MessageBoxButton.OK, icon);
			e.Handled = true;
		}
	}
}
