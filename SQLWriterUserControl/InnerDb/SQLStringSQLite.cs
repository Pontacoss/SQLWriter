using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SQLWriter.Entities;
using SQLWriter.Exceptions;

namespace SQLWriter.InnerDb
{
	internal class SQLStringSQLite
	{
		internal static string GetSQLWriterSQL(string entityName, EnumDbAction action,
			(string propertyName, string operatorStrig, object filteringValue) filter)
		{
			var list = new List<(string propertyName, string operatorStrig, object filteringValue)>();
			list.Add(filter);

			return GetSQLWriterSQL(entityName, action, list);
		}

		internal static string GetSQLWriterSQL(string entityName, EnumDbAction action, 
			List<(string propertyName, string operatorStrig, object filteringValue)> filters)
		{
			var str = GetSQLWriterSQL(entityName, action);
			var sql = str + SQLWriterHelper.GetSQLString(SQLWriterHelper.RDBMS).ConvertFileterToSQLString(filters);
			return sql;
		}
		internal static string GetSQLWriterSQL(string entityName, EnumDbAction action)
		{
			Type type;
			if (entityName == "SQLStringEntity") type = typeof(SQLStringEntity);
			else if (entityName == "PropertyEntity") type = typeof(PropertyEntity);
			else type = typeof(ClassInfoEntity);

			if (action == EnumDbAction.Create)
				return GetSqlString(type).CreateTable;
			else if (action == EnumDbAction.Update)
				return GetSqlString(type).Update;
			else if (action == EnumDbAction.Insert)
				return GetSqlString(type).Insert;
			else if (action == EnumDbAction.Delete)
				return GetSqlString(type).Delete;
			else
				return GetSqlString(type).Query;
		}

		/// <summary>
		/// 対象RDBMS、クラス、アクションを指定しSQL文を発行する。
		/// </summary>
		/// <param name="rdbms">RDBMS名を指定。Enum化予定。現在「SQLite」のみ。</param>
		/// <param name="className">SQL文発行の対象クラス名を指定。</param>
		/// <param name="action">DBAction.Create,DBAction.Update,DBAction.Insert,DBAction.Delete,DBAction.Queryから選択</param>
		/// <returns></returns>
		internal static string GetSQL(EnumRdbms rdbms, string entityName, EnumDbAction action)
		{
			var tableName = GetTableName(rdbms, entityName);

			var p = new List<(string,string,object)>();
			p.Add((nameof(SQLStringEntity.Rdbms), "=",(int)rdbms));
			p.Add((nameof(SQLStringEntity.TableName), "=", tableName));
			p.Add((nameof(SQLStringEntity.Action), "=", (int)action));

			var entity = SQLWriterHelper.Load<SQLStringEntity>(p).SingleOrDefault();

			if (entity == null) throw new NotImplementedException(); //todo: 例外処理。SQLの登録がないケース
			return entity.SQLString;

		}

		/// <summary>
		/// 対象RDBMS、クラス、アクション、検索方法を1つ指定しSQL文を発行する。
		/// </summary>
		/// <param name="rdbms">RDBMS名を指定。Enum化予定。現在「SQLite」のみ。</param>
		/// <param name="className">SQL文発行の対象クラス名を指定。</param>
		/// <param name="action">DBAction.Create,DBAction.Update,DBAction.Insert,DBAction.Delete,DBAction.Queryから選択</param>
		/// <param name="filter">検索対象プロパティの名称、演算子、値 をタプルで設定。</param>
		/// <returns></returns>
		internal static string GetSQL(EnumRdbms rdbms, string entityName, EnumDbAction action,
			(string propertyName, string operatorStrig, object filteringValue) filter)
		{
			var list = new List<(string propertyName, string operatorStrig, object filteringValue)>();
			list.Add(filter);

			return GetSQL(rdbms, entityName, action, list);
		}

		/// <summary>
		/// 対象RDBMS、クラス、アクション、検索方法を複数指定しSQL文を発行する。
		/// </summary>
		/// <param name="rdbms">RDBMS名を指定。Enum化予定。現在「SQLite」のみ。</param>
		/// <param name="className">SQL文発行の対象クラス名を指定。</param>
		/// <param name="action">DBAction.Create,DBAction.Update,DBAction.Insert,DBAction.Delete,DBAction.Queryから選択</param>
		/// <param name="filters">検索対象プロパティの名称、演算子、値 をタプルで設定。</param>
		/// <returns></returns>
		internal static string GetSQL(EnumRdbms rdbms, string entityName, EnumDbAction action,
			List<(string propertyName, string operatorStrig, object filteringValue)> filters)
		{
			var str = GetSQL(rdbms, entityName, action);
			var sql = str + SQLWriterHelper.GetSQLString(rdbms).ConvertFileterToSQLString(filters);
			return sql;
		}

		private static IGetSqlString GetSqlString(Type x)
		{
			string sqlClass = x.Name.Replace("Entity", "SqlString");
			Type type = Type.GetType("SQLWriter.InnerDb." + sqlClass);
			ConstructorInfo ct = type.GetConstructor(Type.EmptyTypes);

			return ct.Invoke(null) as IGetSqlString;
		}

		private static string GetTableName(EnumRdbms rdbms, string entityName)
		{
			var q = (nameof(ClassInfoEntity.ClassName), "=", entityName);
			//var sql = GetSqlString(typeof(SQLWriterClassInfoEntity)).Query 
			//	+ SQLWriterFactory.GetSQLString(SQLWriterHelper.RDBMS).ConvertFileterToSQLString(q);

			var entity = SQLWriterHelper.Load<ClassInfoEntity>(q).SingleOrDefault();
			if (entity == null) throw new CallUnregisteredTableException(); // todo:　例外処理。登録されていないTableの呼び出し

			return entity.TableName;
		}
	}
}
