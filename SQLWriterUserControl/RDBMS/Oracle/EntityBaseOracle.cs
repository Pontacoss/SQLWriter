using System.Collections.Generic;
using System.Linq;
using SQLWriter;
using SQLWriter.InnerDb;
using SQLWriter.Repositories;

namespace SQLWriter.RDBMS.SQLite
{
	internal class EntityBaseOracle : ISQLWriterRepository
	{
		static readonly EnumRdbms RDBMS = EnumRdbms.SQLite;

		public void Delete<T>(T x) where T : class
		{
			OracleHelper.Delete(x);
		}

		public int Update<T>(T x) where T : class
		{
			return OracleHelper.Update(x);
		}

		/// <summary>
		/// Insert	の実行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <returns>Insertに失敗した場合 0、成功した場合は挿入された行のROWIDを返却。</returns>
		public int Insert<T>(T x) where T : class
		{
			var intVal = OracleHelper.Insert(x);
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
			var intVal = OracleHelper.Execute(x);

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
			return OracleHelper.Query<T>(SQLStringSQLite.GetSQL(RDBMS, typeof(T).Name, EnumDbAction.Query));
		}

		public List<T> Load<T>(SQLFilter filter) where T : class
		{
			return OracleHelper.Query<T>(SQLStringSQLite.GetSQL(RDBMS, typeof(T).Name, EnumDbAction.Query, filter));
		}

		public List<T> Load<T>(List<SQLFilter> filters) where T : class
		{
			return OracleHelper.Query<T>(SQLStringSQLite.GetSQL(RDBMS, typeof(T).Name, EnumDbAction.Query, filters));
		}

		public List<T> Query<T>(string sql) where T : class
		{
			return SQLiteHelper.Query<T>(sql);
		}
	}
}
