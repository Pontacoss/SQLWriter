using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Entities
{
	internal class DatabaseInfoEntity
	{
		internal EnumRdbms DBKind { get; } 
		internal string DatabaseName { get; set; }
		internal string DatabasePath { get; set; }
		internal string DBConectionString { get; set; }

		public DatabaseInfoEntity(EnumRdbms dBKind)
		{
			DBKind = dBKind;
		}
	}
}
