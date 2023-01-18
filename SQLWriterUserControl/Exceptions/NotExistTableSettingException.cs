using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{

	internal sealed class NotExistTableSettingException : ExceptionBase
	{
		internal override ExceptionKind Kind => ExceptionKind.Warning;

		internal NotExistTableSettingException(string tableName)
			: base(tableName +" は登録されていません。")
		{ }
	}
}
