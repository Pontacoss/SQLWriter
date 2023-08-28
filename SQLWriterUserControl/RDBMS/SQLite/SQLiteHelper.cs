using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.IO;
using SQLWriter.Exceptions;
using SQLWriter.InnerDb;
using SQLWriter.Entities;
using System.Windows;

namespace SQLWriter.RDBMS.SQLite
{
	internal static class SQLiteHelper
	{
		static readonly string dataBaseName = "WpfAppSQLite.db";
		static readonly string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
			@"\MitsubishiElectric\" + System.Diagnostics.Process.GetCurrentProcess().ProcessName;
		//Assembly.GetExecutingAssembly().Location;
		//static readonly string appPath = Path.GetDirectoryName(fullPath);
		static string DataBasePath = Path.Combine(fullPath, dataBaseName);
		static readonly string ConnectionString = @"Data Source=" + DataBasePath + ";Version=3;";
		static readonly EnumRdbms RDBMS = EnumRdbms.SQLite;

		private static Func<SQLiteDataReader, T> GetFunc<T>() where T : class
		{
			return reader =>
			{
				var ps = typeof(T).GetProperties();
				var x = Activator.CreateInstance(typeof(T));
				foreach (var p in ps)
				{
					var type = SQLWriterHelper.PropertyTypeJudgement(p.PropertyType);
					if (type != null) type.SetValueIntoInstance(x, p, reader[p.Name]);
				}
				return x as T;
			};
		}

		private static SQLiteParameter[] GetSQLiteParameters<T>(T x)
		{
			var list = new List<SQLiteParameter>();
			Dictionary<string, object> dic = SQLWriterHelper.GetRDBMSParameters(RDBMS,x);
			foreach (var p in dic.Keys)
			{
				list.Add(new SQLiteParameter(p, dic[p]));
			}
			return list.ToArray();
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
			var update = SQLStringSQLite.GetSQL(RDBMS, type.Name, EnumDbAction.Update);
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
			var insert = SQLStringSQLite.GetSQL(RDBMS, type.Name, EnumDbAction.Insert);
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
			string update = SQLStringSQLite.GetSQL(RDBMS, type.Name, EnumDbAction.Update);
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
					command.CommandText = SQLStringSQLite.GetSQL(RDBMS, type.Name, EnumDbAction.Insert);
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
			var sql = SQLStringSQLite.GetSQL(RDBMS,x.GetType().Name, EnumDbAction.Delete);
			var parameters = GetSQLiteParameters(x);

			using (var connection = new SQLiteConnection(ConnectionString))
			using (var command = new SQLiteCommand(sql, connection))
			{
				connection.Open();
				if (parameters != null) command.Parameters.AddRange(parameters);

				return command.ExecuteNonQuery();
			}
		}

		internal static List<T> Query<T>(string sql) where T : class
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

		internal static void Initialize()
		{
			using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
			{
				con.Open();

				var classList = ClassInfoEntity.GetAllClassList();

				foreach (var cls in classList)
				{
					string sql = $"DROP TABLE IF EXISTS [" + SQLWriterHelper.GetTableName(cls) + "]";
					using (SQLiteCommand com = new SQLiteCommand(sql, con))
					{
						com.ExecuteNonQuery();
					}
					sql = SQLStringSQLite.GetSQL(RDBMS, cls.Name, EnumDbAction.Create);
					if (sql == null) throw new NotExistTableSettingException(cls.Name);
					using (SQLiteCommand com = new SQLiteCommand(sql, con))
					{
						com.ExecuteNonQuery();
					}
				}
			}
		}

		internal static void RecreateTable(Type type,string tableName)
		{
			if(tableName==string.Empty)
			{
				MessageBox.Show("テーブル名を入力してください。");
				return;
			}
			using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
			{
				con.Open();

				string sql = $"DROP TABLE IF EXISTS [" + tableName + "]";
				using (SQLiteCommand com = new SQLiteCommand(sql, con))
				{
					com.ExecuteNonQuery();
				}
				sql = SQLStringSQLite.GetSQL(RDBMS, type.Name, EnumDbAction.Create);
				using (SQLiteCommand com = new SQLiteCommand(sql, con))
				{
					com.ExecuteNonQuery();
				}
			}
		}
	}
}
