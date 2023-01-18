using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{

	internal sealed class NotYetSettingsException : ExceptionBase
	{
		internal override ExceptionKind Kind => ExceptionKind.Warning;

		internal NotYetSettingsException()
			: base("RDBMSへの書き込み対象となるクラスの設定がされていません。\n" +
				  "SQLWriterFacade.SetTargetEntities()で設定してください。")
		{ }
	}
}
