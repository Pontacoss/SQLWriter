using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{
	internal sealed class ValueObjectNotExistValueException : ExceptionBase
	{
		internal ValueObjectNotExistValueException(string name)
			: base("ValueObject：" + name + " にValueプロパティが設定されていません。")
		{ }

		internal override ExceptionKind Kind => ExceptionKind.Warning;
	}
}
