using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DBox_CS.Core.AppClass
{
    public static class TranslationHelper
    {
        private static readonly string[] EmptyStringArray = new string[0];

        // GOOGLE TRANSLATE V3

        public static string TranslateTextV3(string text, string source, string target, string location = "global")
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Ensure TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var projectId = GetProjectId();
            var accessToken = GetAccessToken();

            var url = string.Format(
                "https://translation.googleapis.com/v3/projects/{0}/locations/{1}:translateText",
                projectId, location);

            var payloadObj = new
            {
                contents = new[] { text },
                sourceLanguageCode = string.IsNullOrWhiteSpace(source) ? null : source,
                targetLanguageCode = string.IsNullOrWhiteSpace(target) ? "ar" : target,
                mimeType = "text/plain"
            };

            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(payloadObj);

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            req.Headers["Authorization"] = "Bearer " + accessToken;
            req.Headers["x-goog-user-project"] = projectId;

            using (var sw = new StreamWriter(req.GetRequestStream()))
                sw.Write(payload);

            try
            {
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    var body = sr.ReadToEnd();
                    var json = JObject.Parse(body);

                    var translations = json["translations"];
                    if (translations != null && translations.HasValues)
                    {
                        var translated = translations[0]?["translatedText"]?.ToString();
                        return translated ?? string.Empty;
                    }
                    return string.Empty;
                }
            }
            catch (WebException ex)
            {
                try
                {
                    using (var er = new StreamReader(ex.Response.GetResponseStream()))
                        System.Diagnostics.Debug.WriteLine("[v3 translateText error] " + er.ReadToEnd());
                }
                catch { }
                return string.Empty;
            }
        }

        // GOOGLE ROMANIZATION (AR → EN)
        public static string[] RomanizeBatch(string[] arabicTokens, out string apiError, string location = "global")
        {
            apiError = null;
            if (arabicTokens == null || arabicTokens.Length == 0)
                return EmptyStringArray;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var projectId = GetProjectId();
            var accessToken = GetAccessToken();

            var url = string.Format(
                "https://translation.googleapis.com/v3/projects/{0}/locations/{1}:romanizeText",
                projectId, location);

            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                sourceLanguageCode = "ar",
                contents = arabicTokens
            });

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            req.Headers["Authorization"] = "Bearer " + accessToken;
            req.Headers["x-goog-user-project"] = projectId;

            using (var sw = new StreamWriter(req.GetRequestStream()))
                sw.Write(payload);

            try
            {
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    var body = sr.ReadToEnd();
                    dynamic json = JObject.Parse(body);

                    var list = new List<string>(arabicTokens.Length);
                    var arr = json["romanizations"];
                    if (arr != null && arr.Count == arabicTokens.Length)
                    {
                        for (int i = 0; i < arr.Count; i++)
                            list.Add(arr[i]?["romanizedText"]?.ToString() ?? string.Empty);
                    }
                    else
                    {
                        for (int i = 0; i < arabicTokens.Length; i++) list.Add(string.Empty);
                    }
                    return list.ToArray();
                }
            }
            catch (WebException ex)
            {
                try
                {
                    using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                        apiError = sr.ReadToEnd();
                }
                catch { }
                return Enumerable.Repeat(string.Empty, arabicTokens.Length).ToArray();
            }
        }

        // FALLBACK AR → EN (when API fails)
        public static string FallbackRomanizeToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return token;

            var map = new Dictionary<char, string>
            {
                { 'ا', "a" }, { 'أ', "a" }, { 'إ', "i" }, { 'آ', "a" },
                { 'ء', "'" }, { 'ؤ', "u" }, { 'ئ', "i" }, { 'ب', "b" },
                { 'ت', "t" }, { 'ث', "th" }, { 'ج', "j" }, { 'ح', "h" },
                { 'خ', "kh" }, { 'د', "d" }, { 'ذ', "dh" }, { 'ر', "r" },
                { 'ز', "z" }, { 'س', "s" }, { 'ش', "sh" }, { 'ص', "s" },
                { 'ض', "d" }, { 'ط', "t" }, { 'ظ', "z" }, { 'ع', "a" },
                { 'غ', "gh" }, { 'ف', "f" }, { 'ق', "q" }, { 'ك', "k" },
                { 'ل', "l" }, { 'م', "m" }, { 'ن', "n" }, { 'ه', "h" },
                { 'و', "w" }, { 'ي', "y" }, { 'ة', "h" }, { 'ى', "a" },
                { 'ﻻ', "la" }
            };

            var sb = new StringBuilder(token.Length * 2);
            foreach (var ch in token)
            {
                string r;
                if (map.TryGetValue(ch, out r)) sb.Append(r);
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        // UTILITIES
        public static bool IsIdLike(string s)
        {
            return !string.IsNullOrEmpty(s) && s.Any(char.IsDigit) && !s.Any(char.IsWhiteSpace);
        }

        public static string NormalizeArabic(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            s = s
                .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
                .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9')
                .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
                .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9')
                .Replace("ـ", ""); // tatweel

            var sb = new StringBuilder(s.Length);
            foreach (var ch in s.Normalize(NormalizationForm.FormC))
            {
                int code = ch;
                bool isDiacritic = (code >= 0x064B && code <= 0x065F) || code == 0x0670 || (code >= 0x06D6 && code <= 0x06ED);
                bool isZeroWidth = code == 0x200C || code == 0x200D || code == 0x200E || code == 0x200F || code == 0x061C;
                if (!isDiacritic && !isZeroWidth) sb.Append(ch);
            }
            s = sb.ToString();

            return Regex.Replace(s, "\\s+", " ").Trim();
        }

        public static bool ContainsArabicLetter(string s)
        {
            foreach (var ch in s)
            {
                int code = ch;
                if ((code >= 0x0600 && code <= 0x06FF) || // Arabic
                    (code >= 0x0750 && code <= 0x077F) || // Arabic Supplement
                    (code >= 0x08A0 && code <= 0x08FF) || // Arabic Extended-A
                    (code >= 0xFB50 && code <= 0xFDFF) || // Presentation Forms-A
                    (code >= 0xFE70 && code <= 0xFEFF))   // Presentation Forms-B
                    return true;
            }
            return false;
        }

        public static List<string> SplitTokens(string s)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < s.Length)
            {
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                if (i >= s.Length) break;
                int start = i;
                while (i < s.Length && !char.IsWhiteSpace(s[i])) i++;
                tokens.Add(s.Substring(start, i - start));
            }
            return tokens;
        }

        // GOOGLE SERVICE ACCOUNT HELPERS
        private static string GetServiceAccountJsonPath()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string addBin = Path.Combine(basePath);
            //string fileName = ConfigurationManager.AppSettings["GoogleServiceAccountJson"]
            //                  ?? "translation-421807-6113b244569e.json";
            string fileName = ConfigurationManager.AppSettings["GoogleSettings:ServiceAccountJson"]
                  ?? "translation-421807-6113b244569e.json";
            return Path.Combine(addBin, fileName);
        }

        private static string GetProjectId()
        {
            var jsonPath = GetServiceAccountJsonPath();
            var jo = JObject.Parse(File.ReadAllText(jsonPath));
            return (string)jo["project_id"];
        }

        private static string GetAccessToken()
        {
            var jsonPath = GetServiceAccountJsonPath();
            var scopes = new[] { "https://www.googleapis.com/auth/cloud-translation" };
            var cred = GoogleCredential.FromFile(jsonPath).CreateScoped(scopes);
            return cred.UnderlyingCredential.GetAccessTokenForRequestAsync().GetAwaiter().GetResult();
        }


    }
}
