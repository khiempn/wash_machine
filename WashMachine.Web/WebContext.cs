using Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WashMachine.Web
{
    public class WebContext
    {
        public static string Version { get { return DateTime.Now.ToString("dd") + ".19"; } }

        public static string DefaultLanguage => "en-US";
        public static string LanguageEn => "en-US";
        public static string LanguageVi => "vi-VN";
        public static string ImageInitial => "/Images/pixel.png";

        public static string GetLink(string defaultLink, string viLink = null)
        {
            var backslash = @"/";
            if (string.IsNullOrEmpty(defaultLink)) backslash = string.Empty;
            if (viLink == null) viLink = defaultLink;
            if (Language == DefaultLanguage) return defaultLink;
            return Language2 + backslash + viLink;
        }

        public static string Language { get { return Thread.CurrentThread.CurrentCulture.Name; } }
        public static string Language2 { get { return Language.ToLower().Substring(0, 2); } }

        public static string GetValue(bool? isCorrect, string correctValue, string incorrectValue = "")
        {
            if (isCorrect == true) return correctValue;
            return incorrectValue;
        }

        public static string GetValue(string valueEn, string valueVi)
        {
            if (Language == LanguageVi) return valueVi;
            return valueEn;
        }

        public static float GetFloat(object value)
        {
            if (value == null) return 0;
            try
            {
                return Convert.ToSingle(value);
            }
            catch
            {
                return 0;
            }
        }

        public static string FormatPrice(decimal? value)
        {
            if (!value.HasValue) return "";
            return value.Value.ToString("n2");
        }

        public static string FormatTime(DateTime? dateTime)
        {
            if (dateTime == null) return string.Empty;
            return dateTime.Value.ToString("dd/MM/yyy HH:mm:ss");
        }
        public static string FormatDate(DateTime? dateTime)
        {
            if (dateTime == null) return string.Empty;
            return dateTime.Value.ToString("dd/MM/yyyy");
        }
        public static string GetFileType(string path)
        {
            path = ("." + path).ToLower();
            var index = path.LastIndexOf(".") + 1;
            var extension = path.Substring(index);
            var type = "UNKNOW";
            switch (extension)
            {
                case "doc":
                case "docx":
                    type = "WORD";
                    break;
                case "pdf":
                    type = "PDF";
                    break;
                case "gif":
                case "jpg":
                case "png":
                    type = "IMAGE";
                    break;
                case "txt":
                    type = "TXT";
                    break;
            }
            return type;
        }

        public static string MergeText(string str1, string str2, string connect = " - ")
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) return str1 + str2;
            return string.Format("{0}{1}{2}", str1, connect, str2);
        }

        public static string AddNewLine(string str1)
        {
            var trimString = "<br />";
            if (string.IsNullOrEmpty(str1)) return string.Empty; ;
            var result = str1.Replace("NACE", trimString + "NACE");
            result = result.Replace("NAICS", trimString + "NAICS");

            if (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }
            return result;
        }

        public static string TakeWords(string input, int length = 150)
        {
            if (input == null) input = string.Empty;
            var text = TextUtilities.RemoveHtmlTags(input);
            var array = text.Split(' ');
            if (array.Length <= 150) return text;
            return string.Join(" ", array.Take(length).ToArray()) + "...";
        }


        public static int GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return int.Parse(value);
        }

        public static string FormatPriceName(string name, DateTime? from, DateTime? to)
        {
            var text = name;
            if (from != null)
            {
                text += " - from " + from.Value.ToString("dd/MM/yyyy");
            }

            if (to != null)
            {
                text += " - to " + to.Value.ToString("dd/MM/yyyy");
            }
            return text;
        }

        public static decimal GetOctopusValue(int value)
        {
            decimal result = value;
            result = result / 10;
            return result;
        }
    }
}
