using System.Collections.Generic;

namespace pdx_ymltranslator.Util
{
    public class YML
    {
        private string lineeng;
        private string linechn;
        private string veng;
        private string vchn;
        private string variablename;
        private string variablenamewithoutnum;
        private string oldeng;
        private int lineindex = 0;

        public YML()
        {
            lineeng = string.Empty;
            linechn = string.Empty;
            veng = string.Empty;
            vchn = string.Empty;
            variablename = string.Empty;
            variablenamewithoutnum = string.Empty;
            oldeng = string.Empty;
        }

        public YML(string LineTo) : this()
        {
            LineCHN = LineTo;
            LineENG = string.Empty;
            variablename = YMLTools.RegexGetName(LineTo);
            variablenamewithoutnum = YMLTools.RegexGetNameOnly(variablename);
            vchn = YMLTools.RegexGetValue(LineTo);
            if (HasError()) { FixError(); }
            oldeng = string.Empty;
        }

        public YML(string LineFrom, Dictionary<string, string> DictForTo) : this()
        {
            LineENG = LineFrom;
            variablename = YMLTools.RegexGetName(LineFrom);
            variablenamewithoutnum = YMLTools.RegexGetNameOnly(variablename);
            veng = YMLTools.RegexGetValue(LineFrom);

            if (string.IsNullOrEmpty(VariableNameWithoutNum) == false
                && DictForTo.TryGetValue(VariableNameWithoutNum, out string outvalue)
                && string.IsNullOrEmpty(outvalue) == false)
            {
                LineCHN = outvalue;
            }
            else
            {
                LineCHN = LineFrom;
            }

            if (HasError()) { FixError(); }
            oldeng = string.Empty;
        }

        public void LoadWithOldDict(Dictionary<string, string> OldDict)
        {
            if (string.IsNullOrEmpty(VariableNameWithoutNum) == false
                && OldDict.TryGetValue(VariableNameWithoutNum, out string outvalue)
                && string.IsNullOrEmpty(outvalue) == false)
            {
                oldeng = YMLTools.RegexGetValue(outvalue);
            }
            else
            {
                oldeng = string.Empty;
            }
        }

        public void LineIndexInitialize(int inputLineIndex)
        {
            lineindex = inputLineIndex;
        }

        public string VariableNameWithoutNum
        {
            get { return variablenamewithoutnum; }
        }

        public string VENG
        {
            get { if (IsComment()) { return lineeng; } else { return veng; } }
        }

        public string VCHN
        {
            get { return vchn; }
        }
        public string OldENG
        {
            get { return oldeng; }
        }

        public string TranslatedLine
        {
            get
            {
                if (IsLineWithComment())
                {
                    return LineCHN.Replace(TestComment(), variablename + "\"" + vchn + "\"");
                }

                if (variablename != "")
                {
                    return variablename + "\"" + vchn + "\"";
                }

                else
                {
                    return linechn;
                }
            }
        }

        private string LineENG
        {
            get { return lineeng; }
            set
            {
                veng = YMLTools.RegexGetValue(value);
                variablename = YMLTools.RegexGetName(value);
                lineeng = value;
            }
        }

        private string LineCHN
        {
            get { return linechn; }
            set
            {
                vchn = YMLTools.RegexGetValue(value);
                variablename = YMLTools.RegexGetName(value);
                linechn = value;
            }
        }

        private string VariableName
        {
            get { return variablename; }
            set
            {
                variablenamewithoutnum = YMLTools.RegexGetNameOnly(value);
                variablename = value;
            }
        }

        public void ApplyLine(string ApplyText)
        {
            if (IsComment())
            {
                linechn = ApplyText;
            }
            else
            {
                vchn = ApplyText;
            }
            vchn = ApplyText;
        }

        private string HasErrorText
        {
            get
            {
                if (HasError()) { return "Error"; }
                else { return ""; }
            }
        }

        public void FixError()
        {
            LineCHN = VariableName + "\"" + LineCHN.Replace(VariableName, "") + "\"";
        }

        private bool IsComment()
        {
            return YMLTools.RemoveSpace(LineCHN).Length > 0 && YMLTools.RemoveSpace(LineCHN)[0] == '#';
        }

        private bool IsSpaceLine()
        {
            return string.IsNullOrEmpty(LineCHN.Trim());
        }

        public bool IsEditable()
        {
            return (IsSpaceLine() || YMLTools.RemoveSpace(LineCHN) == "l_english:" || IsComment()) == false;
        }

        private string TestComment()
        {
            return YMLTools.RegexGetName(LineCHN) + "\"" + YMLTools.RegexGetValue(LineCHN) + "\"";
        }

        private bool IsLineWithComment()
        {
            return LineCHN.Replace(TestComment(), "").Replace(" ", "") != "";
        }

        private bool HasError()
        {
            return variablename != "" && vchn == "" && LineCHN != LineENG;
        }

        public bool SameInToAndFrom()
        {
            return IsEditable() && veng == vchn && veng != "";
        }

        public bool IsAllQoute()
        {
            return IsAllQoute(veng);
        }

        public bool IsAllQoute(string text)
        {
            if (text != "")
            {
                text = YMLTools.RemoveSpace(text);
                if (text.Length > 0)
                {
                    char fst = text[0];
                    char end = text[text.Length - 1];
                    if (fst == '$' && end == '$') { return true; }
                    if (fst == '[' && end == ']') { return true; }
                }
            }
            return false;
        }

        public bool OldNewisDifferent()
        {
            return YMLTools.RemoveSpace(oldeng) != YMLTools.RemoveSpace(veng);
        }
    }
}
