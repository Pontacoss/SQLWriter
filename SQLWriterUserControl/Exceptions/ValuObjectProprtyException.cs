using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{
	internal sealed class ValuObjectProprtyException : Exception
	{
		internal ValuObjectProprtyException(string name) : base(
			name +" にプロパティ Value がありません。")	{}
	}
}
