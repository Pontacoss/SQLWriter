using System.Collections.Generic;
using System.Linq;
using SQLWriter;
using SQLWriter.InnerDb;
using SQLWriter.Repositories;

namespace SQLWriter.RDBMS.SQLite
{
	internal class EntityBaseSQLite : ISQLWriterRepository
	{
		static readonly EnumRdbms RDBMS = EnumRdbms.SQLite;

		public void Delete<T>(T x) where T : class
		{
			SQLiteHelper.Delete(x);
		}

		public int Update<T>(T x) where T : class
		{
			return SQLiteHelper.Update(x);
		}

		/// <summary>
		/// Insert	の実行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <returns>Insertに失敗した場合 0、成功した場合は挿入された行のROWIDを返却。</returns>
		public int Insert<T>(T x) where T : class
		{
			var intVal = SQLiteHelper.Insert(x);
			if (intVal == 0) return 0;
			var type = x.GetType();

			// 主キーが1つでかつINTEGER型の場合、Autoincrementされた値をインスタンスの主キーに入れる。

			var props = SQLWriterHelper.GetPrimaryKeys(type);

			if (props.Count() == 1)
			{
				var prop = props.Single();
				if (prop.DataType == EnumDataType.INTEGER)
				{
					x.GetType().GetProperty(prop.Name).SetValue(x, intVal);
				}
			}
			return intVal;
		}
		public int Save<T>(T x) where T : class
		{
			var intVal = SQLiteHelper.Execute(x);

			if (intVal == 0) return 0;

			// Insertされて戻ってきた場合、かつ 主キーが1つでINTEGER型であれば
			// Autoincrementされた値をインスタンスに入れる。

			var type = x.GetType();

			var props = SQLWriterHelper.GetPrimaryKeys(type);
			if( props.Count() ==1 )
			{
				var entity=props.Single();
				if(entity.DataType== EnumDataType.INTEGER)
				{
					var obj=x.GetType().GetProperty(entity.Name);
					obj.SetValue(x, intVal);
				}
			}
			return intVal;
		}
		public List<T> Load<T>() where T : class
		{
			return SQLiteHelper.Query<T>(SQLStringSQLite.GetSQL(RDBMS, typeof(T).Name, EnumDbAction.Query));
		}

		public List<T> Load<T>((string, string,object) filter) where T : class
		{
			return SQLiteHelper.Query<T>(SQLStringSQLite.GetSQL(RDBMS, typeof(T).Name, EnumDbAction.Query, filter));
		}

		public List<T> Load<T>(List<(string,string, object)> filters) where T : class
		{
			return SQLiteHelper.Query<T>(SQLStringSQLite.GetSQL(RDBMS, typeof(T).Name, EnumDbAction.Query, filters));
		}
	}
}
