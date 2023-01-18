using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SQLWriter.Exceptions;

namespace SQLWriter.Interfaces
{


	internal interface IPropertyType
	{
		object GetValueAccordingToType<T>(PropertyInfo p, T x);
		EnumDataType GetDataType();
		void SetValueIntoInstance(object x, PropertyInfo p, object readerValue);
	}

	internal sealed class TypeInt32 : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.INTEGER;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToInt32(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToInt32(readerValue));
	}
	internal sealed class TypeInt64 : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.INTEGER;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToInt64(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToInt64(readerValue));
	}
	internal sealed class TypeString : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.TEXT;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> p.GetValue(x).ToString();
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, readerValue.ToString());
	}
	internal sealed class TypeBoolean : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.BOOLEAN;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToBoolean(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToBoolean(readerValue));
	}
	internal sealed class TypeEnum : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.INTEGER;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToInt32(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToInt32(readerValue));
	}
	internal sealed class TypeFloat : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.REAL;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToSingle(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToSingle(readerValue));
	}
	internal sealed class TypeDouble : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.REAL;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToDouble(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToDouble(readerValue));
	}
	internal sealed class TypeDateTime : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.DATETIME;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
			=> Convert.ToDateTime(p.GetValue(x));
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
			=> p.SetValue(x, Convert.ToDateTime(readerValue));
	}

	internal sealed class TypeValueObjectInt32 : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.INTEGER;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj); 
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(int) });
			if (constructor == null) 
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { Convert.ToInt32(readerValue) }));
		}
	}
	internal sealed class TypeValueObjectInt64 : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.INTEGER;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(long) });
			if (constructor == null)
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { Convert.ToInt64(readerValue) }));
		}
	}
	internal sealed class TypeValueObjectString : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.TEXT;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(string) });
			if (constructor == null)
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { readerValue.ToString() }));
		}
	}
	internal sealed class TypeValueObjectBoolean : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.BOOLEAN;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(bool) });
			if (constructor == null)
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { Convert.ToBoolean(readerValue) }));
		}
	}
	internal sealed class TypeValueObjectDateTime : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.DATETIME;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(DateTime) });
			if (constructor == null)
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { Convert.ToDateTime(readerValue) }));
		}
	}
	internal sealed class TypeValueObjectFloat : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.REAL;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(float) });
			if (constructor == null)
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { Convert.ToSingle(readerValue) }));
		}
	}
	internal sealed class TypeValueObjectDouble : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.REAL;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var t = p.PropertyType.GetProperties().Single(y => y.Name == "Value");
			if (t == null) throw new ValueObjectNotExistValueException(p.Name);
			object obj = p.GetValue(x);
			if (obj == null) throw new ValueObjectValueException(x.GetType().Name, p.Name);
			return t.GetValue(obj);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var constructor = p.PropertyType.GetConstructor(new Type[] { typeof(double) });
			if (constructor == null)
				throw new NotExistDefaultConstractorException(p.PropertyType.BaseType, p.PropertyType.Name);
			p.SetValue(x, constructor.Invoke(new object[] { Convert.ToDouble(readerValue) }));
		}
	}
	internal sealed class TypeListInt32 : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.TEXT;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var list = p.GetValue(x) as List<int>;
			return string.Join(",", list);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var list = readerValue.ToString().Split(',').ToList();

			var list1 = new List<int>();
			foreach (var i in list)
			{
				if (int.TryParse(i, out int j)) list1.Add(j);
			}
			p.SetValue(x, list1);
		}
	}
	internal sealed class TypeListString : IPropertyType
	{
		public EnumDataType GetDataType()
			=> EnumDataType.TEXT;
		public object GetValueAccordingToType<T>(PropertyInfo p, T x)
		{
			var list = p.GetValue(x) as List<string>;
			return string.Join(",", list);
		}
		public void SetValueIntoInstance(object x, PropertyInfo p, object readerValue)
		{
			var list = readerValue.ToString().Split(',').ToList();
			p.SetValue(x, list);
		}
	}
}
