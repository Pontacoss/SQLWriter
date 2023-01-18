using DebugProject.Views;
using Prism.Ioc;
using SQLWriter;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace DebugProject
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		public App()
		{

			SQLWriterFacade.SetReferenceToDomain
				(" FullPath for Domain ", "Domain.dll",	" namespace "); // please rewrite

		}

		protected override Window CreateShell()
		{
			return Container.Resolve<MainWindow>();
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<SQLWriterTableSettingView, SQLWriterTableSettingViewModel>();
		}
	}
}
