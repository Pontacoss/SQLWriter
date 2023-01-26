using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SQLWriter.Entities;

namespace SQLWriter.Interfaces
{
	internal interface ISQLBuilder
	{
		string CreateSQL(IList<PropertyEntity> columns, string tableName);
		string UpdateSQL(IList<PropertyEntity> columns, string tableName);
		string InsertSQL(IList<PropertyEntity> columns, string tableName);
		string DeleteSQL(IList<PropertyEntity> columns, string tableName);
		string QuerySQL(IList<PropertyEntity> columns, string tableName);
		string ConvertFileterToSQLString(SQLFilter filter);
		string ConvertFileterToSQLString(List<SQLFilter> filters);
		string GetRDBMSParameterName(PropertyInfo p);
	}
}
