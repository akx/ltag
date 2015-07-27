using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LTag
{
	class IniSerializer
	{
		private readonly Dictionary<string, string> _values = new Dictionary<string, string>(); 

		public void WriteObject(string title, object obj)
		{
			var type = obj.GetType();
			foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
			{
				var typeConverter = GetPropTypeConverter(prop);
				if (typeConverter == null) continue;
				var value = prop.GetValue(obj, null);
				var sValue = typeConverter.ConvertToInvariantString(value);
				sValue = sValue ?? Convert.ToString(value, CultureInfo.InvariantCulture);
				_values[String.Format("{0}.{1}", title, prop.Name)] = sValue;
			}
		}

		private static TypeConverter GetPropTypeConverter(PropertyInfo prop)
		{
			var browsableAttrs = prop.GetCustomAttributes(typeof (BrowsableAttribute), true);
			if (browsableAttrs.Any(ba => ((BrowsableAttribute) ba).Browsable == false)) return null;
			var typeConverterAttribute = prop.GetCustomAttributes(typeof (TypeConverterAttribute), true).FirstOrDefault() as TypeConverterAttribute;
			if (typeConverterAttribute != null)
			{
				var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
				if (converterType != null)
				{
					return Activator.CreateInstance(converterType) as TypeConverter;
				}
			}
			return TypeDescriptor.GetConverter(prop.PropertyType);
		}

		public void UpdateObject(string title, object obj)
		{
			var type = obj.GetType();
			foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
			{
				var key = String.Format("{0}.{1}", title, prop.Name);
				if (!_values.ContainsKey(key)) continue;
				var typeConverter = GetPropTypeConverter(prop);
				if (typeConverter == null) continue;
				var sValue = _values[key];
				var value = typeConverter.ConvertFromInvariantString(sValue);
				prop.SetValue(obj, value, null);
			}
		}

		public void Parse(String content)
		{
			foreach (var line in content.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
			{
				var l = line.Trim();
				if (l.Length == 0 || l.StartsWith("#")) continue;
				var bits = l.Split(new[] {'='}, 2).Select(s => s.Trim()).ToArray();
				if (bits.Length != 2) continue;
				_values[bits[0]] = bits[1];
			}
		}

		public String GetValue()
		{
			var sb = new StringBuilder();
			var items = new List<KeyValuePair<string, string>>(_values);
			items.Sort((a, b) => String.Compare(a.Key, b.Key, StringComparison.Ordinal));
			foreach (var item in items)
			{
				sb.AppendFormat("{0} = {1}\n", item.Key, item.Value);
			}
			return sb.ToString();
		}
	}
}
