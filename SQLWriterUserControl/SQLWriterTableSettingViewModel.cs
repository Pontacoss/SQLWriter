using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SQLWriter.Entities;
using SQLWriter.InnerDb;
using SQLWriter.RDBMS.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQLWriter
{
	[RegionMemberLifetime(KeepAlive = true)]
	public class SQLWriterTableSettingViewModel:BindableBase ,INavigationAware
	{
		public static string Title => "SQL Writer";

		#region プロパティ設定
		private IReadOnlyList<Type> _classList;
		public IReadOnlyList<Type> ClassList
		{
			get { return _classList; }
			set { SetProperty(ref _classList, value); }
		}

		private Type _selectedClass;
		public Type SelectedClass
		{
			get { return _selectedClass; }
			set { SetProperty(ref _selectedClass, value); }
		}

		private string _tableName = string.Empty;
		public string TableName
		{
			get { return _tableName; }
			set { SetProperty(ref _tableName, value); }
		}

		private string _sqlTextBox = string.Empty;
		public string SQLTextBox
		{
			get { return _sqlTextBox; }
			set { SetProperty(ref _sqlTextBox, value); }
		}

		private ObservableCollection<PropertyEntity> _columnInfos = new ObservableCollection<PropertyEntity>();
		public ObservableCollection<PropertyEntity> ColumnInfos
		{
			get { return _columnInfos; }
			set { SetProperty(ref _columnInfos, value); }
		}
		#endregion

		private ClassInfoEntity SelectedClassInfo;

		#region コンストラクター
		public SQLWriterTableSettingViewModel()
		{
			ExecuteButton_Click = new DelegateCommand(ExecuteButton_ClickExecute);
			ClassSelectionChanged = new DelegateCommand(ClassSelectionChangedExecute);
			CreateTableButton_Click = new DelegateCommand(CreateTableButton_ClickExecute);
			ResetSQLWriterTableButton_Click = new DelegateCommand(ResetSQLWriterTableButton_ClickExecute);
			ClassList = ClassInfoEntity.GetAllClassList();
			

		}
		#endregion

		public DelegateCommand ResetSQLWriterTableButton_Click { get; }
		private void ResetSQLWriterTableButton_ClickExecute()
		{
			SQLWriterFacade.InitializeInnerDB();
		}
		public DelegateCommand ExecuteButton_Click { get; }
		private void ExecuteButton_ClickExecute()
		{

			//SQLの作成と保存
			SQLTextBox = string.Empty;
			var list = ColumnInfos.ToList();
			foreach (EnumDbAction action in Enum.GetValues(typeof(EnumDbAction)))
			{
				var className = SelectedClassInfo.ClassName;
				foreach (EnumRdbms rdbms in Enum.GetValues(typeof(EnumRdbms)))
				{
					var table = new SQLStringEntity(rdbms, list, TableName, action);
					SQLWriterHelper.Save(table);
				}
				
			}
			SQLTextBox += ClassInfoEntity.GetCreateSQL(list, TableName);
			SQLTextBox += ClassInfoEntity.GetUpdateSQL(list, TableName);
			SQLTextBox += ClassInfoEntity.GetInsertSQL(list, TableName);
			SQLTextBox += ClassInfoEntity.GetDeleteSQL(list, TableName);
			SQLTextBox += ClassInfoEntity.GetQuerySQL(list, TableName);

			// プロパティ設定値の保存
			foreach (var prop in ColumnInfos)
			{
				SQLWriterHelper.Save(prop);
			}

			SelectedClassInfo.ChangeTableName(TableName);
			SQLWriterHelper.Save(SelectedClassInfo);
		}

		public DelegateCommand CreateTableButton_Click { get; }
		private void CreateTableButton_ClickExecute()
		{
			if (SelectedClass == null) return;
			SQLiteHelper.RecreateTable(SelectedClass,TableName);
			MessageBox.Show("OK");
		}

		public DelegateCommand ClassSelectionChanged { get; }
		private void ClassSelectionChangedExecute()
		{
			var p = new SQLFilter(nameof(PropertyEntity.EntityName), SelectedClass.Name);
			var propDBValue = SQLWriterHelper.Load<PropertyEntity>(p);
			SelectedClassInfo = new ClassInfoEntity(SelectedClass);
			ColumnInfos.Clear();

			var propertySetting = SQLWriterHelper.GetPropertyList(SelectedClass);

			var type = Type.GetType(SelectedClass.AssemblyQualifiedName);

			// プロパティの設定値について前回設定値があれば引用
			foreach (var prop in propertySetting)
			{
				
				if (propDBValue != null)
				{
					var previusValue = propDBValue
						.Where(x => x.Name == prop.Name).FirstOrDefault();
					if (previusValue != null)
					{
						prop.IsPrimaryKey = previusValue.IsPrimaryKey;
						prop.IsAutoincrement = previusValue.IsAutoincrement;
						prop.IsUnique = previusValue.IsUnique;
						prop.WordCount = previusValue.WordCount;
					}

					//文字数制限に関しては、
					//System.ComponentModel.DataAnnotationsによる
					//Domain側での設定を優先する。
					if (Attribute.GetCustomAttribute(
						type.GetProperty(prop.Name), 
						typeof(StringLengthAttribute))
						is StringLengthAttribute lenAttr)
					{
						prop.WordCount = lenAttr.MaximumLength;
					}

				}
				ColumnInfos.Add(prop);
			}

			TableName = SelectedClassInfo.TableName;
			SQLTextBox = string.Empty;
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
		}

		public bool IsNavigationTarget(NavigationContext navigationContext) => true;

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}
	}
}

