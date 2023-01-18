using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{
	internal sealed class CallUnregisteredTableException : ExceptionBase
	{
		internal override ExceptionKind Kind => ExceptionKind.Warning;

		internal CallUnregisteredTableException()
			: base("先に実行を押してください。")
		{ }
	}
}
