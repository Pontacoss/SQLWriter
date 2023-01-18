using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{
	internal sealed class NotExistDefaultConstractorException : ExceptionBase
	{
		internal override ExceptionKind Kind => ExceptionKind.Warning;

		internal NotExistDefaultConstractorException(Type type, string value)
			: base("ValueObject ： " + value + "に " + type.Name + "を引数とするコンストラクターがありません。")
		{ }
	}
}
