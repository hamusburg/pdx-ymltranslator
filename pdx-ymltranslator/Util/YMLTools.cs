using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

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
            RegText = RegText.Replace(" ", string.Empty);
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
            return Regex.IsMatch(input, RegexStringWordBoundry(WordToMatch), RegexOptions.IgnoreCase);
        }

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
                case "Youdao":
                    StrOpeninBrowser.Append("http://m.youdao.com/dict?le=eng&q=");
                    break;
                default:
                    StrOpeninBrowser.Append("http://fanyi.baidu.com/?#en/zh/");
                    break;
            } 
            StrOpeninBrowser.Append(TextToTranslate);
            System.Diagnostics.Process.Start(StrOpeninBrowser.ToString());
        }

        public static string RemoveReturnMark(string input)
        {
            StringBuilder RemoveReturnText = new StringBuilder();
            RemoveReturnText.Append(input);
            RemoveReturnText.Replace("\r", string.Empty);
            RemoveReturnText.Replace("\n", string.Empty);
            return RemoveReturnText.ToString();
        }

        public static string RemoveSpace(string input)
        {
            return input.Replace(" ", string.Empty);
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

        public static List<YML> ReadFile(string fileName)
        {
            string EngPath = StaticVars.DIREN + fileName;
            string ChnPath = StaticVars.DIRCNen + fileName;

            if (File.Exists(ChnPath) == false)
            {
                using (StreamWriter sw = File.CreateText(ChnPath))
                {
                    sw.WriteLine("l_english:");
                }
            }

            List<string> listEng = new List<string>(File.ReadAllLines(EngPath));
            if (EngPath.Contains("simp_chinese.yml")) { ChnPath = ChnPath.Replace("simp_chinese.yml", "english.yml"); }
            List<string> listChn = new List<string>(File.ReadAllLines(ChnPath));

            return BuildYMLList(listEng, listChn);
        }
    }
}
