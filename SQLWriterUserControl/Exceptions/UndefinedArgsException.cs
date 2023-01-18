using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{
	internal sealed class UndefinedArgsException : ExceptionBase
	{
		internal override ExceptionKind Kind => ExceptionKind.Warning;

		internal UndefinedArgsException()
			: base("引数が不正です。")
		{ }
	}
}
