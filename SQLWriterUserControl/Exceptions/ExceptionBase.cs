using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLWriter.Exceptions
{
	internal abstract class ExceptionBase : Exception
	{
		internal abstract ExceptionKind Kind { get; }

		internal enum ExceptionKind
		{
			Info,
			Warning,
			Error,
		}

		internal ExceptionBase(string message)
			: base(message) { }

		internal ExceptionBase(string message, Exception exception)
			: base(message, exception) { }
	}
}
