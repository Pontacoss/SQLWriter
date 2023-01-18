using SQLWriter.RDBMS.SQLite;
using System.Collections.Generic;
using SQLWriter.InnerDb;

namespace SQLWriter.Entities
{
	public class SQLStringEntity
	{
		public EnumRdbms Rdbms { get; set; }
		public string TableName { get; set; }
		public EnumDbAction Action { get; set; }
		public string SQLString { get; set; }

		public SQLStringEntity() { }
		public SQLStringEntity(EnumRdbms rdbms,IList<PropertyEntity> columns, string tableName, EnumDbAction action)
		{
			Rdbms = rdbms; //todo:要変更
			TableName = tableName;
			Action = action;
			if (Action == EnumDbAction.Create) SQLString = SQLWriterHelper.GetSQLString(Rdbms).CreateSQL(columns, tableName);
			else if (Action == EnumDbAction.Update) SQLString = SQLWriterHelper.GetSQLString(Rdbms).UpdateSQL(columns, tableName);
			else if (Action == EnumDbAction.Insert) SQLString = SQLWriterHelper.GetSQLString(Rdbms).InsertSQL(columns, tableName);
			else if (Action == EnumDbAction.Delete) SQLString = SQLWriterHelper.GetSQLString(Rdbms).DeleteSQL(columns, tableName);
			else if (Action == EnumDbAction.Query) SQLString = SQLWriterHelper.GetSQLString(Rdbms).QuerySQL(columns, tableName);
			
		}
	}
}