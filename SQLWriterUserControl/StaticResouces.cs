
using System.Linq;
using System;

namespace SQLWriter
{
	public enum EnumDbAction
	{
		Create,
		Update,
		Insert,
		Delete,
		Query,
	}
	public enum EnumRdbms
	{
		SQLite,
	}

	public enum EnumDataType
	{
		TEXT,
		INTEGER,
		BOOLEAN,
		REAL,
		DATETIME
	}

	public class SQLFilter
	{
		/// <summary>
		/// nameof(クラス名.パラメータ名)　または　"パラメータ名"
		/// </summary>
		public string Name { get; }
		public object Value { get; }
		public string Operator { get; }

		/// <summary>
		/// QueryのWhere句を設定。
		/// </summary>
		/// <param name="name">パラメータ名を指定。nameof(クラス名.パラメータ名)　または　"パラメータ名"</param>
		/// <param name="value">value=nullの場合、NotImplementedException(未実装例外)を吐きます。</param>
		/// <param name="operatorString">演算子を指定します。規定値は "＝"(イコール)です。</param>
		/// <exception cref="NotImplementedException"></exception>
		public SQLFilter(string name, object value, string operatorString = "=")
		{
			Name = name;
			Operator = operatorString;

			if (value == null)
				throw new NotImplementedException();

			var baseType = value.GetType().BaseType;
			if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.Contains("ValueObject"))
			{
				var ps = value.GetType().GetProperties().Single(
					x => x.Name == "Value" && x.DeclaringType.IsAbstract == false);
				Value = ps.GetValue(value);
			}
			else
				Value = value;

		}
	}
}
