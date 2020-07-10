using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Zavand.MvcMananaCore
{
    public static class ReflectionExtensions
    {
        public static T[] ToArraySafely<T>(this IEnumerable<T> collection)
        {
            return collection as T[] ?? collection.ToArray();
        }

        public static string ToArrayString(this IEnumerable<byte> data)
        {
            return String.Join("",data.Select(m=>m.ToString("X2")));
        }

        public static void SetValue(PropertyInfo p, object instance, IEnumerable<string> values)
        {
            try
            {
                var vv = values?.ToArraySafely();

                var value = vv.FirstOrDefault();
                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(instance, value);
                }
                else if (p.PropertyType.IsArray && p.PropertyType.HasElementType || typeof(IEnumerable).IsAssignableFrom(p.PropertyType))
                {
                    var t = p.PropertyType.GetElementType();
                    if (t == null)
                        t = p.PropertyType.GenericTypeArguments.First();
                    var converter = TypeDescriptor.GetConverter(t);
                    var oo = new List<object>();
                    foreach (var v in vv)
                    {
                        if (v != null)
                        {
                            try
                            {
                                var converted = converter.ConvertFrom(v);
                                oo.Add(converted);
                            }
                            catch
                            {
                                // skip
                            }
                        }
                        else if (t.IsClass || Nullable.GetUnderlyingType(t) != null)
                        {
                            oo.Add(null);
                        }
                    }

                    var ar = Array.CreateInstance(t, oo.Count);

                    Array.Copy(oo.ToArray(), ar, oo.Count);

                    p.SetValue(instance, ar);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var converted = converter.ConvertFrom(value);
                    p.SetValue(instance, converted);
                }
            }
            catch
            {
                // skip
            }
        }

        public static void SetValue(IEnumerable<PropertyInfo> properties, string propertyName, object instance, IEnumerable<string> values)
        {
            var p = properties.FirstOrDefault(m => m.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (p == null)
                return;
            SetValue(p, instance, values);
        }

        public static void SetValue(string propertyName, object instance, IEnumerable<string> values)
        {
            var t = instance.GetType();
            var pp = t.GetProperties()
                .Where(m => m.CanWrite)
                .ToArray();

            SetValue(pp,propertyName,instance,values);
        }

        public static void SetValue(string propertyName, object instance, string value)
        {
            SetValue(propertyName, instance, new[] { value });
        }

        public static void SetValue(IEnumerable<PropertyInfo> properties, string propertyName, object instance, string value)
        {
            SetValue(properties, propertyName, instance, new[] { value });
        }
    }
}
