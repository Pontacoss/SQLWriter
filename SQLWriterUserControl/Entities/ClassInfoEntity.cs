using SQLWriter.Exceptions;
using SQLWriter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SQLWriter.Entities
{
	public sealed class ClassInfoEntity
	{
		public string ClassName { get; set; }
		public string TableName { get; set; }

		public ClassInfoEntity(){}

		public ClassInfoEntity(Type type)
		{
			ClassName = type.Name;
			TableName = type.Name.Replace("Entity","");
		}

		internal void ChangeTableName(string tableName)
		{
			TableName = tableName;
		}

		internal static IReadOnlyList<Type> GetSQLWriterEntityList()
		{
			Assembly assm = Assembly.LoadFrom("SQLWriter.dll");

			var types = assm.GetTypes()
				.Where(t => t.Namespace != null)
				.Where(p => p.Namespace.Contains("SQLWriter.Entities"))
				.Where(q => q.Name.Contains("Entity"))
				.Where(q => q.IsClass == true)
				.Select(s => s).ToList();

			return types as IReadOnlyList<Type>;
		}
		internal static IReadOnlyList<Type> GetAllClassList()
		{
			if (SQLWriterFacade.AssemblyFile == null)
			{
				throw new NotYetSettingsException();
			}
			Assembly assm = Assembly.LoadFrom(SQLWriterFacade.AssemblyFilePath + 
				SQLWriterFacade.AssemblyFile);

			var types = assm.GetTypes()
				.Where(t => t.Namespace != null)
				.Where(p => p.Namespace.Contains(SQLWriterFacade.NameSpace))
				.Where(q => q.IsPublic == true)
				.OrderBy(o => o.Name)
				.Select(s => s).ToList();

			return types as IReadOnlyList<Type>;
		}

		internal static string GetCreateSQL(IList<PropertyEntity> columns, string tableName) 
		{
			var sb = new StringBuilder();
			sb.Append($"public string CreateTable =>\n\t@\"create table [{tableName}](");
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
				if (enter == 4 && columns.IndexOf(c) + 1 != columns.Count)
				{
					enter = 0;
					sb.Append("\n\t\t\t\t");
				}
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

			sb.Append(")\";\n");

			return sb.ToString();
		}

		internal static string GetUpdateSQL(IList<PropertyEntity> columns, string tableName) 
		{
			var sb = new StringBuilder();
			sb.Append($"public string Update =>\n\t@\"update [{tableName}] set ");
			var enter = 0;

			var key_counter = columns.Count(c => c.IsPrimaryKey == true);

			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == true) continue;
				sb.Append($"[{c.Name}]=@{c.Name},");
				enter++;
				if (enter == 4 && columns.IndexOf(c) + 1 != columns.Count - key_counter)
				{
					enter = 0;
					sb.Append("\n\t\t\t\t");
				}
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
				sb.Append($"\n\t\t\twhere {sb_Key.ToString()}");
			}

			sb.Append("\";\n");

			return sb.ToString();
		}

		internal static string GetInsertSQL(IList<PropertyEntity> columns, string tableName) 
		{
			var sb = new StringBuilder();
			var sb_val = new StringBuilder();
			sb.Append($"public string Insert =>\n\t@\"insert into [{tableName}]\n\t\t\t\t(");
			var enter = 0;

			var key_counter = columns.Count(c => c.IsPrimaryKey == true && c.IsAutoincrement == true);

			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == true && c.IsAutoincrement == true) continue;
				sb.Append($"[{c.Name}],");
				sb_val.Append($"@{c.Name},");
				enter++;
				if (enter == 7 && columns.IndexOf(c) + 1 != columns.Count - key_counter)
				{
					enter = 0;
					sb.Append("\n\t\t\t\t");
					sb_val.Append("\n\t\t\t\t");
				}
			}
			sb.Length--;
			sb_val.Length--;
			sb.Append($")\n\t\t\tvalues\n\t\t\t\t({sb_val})\";\n");

			return sb.ToString();
		}

		internal static string GetDeleteSQL(IList<PropertyEntity> columns, string tableName) 
		{
			var sb = new StringBuilder();
			var sb_val = new StringBuilder();
			sb.Append($"public string Delete =>\n\t@\"Delete from [{tableName}] where ");
			foreach (PropertyEntity c in columns)
			{
				if (c.IsPrimaryKey == false) continue;
				sb.Append($"[{c.Name}]=@{c.Name} and ");
			}
			sb.Length -= 5;
			sb.Append("\";\n");
			return sb.ToString();
		}

		internal static string GetQuerySQL(IList<PropertyEntity> columns, string tableName) 
		{
			var sb = new StringBuilder();
			sb.Append($"public string Query =>\n\t@\"select ");
			var enter = 0;
			foreach (PropertyEntity c in columns)
			{
				sb.Append($"[{c.Name}],");
				enter++;
				if (enter == 7 && columns.IndexOf(c) + 1 != columns.Count)
				{
					enter = 0;
					sb.Append("\n\t\t\t\t");
				}
			}
			sb.Length--;
			sb.Append($"\n\t\t\tfrom [{tableName}]\";\n");

			return sb.ToString();
		}
	}
}
