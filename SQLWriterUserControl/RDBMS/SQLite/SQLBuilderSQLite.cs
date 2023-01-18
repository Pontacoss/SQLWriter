using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SQLWriter.Entities;
using SQLWriter.Interfaces;

namespace SQLWriter.RDBMS.SQLite
{
	internal class SQLBuilderSQLite : ISQLBuilder
	{
		public string CreateSQL(IList<PropertyEntity> columns, string tableName)
		{
			var sb = new StringBuilder();
			sb.Append($"create table [{tableName}](");
			int enter = 0;
			foreach (PropertyEntity c in columns)
			{
				string str;
				if (c.DataType == EnumDataType.TEXT)
				{
					str = $" VARCHAR({c.WordCount}),";
				}
				else
				{
					str = $" {c.DataType},";
				}
				sb.Append($"[{c.Name}]" + str);
				enter++;

			}
			sb.Length--;
			var sb_Unique = new StringBuilder();
			foreach (PropertyEntity c in columns)
			{
				if (c.IsUnique == true) sb_Unique.Append($"[{c.Name}],");
			}
			if (sb_Unique.Length > 0)
			{
				sb_Unique.Length--;
				sb.Append($",UNIQUE ({sb_Unique})");
			}

			var sb_Key = new StringBuilder();
			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == true) sb_Key.Append($"[{c.Name}],");
			}
			if (sb_Key.Length > 0)
			{
				sb_Key.Length--;
				sb.Append($",Primary Key ({sb_Key})");
			}
			sb.Append(")");

			return sb.ToString();
		}

		public string UpdateSQL(IList<PropertyEntity> columns, string tableName)
		{
			var sb = new StringBuilder();
			sb.Append($"update [{tableName}] set ");

			var key_counter = columns.Count(c => c.IsPrimaryKey == true);

			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == true) continue;
				sb.Append($"[{c.Name}]=@{c.Name},");
			}
			sb.Length--;

			var sb_Key = new StringBuilder();
			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == false) continue;
				sb_Key.Append($"[{c.Name}]=@{c.Name} and ");
			}
			if (sb_Key.Length > 0)
			{
				sb_Key.Length = sb_Key.Length - 4;
				sb.Append($" where {sb_Key.ToString()}");
			}
			return sb.ToString();
		}

		public string InsertSQL(IList<PropertyEntity> columns, string tableName)
		{
			var sb = new StringBuilder();
			var sb_val = new StringBuilder();
			sb.Append($"insert into [{tableName}](");

			var key_counter = columns.Count(c => c.IsPrimaryKey == true && c.IsAutoincrement == true);

			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == true && c.IsAutoincrement == true) continue;
				sb.Append($"[{c.Name}],");
				sb_val.Append($"@{c.Name},");
			}
			sb.Length--;
			sb_val.Length--;
			sb.Append($") values({sb_val})");

			return sb.ToString();
		}

		public string DeleteSQL(IList<PropertyEntity> columns, string tableName)
		{
			var sb = new StringBuilder();
			var sb_val = new StringBuilder();
			sb.Append($"Delete from [{tableName}] where ");
			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == false) continue;
				sb.Append($"[{c.Name}]=@{c.Name} and ");
			}
			sb.Length -= 5;
			return sb.ToString();
		}

		public string QuerySQL(IList<PropertyEntity> columns, string tableName)
		{
			var sb = new StringBuilder();
			sb.Append($"select ");
			foreach (PropertyEntity c in columns)
			{
				sb.Append($"[{c.Name}],");
			}
			sb.Length--;
			sb.Append($" from [{tableName}]");

			return sb.ToString();
		}
		public string ConvertFileterToSQLString((string propertyName, string operatorStrig, object filteringValue) filter)
		{
			var filters = new List<(string propertyName, string operatorStrig, object filteringValue)>();
			filters.Add(filter);

			return ConvertFileterToSQLString(filters);
		}

		public string ConvertFileterToSQLString(List<(string propertyName, string operatorStrig, object filteringValue)> filters)
		{
			var sb = new StringBuilder();

			sb.Append(" where (");
			foreach (var filter in filters)
			{ 
				if (bool.TryParse(filter.filteringValue.ToString(), out bool result))
					sb.Append("[" + filter.propertyName + "]" + filter.operatorStrig  + result + " and ");
				else
					sb.Append("[" + filter.propertyName + "]" + filter.operatorStrig + "\'" + filter.filteringValue + "\' and ");
			}
			sb.Length -= 4;
			sb.Append(")");

			return sb.ToString();
		}

		public string GetRDBMSParameterName(PropertyInfo p) => "@" + p.Name;
	}
}
