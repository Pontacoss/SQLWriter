
using SQLWriter.Interfaces;

namespace SQLWriter.Entities
{
	public class PropertyEntity /*: INotifyPropertyChanged*/
	{
		//public event PropertyChangedEventHandler PropertyChanged;
		//protected bool RaisePropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		//{
		//	if (Equals(field, value)) return false;

		//	field = value;

		//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		//	return true;
		//}

		public string EntityName { get; set; }
		public string Name { get; set; } = string.Empty;

		public EnumDataType DataType { get; set; } = EnumDataType.INTEGER;

		public int WordCount { get; set; }

		public bool IsPrimaryKey { get; set; } = false;

		public bool IsAutoincrement { get; set; } = false;

		public bool IsUnique { get; set; } =false;

		public PropertyEntity(){}
		public PropertyEntity(string entityName, string name, EnumDataType dataType)
		{
			EntityName = entityName;
			Name = name;
			DataType = dataType;
		}

		public PropertyEntity(string entityName, string name,
			EnumDataType dataType, int wordCount, bool isPrimaryKey, 
			bool isAutoincrement, bool isUnique) : this(entityName, name, dataType)
		{
			WordCount = wordCount;
			IsPrimaryKey = isPrimaryKey;
			IsAutoincrement = isAutoincrement;
			IsUnique = isUnique;
		}
	}
}
