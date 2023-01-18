using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using SQLWriter.Entities;
using SQLWriter.Interfaces;
using SQLWriter.RDBMS.SQLite;

namespace SQLWriter.InnerDb
{
	internal class SQLWriterHelper 
	{
		static readonly string dataBaseName = "SQLWriter.db";
		static readonly string fullPath = Assembly.GetExecutingAssembly().Location;
		static readonly string appPath = Path.GetDirectoryName(fullPath);
		//static readonly string appPath = SQLWriterFacade.AssemblyFilePath;
		static string DataBasePath = Path.Combine(appPath, dataBaseName);
		static readonly string ConnectionString = @"Data Source=" + DataBasePath + ";Version=3;";
		internal static readonly EnumRdbms RDBMS = EnumRdbms.SQLite;

		internal static ISQLBuilder GetSQLString(EnumRdbms rdbms)
		{
			if (rdbms == EnumRdbms.SQLite)
			{
				return new SQLBuilderSQLite();
			}
			else
				return new SQLBuilderSQLite();
		}
		internal static IPropertyType PropertyTypeJudgement(Type propertyType)
		{
			if (propertyType == typeof(int)) 
				return new TypeInt32();
			else if (propertyType == typeof(long))
				return new TypeInt64();
			else if (propertyType == typeof(string))
				return new TypeString();
			else if (propertyType == typeof(double))
				return new TypeDouble();
			else if (propertyType == typeof(float))
				return new TypeFloat();
			else if (propertyType == typeof(bool))
				return new TypeBoolean();
			else if (propertyType == typeof(DateTime))
				return new TypeDateTime();
			else if (propertyType.IsEnum)
				return new TypeEnum();
			else if (propertyType.IsClass)
			{
				Type baseType = propertyType.BaseType;
				if (baseType.IsGenericType && baseType.Name.Contains( "ValueObject"))
				{
					var t = propertyType.GetProperties().Single(
						x => x.Name == "Value" && x.DeclaringType.IsAbstract == false);
					
					var type = t.PropertyType;
					if (type == typeof(string)) return new TypeValueObjectString();
					else if (type == typeof(int)) return new TypeValueObjectInt32();
					else if (type == typeof(long)) return new TypeValueObjectInt64();
					else if (type == typeof(double)) return new TypeValueObjectDouble();
					else if (type == typeof(float)) return new TypeValueObjectFloat();
					else if (type == typeof(bool)) return new TypeValueObjectBoolean();
					else if (type == typeof(DateTime)) return new TypeValueObjectDateTime();
				}
				else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
					if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
				{
					var listType = propertyType.GetGenericArguments()[0];
					if (listType == typeof(string)) return new TypeListString();
					else if (listType == typeof(int)) return new TypeListInt32();
				}
			}
			return null;
		}

		internal static List<PropertyEntity> GetPropertyList(Type type)
		{
			var list = new List<PropertyEntity>();
			var ps = type.GetProperties();
			foreach (var p in ps)
			{
				var propType = PropertyTypeJudgement(p.PropertyType);
				if(propType==null) continue;
				list.Add(new PropertyEntity(type.Name, p.Name, propType.GetDataType()));
			}
			return list;
		}

		internal static Dictionary<string, object> GetRDBMSParameters<T>(EnumRdbms rdbms,T x)
		{
			var dic = new Dictionary<string, object>();
			var type = x.GetType();
			var ps = type.GetProperties();

			foreach (var p in ps)
			{
				var proptype = PropertyTypeJudgement(p.PropertyType);
				if (proptype == null) continue;
				dic.Add(SQLWriterHelper.GetSQLString(rdbms).GetRDBMSParameterName(p), proptype.GetValueAccordingToType(p, x));
			}
			return dic;
		}
		

		private static Func<SQLiteDataReader, T> GetFunc<T>() where T : class
		{
			return reader =>
			{
				var ps = typeof(T).GetProperties();
				var x = Activator.CreateInstance(typeof(T));
				foreach (var p in ps)
				{
					var type = PropertyTypeJudgement(p.PropertyType);
					if (type != null) type.SetValueIntoInstance(x, p, reader[p.Name]);
				}
				return x as T;
			};
		}

		private static SQLiteParameter[] GetSQLiteParameters<T>(T x)
		{
			var type = x.GetType();
			var ps = type.GetProperties();

			var list = new List<SQLiteParameter>();
			foreach (var p in ps)
			{
				var proptype = PropertyTypeJudgement(p.PropertyType);
				if (proptype == null) continue;
				list.Add(new SQLiteParameter("@" + p.Name, proptype.GetValueAccordingToType(p, x)));
			}
			return list.ToArray();
		}
		internal static int Save<T>(T x) where T : class
		{
			var intVal = Execute(x);

			if (intVal == 0) return 0;

			// Insertされて戻ってきた場合、かつ 主キーが1つでINTEGER型であれば
			// Autoincrementされた値をインスタンスに入れる。

			var type = x.GetType();

			var props = GetPrimaryKeys(type);
			if (props.Count() == 1)
			{
				var entity = props.Single();
				if (entity.DataType == EnumDataType.INTEGER)
				{
					var obj = x.GetType().GetProperty(entity.Name);
					obj.SetValue(x, intVal);
				}
			}
			return intVal;
		}
		/// <summary>
		/// Updateの実行。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <returns>更新された行数</returns>
		internal static int Update<T>(T x) where T : class
		{
			var type = x.GetType();
			var update = SQLStringSQLite.GetSQLWriterSQL(type.Name, EnumDbAction.Update);
			var parameters = GetSQLiteParameters(x);
			using (var connection = new SQLiteConnection(ConnectionString))
			using (var command = new SQLiteCommand(update, connection))
			{
				connection.Open();
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}
				return command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Insertの実行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <returns>挿入された行のROWID</returns>
		internal static int Insert<T>(T x) where T : class
		{
			var type = x.GetType();
			var insert = SQLStringSQLite.GetSQLWriterSQL(type.Name, EnumDbAction.Insert);
			var parameters = GetSQLiteParameters(x);
			using (var connection = new SQLiteConnection(ConnectionString))
			using (var command = new SQLiteCommand(insert, connection))
			{
				connection.Open();
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}
				command.ExecuteNonQuery();

				string sql = "SELECT last_insert_rowid()";
				SQLiteCommand cmd = new SQLiteCommand(sql, connection);
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		/// <summary>
		/// Update または Insert を実行。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <returns>Updateの場合は0を、Insertの場合InsertされたROWIDを返却。</returns>
		internal static int Execute<T>(T x) where T : class
		{
			var type = x.GetType();
			string update;
			update = SQLStringSQLite.GetSQLWriterSQL(type.Name, EnumDbAction.Update);
			var parameters = GetSQLiteParameters(x);
			using (var connection = new SQLiteConnection(ConnectionString))
			using (var command = new SQLiteCommand(update, connection))
			{
				connection.Open();
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}
				var p = command.ExecuteNonQuery();
				var lastId = 0;
				if (p < 1)
				{
					command.CommandText = SQLStringSQLite.GetSQLWriterSQL(type.Name, EnumDbAction.Insert);
					command.ExecuteNonQuery();

					string sql = "SELECT last_insert_rowid()";
					SQLiteCommand cmd = new SQLiteCommand(sql, connection);
					lastId = Convert.ToInt32(cmd.ExecuteScalar());
				}
				return lastId;
			}
		}

		internal static int Delete<T>(T x)
		{
			var sql = SQLStringSQLite.GetSQLWriterSQL(nameof(T), EnumDbAction.Delete);
			var parameters = GetSQLiteParameters(x);

			using (var connection = new SQLiteConnection(ConnectionString))
			using (var command = new SQLiteCommand(sql, connection))
			{
				connection.Open();
				if (parameters != null) command.Parameters.AddRange(parameters);

				return command.ExecuteNonQuery();
			}
		}

		internal static  List<T> Load<T>() where T : class
		{
			return Query<T>(SQLStringSQLite.GetSQLWriterSQL(typeof(T).Name, EnumDbAction.Query));
		}

		internal static List<T> Load<T>((string propertyName, string operatorStrig,object filteringValue) filter) where T : class
		{
			return Query<T>(SQLStringSQLite.GetSQLWriterSQL(typeof(T).Name, EnumDbAction.Query, filter));
		}
		internal static List<T> Load<T>(List<(string propertyName, string operatorStrig, object filteringValue)> filters) where T : class
		{
			return Query<T>(SQLStringSQLite.GetSQLWriterSQL(typeof(T).Name, EnumDbAction.Query, filters));
		}

		private static List<T> Query<T>(string sql) where T : class
		{
			var list = new List<T>();
			var function = GetFunc<T>();
			using (var connection = new SQLiteConnection(ConnectionString))
			using (var command = new SQLiteCommand(sql, connection))
			{
				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(function(reader));
					}
				}
			}
			return list;
		}

		internal static void InitializeSQLWriterDB()
		{
			using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
			{
				con.Open();

				var sqlwriter = ClassInfoEntity.GetSQLWriterEntityList();

				foreach (var cls in sqlwriter)
				{
					string sql = $"DROP TABLE IF EXISTS [" + cls.Name.Replace("Entity","") + "]";
					using (SQLiteCommand com = new SQLiteCommand(sql, con))
					{
						com.ExecuteNonQuery();
					}
					using (SQLiteCommand com = new SQLiteCommand(
						SQLStringSQLite.GetSQLWriterSQL(cls.Name, EnumDbAction.Create), con))
					{
						com.ExecuteNonQuery();
					}
				}
			}
		}

		internal static List<PropertyEntity> GetPrimaryKeys(Type entity)
		{
			var p = new List<(string, string, object)>();
			p.Add((nameof(PropertyEntity.EntityName), "=", entity.Name));
			p.Add((nameof(PropertyEntity.IsPrimaryKey), "=", true));

			return Load<PropertyEntity>(p);
		}

		internal static string GetTableName(Type type)
		{
			var f = (nameof(ClassInfoEntity.ClassName), "=", type.Name);
			var entity = Load<ClassInfoEntity>(f);
			if (entity.Count == 0) return type.Name.Replace("Entity", "");
			return entity.First().TableName;
		}
	}
}
