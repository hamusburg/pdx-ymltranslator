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
            lineeng = "";
            linechn = "";
            veng = "";
            vchn = "";
            variablename = "";
            variablenamewithoutnum = "";
            oldeng = "";
        }

        public YML(string LineTo) : this()
        {
            LineCHN = LineTo;
            LineENG = "";
            variablename = YMLTools.RegexGetName(LineTo);
            variablenamewithoutnum = YMLTools.RegexGetNameOnly(variablename);
            vchn = YMLTools.RegexGetValue(LineTo);
            if (HasError()) { FixError(); }
            oldeng = "";
        }

        public YML(string LineFrom, Dictionary<string, string> DictForTo) : this()
        {
            LineENG = LineFrom;
            variablename = YMLTools.RegexGetName(LineFrom);
            variablenamewithoutnum = YMLTools.RegexGetNameOnly(variablename);
            veng = YMLTools.RegexGetValue(LineFrom);

            if (VariableNameWithoutNum != "" && DictForTo.TryGetValue(VariableNameWithoutNum, out string outvalue) && outvalue != "")
            {
                LineCHN = outvalue;
                //vchn = YMLTools.RegexGetValue(outvalue);
            }
            else
            {
                LineCHN = LineFrom;
                //vchn = YMLTools.RegexGetValue(LineFrom);
            }

            if (HasError()) { FixError(); }
            oldeng = "";
        }

        public void LoadWithOldDict(Dictionary<string, string> OldDict)
        {
            if (VariableNameWithoutNum != "" && OldDict.TryGetValue(VariableNameWithoutNum, out string outvalue) && outvalue != "")
            {
                oldeng = YMLTools.RegexGetValue(outvalue);
            }
            else { oldeng = ""; }
        }

        public void LineIndexInitialize(int inputLineIndex)
        {
            lineindex = inputLineIndex;
        }

        public string VariableNameWithoutNum
        {
            get
            {
                return variablenamewithoutnum;
            }
        }

        public string VENG
        {
            get
            {
                if (IsComment()) { return lineeng; }
                return veng;
            }
        }

        public string VCHN
        {
            get
            {
                return vchn;
            }
        }
        public string OldENG
        {
            get { return oldeng; }
        }

        public string TranslatedLine
        {
            get
            {
                if (IsLineWithComment()) { return LineCHN.Replace(TestComment(), variablename + "\"" + vchn + "\""); }
                if (variablename != "") { return variablename + "\"" + vchn + "\""; }
                else { return linechn; }
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
            if (IsComment()) { linechn = ApplyText; }
            else { vchn = ApplyText; }
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
            if (YMLTools.RemoveSpace(LineCHN).Length > 0)
            {
                if (YMLTools.RemoveSpace(LineCHN).Substring(0, 1) == "#") { return true; }
            }
            return false;
        }

        private bool IsSpaceLine()
        {
            if (LineCHN.Replace(" ", "") == "") { return true; }
            return false;
        }

        public bool IsEditable()
        {
            if (IsSpaceLine())
            {
                return false;
            }

            if (YMLTools.RemoveSpace(LineCHN) == "l_english:")
            {
                return false;
            }

            if (IsComment())
            {
                return false;
            }
            return true;
        }

        private string TestComment()
        {
            return YMLTools.RegexGetName(LineCHN) + "\"" + YMLTools.RegexGetValue(LineCHN) + "\"";
        }

        private bool IsLineWithComment()
        {
            if (LineCHN.Replace(TestComment(), "").Replace(" ", "") != "")
            {
                return true;
            }
            return false;
        }

        private bool HasError()
        {
            if (variablename != "" && vchn == "" && LineCHN != LineENG)
            { return true; }
            return false;
        }

        public bool SameInToAndFrom()
        {
            if (IsEditable() && veng == vchn && veng != "")
            {
                return true;
            }
            return false;
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
                    string first = text.Substring(0, 1);
                    string end = text.Substring(text.Length - 1);
                    if (first == "$" && end == "$") { return true; }
                    if (first == "[" && end == "]") { return true; }
                }

                //if (first == "§" && text.Substring(text.Length - 2) == "§!")
                //{
                //    if (IsAllQoute(YMLTools.RegexRemoveColorSign(text))) { return true; }
                //}
            }
            return false;
        }
        public bool OldNewisDifferent()
        {
            if (YMLTools.RemoveSpace(oldeng) == YMLTools.RemoveSpace(veng)) { return false; }
            return true;
        }
    }
}
