using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter
{
	public sealed class SQLWriterDatabaseSettingViewModel : BindableBase, INavigationAware
	{
		public bool IsNavigationTarget(NavigationContext navigationContext) => false;


		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
		}
	}
}
