using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{

	internal sealed class ValueObjectValueException : ExceptionBase
	{
		internal ValueObjectValueException(string className,string paramNam)
			: base("ValueObject<>にnull値が設定されました。" +
							"\n\n クラス名：" + className + "\n パラメータ名 : " + paramNam){ }

		internal override ExceptionKind Kind => ExceptionKind.Warning;
	}
}
