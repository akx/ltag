using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;

namespace LTag
{
	public class PointFConverter : TypeConverter
	{
		private static readonly TypeConverter FloatConverter = TypeDescriptor.GetConverter(typeof(float));
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			culture = culture ?? CultureInfo.CurrentCulture;
			if (value == null) return base.ConvertFrom(context, culture, value);
			var str = value.ToString().Trim();
			if (str.Length == 0) return null;
			var ch = culture.TextInfo.ListSeparator[0];
			var numArray = new List<float>();
			foreach (var s in str.Split(new[] { ch }))
			{
				var converted = FloatConverter.ConvertFromString(context, culture, s);
				if (converted != null) numArray.Add((float)converted);
			}
			if (numArray.Count != 2) throw new ArgumentException("Invalid format");
			return new PointF(numArray[0], numArray[1]);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null) throw new ArgumentNullException("destinationType");
			if (!(value is PointF)) return base.ConvertTo(context, culture, value, destinationType);
			if (destinationType == typeof(string))
			{
				var point = (PointF)value;
				if (culture == null) culture = CultureInfo.CurrentCulture;
				var separator = culture.TextInfo.ListSeparator + " ";
				var strArray = new[]
				{
					FloatConverter.ConvertToString(context, culture, point.X),
					FloatConverter.ConvertToString(context, culture, point.Y)
				};
				return string.Join(separator, strArray);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				var point2 = (PointF)value;
				var constructor = typeof(PointF).GetConstructor(new[] { typeof(float), typeof(float) });
				if (constructor != null) return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (propertyValues == null) throw new ArgumentNullException("propertyValues");
			object xvalue = propertyValues["X"];
			object yvalue = propertyValues["Y"];
			if (((xvalue == null) || (yvalue == null)) || (!(xvalue is float) || !(yvalue is float)))
			{
				throw new ArgumentException("Invalid property value entry");
			}
			return new PointF((float)xvalue, (float)yvalue);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(PointF), attributes).Sort(new[] { "X", "Y" });
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}