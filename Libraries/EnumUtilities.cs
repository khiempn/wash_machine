using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Libraries
{
    public static class EnumUtilities
    {
        public static List<SelectItem> RetrieveListItems<T>() where T : struct
        {
            var items = new List<SelectItem>();
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            foreach (var item in values)
            {
                var selectItem = new SelectItem
                {
                    Id = (int)Enum.Parse(typeof(T), item.ToString()),
                };
                selectItem.Name = GetName(item);
                items.Add(selectItem);
            }
            return items;
        }

        private static string GetDescription<T>(T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                return string.Empty;
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static string GetEnumName<T>(this int? value)
        {
            if (value == null) return string.Empty;
            try
            {
                var enumValue = (T)Enum.ToObject(typeof(T), value);
                return enumValue + string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static string GetName<T>(this int? value)
        {
            if (value == null) return string.Empty;
            try
            {
                var enumValue = (T)Enum.ToObject(typeof(T), value);
                return GetName(enumValue);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetName<T>(this decimal? value) where T : struct
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            var lastName = string.Empty;
            foreach (var item in values)
            {
                var name = GetName<T>(item);
                var enumSlug = TextUtilities.ConvertToSlug(name);
                var start = GetValue<T>(enumSlug);
                if (value < start)
                {
                    return lastName;
                }
                lastName = name;
            }
            return string.Empty;
        }

        public static string GetName<T>(this T value)
        {
            try
            {
                var fi = value.GetType().GetField(value.ToString());

                var attributes = (DisplayNameAttribute[])fi.GetCustomAttributes(typeof(DisplayNameAttribute), false);

                return attributes.Length > 0 ? attributes[0].DisplayName : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetName<T>(string slug) where T : struct
        {
            if (string.IsNullOrEmpty(slug)) return string.Empty;
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            foreach (var item in values)
            {
                var name = GetName<T>(item);
                var enumSlug = TextUtilities.ConvertToSlug(name);
                if (enumSlug == slug || item + string.Empty == slug)
                {
                    return name;
                }
            }
            return string.Empty;
        }




        public static string GetAmbient<T>(this int? value)
        {
            if (value == null) return string.Empty;
            try
            {
                var enumValue = (T)Enum.ToObject(typeof(T), value);
                return GetAmbient(enumValue);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetAmbient<T>(this T value)
        {
            try
            {
                var fi = value.GetType().GetField(value.ToString());

                var attributes = (AmbientValueAttribute[])fi.GetCustomAttributes(typeof(AmbientValueAttribute), false);

                return attributes.Length > 0 ? attributes[0].Value + "" : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static Distance GetAmbient<T>(string slug) where T : struct
        {
            var distance = new Distance { Start = 0, End = 0 };
            if (slug == null) return distance;
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            foreach (var item in values)
            {
                var name = GetName<T>(item);
                var enumSlug = TextUtilities.ConvertToSlug(name);
                if (enumSlug == slug)
                {
                    var ambinet = GetAmbient<T>(item);
                    var arr = ambinet.Split('-');
                    distance.Start = int.Parse(arr[0]);
                    distance.End = int.Parse(arr[1]);
                    return distance;
                }
            }
            return distance;
        }



        public static int GetValue<T>(string slug) where T : struct
        {
            if (string.IsNullOrEmpty(slug)) return 0;
            var items = new List<SelectItem>();
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            foreach (var item in values)
            {
                if (item.ToString() == slug)
                {
                    return (int)Enum.Parse(typeof(T), item.ToString());
                }
                var name = GetName<T>(item);
                var enumSlug = TextUtilities.ConvertToSlug(name);
                if (enumSlug == slug)
                {
                    return (int)Enum.Parse(typeof(T), item.ToString());
                }
            }
            return 0;
        }

    }
    public class Distance
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
}
