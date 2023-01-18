using SQLWriter.Repositories;
using SQLWriter.RDBMS.SQLite;

namespace SQLWriter
{
	public class SQLWriterFactories
	{
		public static ISQLWriterRepository CreateEntity(EnumRdbms rdbms)
		{
			if (rdbms == EnumRdbms.SQLite) return new EntityBaseSQLite();
			else return null;
		}
	}
}
