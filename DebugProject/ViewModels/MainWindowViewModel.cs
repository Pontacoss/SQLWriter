using Prism.Mvvm;
using Prism.Regions;
using SQLWriter;
using SQLWriter.Entities;

namespace DebugProject.ViewModels
{
	public class MainWindowViewModel : BindableBase
	{
		private IRegionManager _regionManager; 
		public MainWindowViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			_regionManager.RegisterViewWithRegion("ContentRegion", nameof(SQLWriterTableSettingView));
		}
	}
}
