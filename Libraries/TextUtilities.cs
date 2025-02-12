using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Libraries
{

    public class TextUtilities
    {
        public const string FormatDate = "dd/MM/yyyy";
        public const string FormatDateTime = "dd/MM/yyyy HH:mm:ss";

        public static string GetString(DateTime? date)
        {
            if (date == null) return string.Empty;
            return date.Value.ToString("dd/MM/yyyy");
        }
        public static DateTime? ConvertToDatetime(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            try
            {
                value = value.Trim().Replace("-", "/");
                string[] arr = value.Split('/');
                return new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]));
            }
            catch
            {
                return null;
            }

        }
        public static DateTime? GetDatetime(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            try
            {
                value = value.Trim().Replace("-", "/");
                string[] arr = value.Split('/');
                return new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]));
            }
            catch
            {
                return null;
            }

        }
        public static string ConvertToFullString(DateTime? dt)
        {
            if (dt == null)
                return string.Empty;
            else
            {
                string value = Convert.ToDateTime(dt.Value).ToString("dd/MM/yyyy HH:mm");
                return value.Replace("AM", "SA").Replace("PM", "CH");
            }
        }

        public static string ConvertToString(decimal? value)
        {
            if (value == null || value.Value <= 0)
                return string.Empty;

            string temp = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:c}", value);
            temp = temp.Replace(",00", string.Empty);
            temp = temp.Replace("₫", "");
            return temp;
        }

        public static string ConvertToString(DateTime? date)
        {
            if (date == null) return string.Empty;
            return date.Value.ToString(FormatDate);
        }

        public static string IntToString(int? value)
        {
            if (value == null || value.Value < 0)
                return string.Empty;

            string temp = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:c}", value);
            temp = temp.Replace(",00", string.Empty);
            temp = temp.Replace("₫", "");
            return temp;
        }

        public static string MergeStrings(string param1, string param2)
        {
            string value = string.Empty;

            if (!string.IsNullOrEmpty(param1) && !string.IsNullOrEmpty(param2))
                value = string.Format("{0}; {1}", param1, param2);
            else if (!string.IsNullOrEmpty(param1))
                value = string.Format("{0}", param1);
            else if (!string.IsNullOrEmpty(param2))
                value = string.Format("{0}", param2);

            return value;
        }

        public static string GenerateSeparator(int num)
        {
            if (num <= 1)
                return string.Empty;

            string value = string.Empty;
            for (int i = 1; i < num; i++)
            {
                value += "--";
            }

            return value;
        }

        public static string MakeSeparator(int num)
        {
            if (num <= 1)
                return string.Empty;

            string value = string.Empty;
            for (int i = 1; i < num; i++)
            {
                value += "--";
            }

            return value;
        }

        public static string GetBetweenStrings(string param1, string param2)
        {
            string value = string.Empty;

            if (!string.IsNullOrEmpty(param1))
                value = string.Format("{0}", param1);
            else if (!string.IsNullOrEmpty(param2))
                value = string.Format("{0}", param2);

            return value;
        }

        /// <summary>
        /// Remove all html tags exit on string value
        /// </summary>
        /// <param name="stringSummary"></param>
        /// <returns></returns>
        public static string RemoveHtml(string stringSummary)
        {
            string Summary = "";
            string[] arraySummary = stringSummary.Split(new Char[] { '<' });
            foreach (string s in arraySummary)
            {
                string htmlString = s.Substring(0, s.IndexOf(">") + CountChars(">"));
                if (string.IsNullOrEmpty(htmlString))
                {
                    Summary += " " + s.Trim();
                }
                else
                {
                    Summary += " " + s.Replace(htmlString, string.Empty).Trim();
                }
            }
            return Summary.Replace("\n", string.Empty);
        }

        /// <summary>
        /// Remove all html tag not allow on string value
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveHtmlTags(string html)
        {
            if (html == null || html == string.Empty)
                return string.Empty;

            html = Regex.Replace(html, "<title.*?</title>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<object.*?</object>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<style.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<w.*?</w.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            html = rx.Replace(html, string.Empty);

            return html;
        }

        /// <summary>
        /// Remove all script and html tag on string value
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveScriptHtmlTags(string html)
        {
            if (html == null || html == string.Empty)
                return string.Empty;
            html = Regex.Replace(html, "<title.*?</title>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<object.*?</object>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<script.*?</script>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<style.*?</style>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<w.*?</w.*?>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return html;
        }

        /// <summary>
        /// Remove all tag can be used to apply injection technology
        /// </summary>
        /// <param name="stringinput"></param>
        /// <returns></returns>
        public static string RemoveScriptTags(object stringinput)
        {
            string stringoutput;
            stringoutput = stringinput.ToString().Replace("'", "&#146;");
            stringoutput = stringinput.ToString().Replace("script", "&#115;cript");
            stringoutput = stringinput.ToString().Replace("SCRIPT", "&#083;CRIPT");
            stringoutput = stringinput.ToString().Replace("Script", "&#083;cript");
            stringoutput = stringinput.ToString().Replace("script", "&#083;cript");
            stringoutput = stringinput.ToString().Replace("object", "&#111;bject");
            stringoutput = stringinput.ToString().Replace("OBJECT", "&#079;BJECT");
            stringoutput = stringinput.ToString().Replace("Object", "&#079;bject");
            stringoutput = stringinput.ToString().Replace("object", "&#079;bject");
            stringoutput = stringinput.ToString().Replace("applet", "&#097;pplet");
            stringoutput = stringinput.ToString().Replace("APPLET", "&#065;PPLET");
            stringoutput = stringinput.ToString().Replace("Applet", "&#065;pplet");
            stringoutput = stringinput.ToString().Replace("applet", "&#065;pplet");
            stringoutput = stringinput.ToString().Replace("event", "&#101;vent");
            stringoutput = stringinput.ToString().Replace("EVENT", "&#069;VENT");
            stringoutput = stringinput.ToString().Replace("Event", "&#069;vent");
            stringoutput = stringinput.ToString().Replace("event", "&#069;vent");
            stringoutput = stringinput.ToString().Replace("document", "&#100;ocument");
            stringoutput = stringinput.ToString().Replace("DOCUMENT", "&#068;OCUMENT");
            stringoutput = stringinput.ToString().Replace("Document", "&#068;ocument");
            stringoutput = stringinput.ToString().Replace("document", "&#068;ocument");
            stringoutput = stringinput.ToString().Replace("cookie", "&#099;ookie");
            stringoutput = stringinput.ToString().Replace("COOKIE", "&#067;OOKIE");
            stringoutput = stringinput.ToString().Replace("Cookie", "&#067;ookie");
            stringoutput = stringinput.ToString().Replace("cookie", "&#067;ookie");
            stringoutput = stringinput.ToString().Replace("form", "&#102;orm");
            stringoutput = stringinput.ToString().Replace("FORM", "&#070;ORM");
            stringoutput = stringinput.ToString().Replace("Form", "&#070;orm");
            stringoutput = stringinput.ToString().Replace("form", "&#070;orm");
            stringoutput = stringinput.ToString().Replace("iframe", "i&#102;rame");
            stringoutput = stringinput.ToString().Replace("IFRAME", "I&#070;RAME");
            stringoutput = stringinput.ToString().Replace("Iframe", "I&#102;rame");
            stringoutput = stringinput.ToString().Replace("iframe", "i&#102;rame");
            stringoutput = stringinput.ToString().Replace("textarea", "&#116;extarea");
            stringoutput = stringinput.ToString().Replace("TEXTAREA", "&#84;EXTAREA");
            stringoutput = stringinput.ToString().Replace("Textarea", "&#84;extarea");
            stringoutput = stringinput.ToString().Replace("textarea", "&#84;extarea");
            stringoutput = stringinput.ToString().Replace("input", "&#073;nput");
            stringoutput = stringinput.ToString().Replace("Input", "&#073;nput");
            stringoutput = stringinput.ToString().Replace("INPUT", "&#073;nput");
            stringoutput = stringinput.ToString().Replace("input", "&#073;nput");
            stringoutput = stringinput.ToString().Replace("<", "&lt;");
            stringoutput = stringinput.ToString().Replace(">", "&gt;");
            stringoutput = stringinput.ToString().Replace("'", "''");
            stringoutput = stringinput.ToString().Replace("=", "&#061;");
            stringoutput = stringinput.ToString().Replace("select", "sel&#101;ct");
            stringoutput = stringinput.ToString().Replace("join", "jo&#105;n");
            stringoutput = stringinput.ToString().Replace("union", "un&#105;on");
            stringoutput = stringinput.ToString().Replace("where", "wh&#101;re");
            stringoutput = stringinput.ToString().Replace("insert", "ins&#101;rt");
            stringoutput = stringinput.ToString().Replace("delete", "del&#101;te");
            stringoutput = stringinput.ToString().Replace("update", "up&#100;ate");
            stringoutput = stringinput.ToString().Replace("like", "lik&#101;");
            stringoutput = stringinput.ToString().Replace("drop", "dro&#112;");
            stringoutput = stringinput.ToString().Replace("create", "cr&#101;ate");
            stringoutput = stringinput.ToString().Replace("modify", "mod&#105;fy");
            stringoutput = stringinput.ToString().Replace("rename", "ren&#097;me");
            stringoutput = stringinput.ToString().Replace("alter", "alt&#101;r");
            stringoutput = stringinput.ToString().Replace("cast", "ca&#115;t");

            return stringoutput;
        }

        /// <summary>
        /// Get number words of string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int CountChars(string value)
        {
            int result = 0;
            bool lastWasSpace = false;

            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (lastWasSpace == false)
                    {
                        result++;
                    }
                    lastWasSpace = true;
                }
                else
                {
                    result++;
                    lastWasSpace = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string Cleanup(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.IndexOf('(') > -1 && value.IndexOf(')') > -1)
            {
                value = value.Substring(value.IndexOf('(') + 1, value.IndexOf(')') - value.IndexOf('(') - 1);
                return value;
            }

            return string.Empty;
        }

        public static string Encryption(string pstrPwd, string saltKey = "")
        {
            string password = string.Concat(pstrPwd, saltKey);
            string strHex = string.Empty;
            // A hash function works on a byte array, so we will create two arrays, 
            // one for our resulting hash and one for the given text.
            byte[] HashValue, MessageBytes = UnicodeEncoding.Default.GetBytes(password);

            // Now we create an object that will hash our text:
            SHA512 sha512 = new SHA512Managed();

            // And finally we calculate the hash and convert it to a hexadecimal string. 
            // Which we can store in a database for example
            HashValue = sha512.ComputeHash(MessageBytes);
            foreach (byte b in HashValue)
                strHex += string.Format("{0:x2}", b);
            return strHex;
        }

        private static string _keyHash = "cQfTjWnZr4u7x!A%D*G-KaPdSgUkXp2s";
        public static string EncryptAes(string text)
        {
            try
            {
                // Convert String to Byte
                if (string.IsNullOrEmpty(text)) return string.Empty;
                byte[] MsgBytes = Encoding.UTF8.GetBytes(text);
                byte[] KeyBytes = Encoding.UTF8.GetBytes(_keyHash);

                // Hash the password with SHA256
                //Secure Hash Algorithm
                //Operation And, Xor, Rot,Add (mod 232),Or, Shr
                //block size 1024
                //Rounds 80
                //rotation operator , rotates point1 to point2 by theta1=> p2=rot(t1)p1
                //SHR shift to right
                KeyBytes = SHA256.Create().ComputeHash(KeyBytes);

                byte[] bytesEncrypted = AES_Encryption(MsgBytes, KeyBytes);

                string encryptionText = Convert.ToBase64String(bytesEncrypted);

                return encryptionText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string DecryptAes(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return string.Empty;

                // Convert String to Byte
                byte[] MsgBytes = Convert.FromBase64String(text);
                byte[] KeyBytes = Encoding.UTF8.GetBytes(_keyHash);
                KeyBytes = SHA256.Create().ComputeHash(KeyBytes);

                byte[] bytesDecrypted = AES_Decryption(MsgBytes, KeyBytes);

                string decryptionText = Encoding.UTF8.GetString(bytesDecrypted);

                return decryptionText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private static byte[] AES_Encryption(byte[] Msg, byte[] Key)
        {
            byte[] encryptedBytes = null;

            //salt is generated randomly as an additional number to hash password or message in order o dictionary attack
            //against pre computed rainbow table
            //dictionary attack is a systematic way to test all of possibilities words in dictionary wheather or not is true?
            //to find decryption key
            //rainbow table is precomputed key for cracking password
            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.  == 16 bits
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(Key, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(Msg, 0, Msg.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decryption(byte[] Msg, byte[] Key)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(Key, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(Msg, 0, Msg.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }


        public static string RandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return RandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string CreateSaltKey(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }
        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public static string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1")
        {
            if (String.IsNullOrEmpty(passwordFormat))
                passwordFormat = "SHA1";
            var saltAndPassword = String.Concat(password, saltkey);

            var algorithm = HashAlgorithm.Create(passwordFormat);
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name", "passwordFormat");

            var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        // START New
        //public static string ConvertToUnSign3(string s)
        //{
        //    if (s == null) s = string.Empty;
        //    Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        //    string temp = s.Normalize(NormalizationForm.FormD);
        //    string name = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        //    name = Regex.Replace(name, @"\s+", "");
        //    name = name.Replace(" ", "-");
        //    return name;
        //}
        public static string ConvertToSlug(string s)
        {
            s += string.Empty;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD).Trim();
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace(",", "").Replace(" ", "-").Replace("---", "-").ToLower();
        }
        public static string ConvertToUnSign3(string s)
        {
            s += string.Empty;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD).Trim();
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToLower();
        }

        public static string GetRandomString(int len)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[len];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(5);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static string GetReadonly(bool value)
        {
            if (value) return "readonly";
            return string.Empty;
        }
        public static string GetDisabled(bool value)
        {
            if (value) return "true";
            return string.Empty;
        }
        public static string GenerateAddress(string street, string district, string province, string country, string category, string districtCategory)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(street))
                result = street;
            if (!string.IsNullOrEmpty(district))
            {
                if (IsNumber(district.Trim()))
                {
                    if (!string.IsNullOrEmpty(districtCategory))
                        result = !string.IsNullOrEmpty(result)
                            ? string.Format("{0}, {1} {2}", result, districtCategory, district.Trim())
                            : string.Format("{0} {1}", districtCategory, district.Trim());
                    else
                        result = !string.IsNullOrEmpty(result)
                            ? string.Format("{0}, District {1}", result, district.Trim())
                            : string.Format("District {0}", district.Trim());
                }
                else
                {
                    if (!string.IsNullOrEmpty(districtCategory))
                        result = !string.IsNullOrEmpty(result) ? string.Format("{0}, {1} {2}", result, district.Trim(), districtCategory) : string.Format("{0} {1}", district.Trim(), districtCategory);
                    else
                        result = !string.IsNullOrEmpty(result) ? string.Format("{0}, {1} District", result, district.Trim()) : string.Format("{0} District", district.Trim());
                }
            }

            if (!string.IsNullOrEmpty(province))
            {
                result = !string.IsNullOrEmpty(result) ? string.Format("{0}, {1}", result, province) : province;
                if (!string.IsNullOrEmpty(category))
                    result = string.Format("{0} {1}", result, category);
            }

            if (!string.IsNullOrEmpty(country))
                result = !string.IsNullOrEmpty(result) ? string.Format("{0}, {1}", result, country) : country;

            return result;
        }
        public static bool IsNumber(string input)
        {
            string OnlyNumbers = @"^[0-9]+$";
            return Regex.IsMatch(input, OnlyNumbers);
        }
        public static int GetInt(string input)
        {
            input = (string.Empty + input).Trim();
            string onlyNumbers = @"^[0-9]+$";
            var isNumber = Regex.IsMatch(input, onlyNumbers);
            if (isNumber)
            {
                return int.Parse(input);
            }
            return 0;
        }

        public static List<string> GetListString(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj)) obj = "[]";
                var list = JsonConvert.DeserializeObject<List<string>>(obj);
                return list;
            }
            catch(Exception ex)
            {
                return new List<string>();
            }
        }

        public static T DeserializeObject<T>(string text)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(text);
                return result;
            }catch(Exception e)
            {
                return default(T);
            }
        }
        public static string SerializeObject<T>(T obj)
        {
            try
            {
                var result = JsonConvert.SerializeObject(obj);
                return result;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
