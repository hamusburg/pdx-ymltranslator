using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Microsoft.VisualBasic;
using pdx_ymltranslator.Model;

namespace pdx_ymltranslator.Util
{
    public static class YMLTools
    {
        public static string RegexGetWith(string RegText, string RegexRule)
        {
            Regex Reggetname = new Regex(RegexRule, RegexOptions.None);
            StringBuilder returnString = new StringBuilder();
            var matches = Reggetname.Matches(RegText);

            foreach (var item in matches)
            {
                returnString.Append(item.ToString());
            }
            return returnString.ToString();
        }

        public static string RegexGetName(string RegText)
        {
            return RegexGetWith(RegText, "(^.*?):.*?(?=\")");
        }

        public static string RegexGetValue(string RegText)
        {
            return RegexGetWith(RegText, "(?<=(\\s\")).+(?=\")");
        }

        public static string RegexGetNameOnly(string RegText)
        {
            RegText = RegText.Replace(" ", "");
            return RegexGetWith(RegText, "^.*(?=:)");
        }

        public static string RegexRemoveColorSign(string RegText)
        {
            return RegexGetWith(RegText, "(?<=(§.)).+(?=(§!))");
        }

        private static string RegexStringWordBoundry(string input)
        {
            return @"(\W|^)" + input + @"(\W|$)";
        }

        public static bool RegexContainsWord(string input, string WordToMatch)
        {
            if (Regex.IsMatch(input, RegexStringWordBoundry(WordToMatch), RegexOptions.IgnoreCase)) { return true; }
            return false;
        }
        // 用于截取

        public static void OpenWithBrowser(string TextToTranslate, string APIEngine)
        {
            StringBuilder StrOpeninBrowser = new StringBuilder();
            switch (APIEngine)
            {
                case "Google":
                    StrOpeninBrowser.Append("http://translate.google.com/?#auto/zh-CN/");
                    break;
                case "Baidu":
                    StrOpeninBrowser.Append("http://fanyi.baidu.com/?#en/zh/");
                    break;
                case "Help":
                    StrOpeninBrowser.Append("https://github.com/inkitter/pdx-ymltranslator");
                    break;
                default:
                    StrOpeninBrowser.Append("http://fanyi.baidu.com/?#en/zh/");
                    break;
            }
            StrOpeninBrowser.Append(TextToTranslate);
            System.Diagnostics.Process.Start(StrOpeninBrowser.ToString());
        }
        // 用于默认浏览器打开翻译网页

        public static string RemoveReturnMark(string input)
        {
            StringBuilder RemoveReturnText = new StringBuilder();
            RemoveReturnText.Append(input);
            RemoveReturnText.Replace("\r", "");
            RemoveReturnText.Replace("\n", "");
            return RemoveReturnText.ToString();
        }
        // 用于移除换行符。

        public static string RemoveSpace(string input)
        {
            return input.Replace(" ", "");
        }

        public static string ReplaceWithUserDict(string input, Dictionary<string, string> dict)
        {
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                Regex rgx = new Regex(RegexStringWordBoundry(kvp.Key), RegexOptions.IgnoreCase);
                input = rgx.Replace(input, " " + kvp.Key + "<" + kvp.Value + "> ");
            }
            return input;
        }

        public static List<YML> BuildYMLList(List<string> listEng, List<string> listChn)
        {
            Dictionary<string, string> dictChn = BuildDictionary(listChn);

            List<YML> returnYMLList = new List<YML>();
            foreach (string lineEng in listEng)
            {
                returnYMLList.Add(new YML(lineEng, dictChn));
            }
            foreach (YML lineYML in returnYMLList)
            {
                if (dictChn.ContainsKey(lineYML.VariableNameWithoutNum))
                {
                    dictChn.Remove(lineYML.VariableNameWithoutNum);
                }
            }
            List<string> dictLeft = new List<string>(dictChn.Values);
            foreach (string lineLeft in dictLeft)
            {
                returnYMLList.Add(new YML(lineLeft));
            }
            return returnYMLList;
        }

        public static Dictionary<string, string> BuildDictionary(List<string> list)
        {
            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (string line in list)
            {
                string vn = RegexGetNameOnly(RegexGetName(line));
                if (!returnDict.ContainsKey(vn))
                {
                    returnDict.Add(vn, line);
                }
            }
            return returnDict;
        }

        public static string ToSimplifiedChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.SimplifiedChinese, 0);
        }

        public static string ToTraditionalChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.TraditionalChinese, 0);
        }

        public static FileExistInfo IsFileExistInfo(string filename)
        {
            FileExistInfo finfo = new FileExistInfo() { FileName = filename, IsExist = false };

            if (File.Exists(finfo.FileName))
            {
                finfo.IsExist = true;
                return finfo;
            }

            finfo.FileName = finfo.FileName.Replace("english.yml", "l_simp_chinese.yml");
            if (File.Exists(finfo.FileName))
            {
                finfo.IsExist = true;
                return finfo;
            }

            finfo.FileName = finfo.FileName.Replace("l_simp_chinese.yml", "l_english.yml");
            if (File.Exists(finfo.FileName))
            {
                finfo.IsExist = true;
                return finfo;
            }
            finfo.FileName = filename;
            return finfo;
        }

        public static List<YML> ReadFile(string filename)
        {
            string EngPath = StaticVars.DIREN + filename;
            string ChnPath = StaticVars.DIRCNen + filename;

            FileExistInfo finfo = IsFileExistInfo(ChnPath);
            if (!finfo.IsExist)
            {
                FileStream fs = File.Create(ChnPath);
                Byte[] info = new UTF8Encoding(true).GetBytes("l_english:");
                fs.Write(info, 0, info.Length);
                fs.Close();
            }
            // 检测chn文件夹内文件是否存在，不存在则建立。

            List<string> listEng = new List<string>(File.ReadAllLines(EngPath));
            if (EngPath.Contains("simp_chinese.yml")) { ChnPath = ChnPath.Replace("simp_chinese.yml", "english.yml"); }
            List<string> listChn = new List<string>(File.ReadAllLines(ChnPath));

            return BuildYMLList(listEng, listChn);
        }
    }
}
