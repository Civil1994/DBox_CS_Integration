using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace DBox_CS.Core.AppClass
{
    public class TranslationHelperV2
	{
		public static string TranslateFromEnglishToArabic(string EnglishText, string JsonPath)
		{
			string result = "";
			string value = Path.Combine(Directory.GetCurrentDirectory(), JsonPath);
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", value);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			TranslationClient translationClient = TranslationClient.Create();
			string text = "ar";
			TranslationResult translationResult = translationClient.TranslateText(EnglishText, "ar", "en");
			if (translationResult != null)
			{
				result = translationResult.TranslatedText;
			}
			return result;
		}

		public static string TranslateFromTo(string textToTranslate, string sourceLang, string targetLang, string JsonPath)
		{
			string result = "";
			string value = Path.Combine(Directory.GetCurrentDirectory(), JsonPath);
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", value);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			TranslationClient translationClient = TranslationClient.Create();
			TranslationResult translationResult = translationClient.TranslateText(textToTranslate, targetLang, sourceLang);
			if (translationResult != null)
			{
				result = translationResult.TranslatedText;
			}
			return result;
		}
	}
}
