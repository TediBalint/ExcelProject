using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace ExcelProject
{
    public class Function {
        public string Name { get; private set; }
        public string[] ParameterNames { get; private set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Description { get; private set; }
        private static readonly string secretCharacter = "🜲";
        public string raw { get; set; }
        private Function(string _name, string _params) {
            Name = _name;
            Parameters = new();
            InitializeParamNames();
            if (_params == secretCharacter) return;
            List<string> paramVals = new();
            bool readyToSplit = true;
            int lastIdx = 0;
            for (int i = 0; i < _params.Length; i++) {
                if (_params[i] == '(') {
                    readyToSplit = false;
                }
                else if (_params[i] == ';' && readyToSplit) {
                    paramVals.Add(_params.Substring(lastIdx, i - lastIdx));
                    lastIdx = i + 1;
                }
                else if (_params[i] == ')') {
                    readyToSplit = true;
                }
            }
            paramVals.Add(_params.Substring(lastIdx, _params.Length - lastIdx));
            if (ParameterNames[0].StartsWith("*")) {
                int minus = ParameterNames.Length - 1;
                for (int i = 0; i < paramVals.Count - minus; i++) {
                    try {
                        string evaluatedParam = evaluateParameter(paramVals[i]);
                        Parameters.Add($"{ParameterNames[0].Replace("*", "")}{i + 1}", evaluatedParam);
                    }
                    catch {
                        throw new Exception("A paraméterek száma nem elegendő");
                    }
                }
                for (int i = 0; i < minus; i++) {
                    Parameters.Add(ParameterNames[i + 1], paramVals[i + paramVals.Count - minus]);
                }
            }
            else {
                if (paramVals.Count != ParameterNames.Length) {
                    throw new Exception("A paraméterek száma nem elegendő");
                }
                for (int i = 0; i < paramVals.Count; i++) {
                    string evaluatedParam = evaluateParameter(paramVals[i]);
                    Parameters.Add(ParameterNames[i], evaluatedParam);
                }
            }
        }
        public static string evaluateParameter(string param) {
            Function f = Compile(param);
            string evaluatedParam;
            try { evaluatedParam = Evaluate(param).ToString(); }
            catch { evaluatedParam = f.Invoke(); }
            return evaluatedParam;
        }
        public static Function?[] getAllFunctions() {
            string jsonstring = File.ReadAllText("paramNames.json");
            List<ParamSwitcher>? allFns = JsonSerializer.Deserialize<List<ParamSwitcher>>(jsonstring);
            return allFns.Select(n => Compile($"={n.Name}({secretCharacter})")).ToArray();
        }
        private void InitializeParamNames() {
            string jsonstring = File.ReadAllText("paramNames.json");
            List<ParamSwitcher>? allFns = JsonSerializer.Deserialize<List<ParamSwitcher>>(jsonstring);
            ParamSwitcher? fn = allFns.Where(x => x.Name == Name).FirstOrDefault();
            if (fn != null) {
                ParameterNames = fn.Values;
                Description = fn.Description;
            }
            else ParameterNames = [];
        }
        private static int TranslateLettersToIdx(string colName) {
            int power = 0;
            double res = 0;
            foreach (char c in colName.Reverse()) {
                res += (c + 1 - 'A') * Math.Pow(26, power);
                power++;
            }
            return (int)res;
        }
        private static string RemoveFirstChar(string str) {
            return string.Join(string.Empty, str.ToList().Skip(1));
        }
        public string Invoke() {
            return Name switch {
                "SZUM" => SumOrAvg().ToString(),
                "ÁTLAG" => SumOrAvg(false).ToString(),
                "SZUMHA" => SumIfOrAvgIf().ToString(),
                "ÁTLAGHA" => SumIfOrAvgIf(false).ToString(),
                "DARAB" => Count().ToString(),
                "DARABHA" => CountIf().ToString(),
                "MAX" => Extreme().ToString(),
                "MIN" => Extreme(false).ToString(),
                "INDEX" => Index(),
                "HOL.VAN" => WhereIs().ToString(),
                "BAL" => LeftOrRight(),
                "JOBB" => LeftOrRight(false),
                _ => Name[0] == '=' ? Evaluate(RemoveFirstChar(Name)).ToString() : Name
            };
        }
        private (int, int) getCoordsFromText(string text) {
            int x = TranslateLettersToIdx(new Regex(@"^[A-Z]+").Match(text).ToString());
            int y = int.Parse(new Regex(@"\d+").Match(text).ToString());
            return (x, y);
        }
        private bool FitsCriteria(string inp, string crit){
            string critValueAsStr = new Regex(@"(?!>|<|!|=).*$").Match(crit).ToString();
            try {
                double n;
                double critValue = 0;
                if (Statics.CellCoordRegex.Match(critValueAsStr).Success) {
                    (int x, int y) = getCoordsFromText(critValueAsStr);
                    critValue = double.Parse(Statics.CellPropertiesModels[y][x].Text);
                }
                else critValue = double.Parse(critValueAsStr);
                n = double.Parse(inp);
                if (crit[0] == '>') {
                    if (crit[1] == '=') return n >= critValue;
                    else return n > critValue;
                }
                if (crit[0] == '<') {
                    if (crit[1] == '>') return n != critValue;
                    if (crit[1] == '=') return n <= critValue;
                    else return n < critValue;
                }
                if (crit[0] == '!') return n != critValue;
                else return n == critValue;
            }
            catch {
                if (Statics.CellCoordRegex.Match(critValueAsStr).Success) {
                    (int x, int y) = getCoordsFromText(critValueAsStr);
                    critValueAsStr = Statics.CellPropertiesModels[y][x].Text;
                }
                if ((crit[0] == '<' && crit[1] != '>') || (crit[0] == '!' && crit[1] != '=')) throw new Exception("#Hibás kritérium");
                if ((crit[0] == '<' && crit[1] == '>') || (crit[0] == '!' && crit[1] == '=')) return inp != critValueAsStr;
                return inp == critValueAsStr; 
            }
        }
        private double SumOrAvg(bool sumOnly = true) {
            double sum = 0;
            int count = 0;
            foreach (var param in Parameters) {
                if (Statics.CellCoordRegex.Match(param.Value).Success) {
                    (int x, int y) = getCoordsFromText(param.Value);
                    sum += double.Parse(Statics.CellPropertiesModels[y][x].Text);
                }
                else if (Statics.CellTerritoryRegex.Match(param.Value).Success) {
                    string[] startAndEnd = param.Value.Split(":");
                    (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                    (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                    if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                    for (int i = miny; i <= maxy; i++) {
                        for (int j = minx; j <= maxx; j++) {
                            sum += double.Parse(Statics.CellPropertiesModels[i][j].Text);
                            count++;
                        }
                    }
                    count--;
                }
                else sum += double.Parse(param.Value);
                count++;
            }
            if (sumOnly) return sum;
            return sum / count;
        }
        private double SumIfOrAvgIf(bool sumOnly = true) {
            double sum = 0;
            int count = 0;
            string sumTerritory;
            try { sumTerritory = Parameters["Összeg_Tartomány"]; }
            catch { sumTerritory = Parameters["Átlag_Tartomány"]; }
            string CriteriaNumber;
            if (Statics.CellTerritoryRegex.Match(sumTerritory).Success && Statics.CellTerritoryRegex.Match(Parameters["Tartomány"]).Success) {
                string[] territoryStartAndEnd = Parameters["Tartomány"].Split(":");
                (int terrx, int terry) = getCoordsFromText(territoryStartAndEnd[0]);
                (int terrendx, int terrendy) = getCoordsFromText(territoryStartAndEnd[1]);
                string[] startAndEnd = sumTerritory.Split(":");
                (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                if ((terrendx - terrx) * (terrendy - terry) != (maxx - minx) * (maxy - miny)) throw new Exception("#A tartományok számossága nem egyezik");

                for (int i = miny; i <= maxy; i++) {
                    for (int j = minx; j <= maxx; j++) {
                        CriteriaNumber = Statics.CellPropertiesModels[i + Math.Abs(terry - miny)][j + Math.Abs(terrx - minx)].Text;
                        if (FitsCriteria(CriteriaNumber, Parameters["Kritérium"])) {
                            sum += double.Parse(Statics.CellPropertiesModels[i][j].Text);
                            count++;
                        }
                    }
                }
            }
            else throw new Exception("#Hibás tartományhivatkozás");
            if (sumOnly) return sum;
            return sum / count;
        }
        private int Count() {
            int count = 0;
            if (Statics.CellTerritoryRegex.Match(Parameters["Tartomány"]).Success) {
                string[] startAndEnd = Parameters["Tartomány"].Split(":");
                (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                for (int i = miny; i <= maxy; i++) {
                    for (int j = minx; j <= maxx; j++) {
                        try {
                            _ = double.Parse(Statics.CellPropertiesModels[i][j].Text);
                            count++;
                        }
                        catch { }
                    }
                }
            }
            else throw new Exception("Hibás tartományhivatkozás");
            return count;
        }
        private int CountIf() {
            int count = 0;
            if (Statics.CellTerritoryRegex.Match(Parameters["Tartomány"]).Success) {
                string[] startAndEnd = Parameters["Tartomány"].Split(":");
                (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                for (int i = miny; i <= maxy; i++) {
                    for (int j = minx; j <= maxx; j++) {
                        if (FitsCriteria(Statics.CellPropertiesModels[i][j].Text, Parameters["Kritérium"])) count++;
                    }
                }
            }
            else throw new Exception("#Hibás tartományhivatkozás");
            return count;
        }
        private double Extreme(bool max = true) {
            List<double> nums = new();
            foreach (var param in Parameters) {
                if (Statics.CellCoordRegex.Match(param.Value).Success) {
                    (int x, int y) = getCoordsFromText(param.Value);
                    nums.Add(double.Parse(Statics.CellPropertiesModels[y][x].Text));
                }
                else if (Statics.CellTerritoryRegex.Match(param.Value).Success) {
                    string[] startAndEnd = param.Value.Split(":");
                    (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                    (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                    if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                    for (int i = miny; i <= maxy; i++) {
                        for (int j = minx; j <= maxx; j++) {
                            nums.Add(double.Parse(Statics.CellPropertiesModels[i][j].Text));
                        }
                    }
                }
                else nums.Add(double.Parse(param.Value));
            }
            if (max) return nums.Max();
            return nums.Min();
        }
        private int WhereIs() {
            if (Statics.CellTerritoryRegex.Match(Parameters["Tartomány"]).Success) {
                string[] startAndEnd = Parameters["Tartomány"].Split(":");
                (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                bool isColumn = maxx - minx == 0;
                bool isRow = maxy - miny == 0;
                if (!isColumn && !isRow) throw new Exception("#Tartomány csak egy sor vagy egy oszlop lehet");
                for (int i = miny; i <= maxy; i++) {
                    for (int j = minx; j <= maxx; j++) {
                        if (Statics.CellPropertiesModels[i][j].Text == Parameters["Érték"]) { 
                            if (isColumn) return i + 1 - miny;
                            else return j + 1 - minx;
                        }
                    }
                }
                throw new Exception("#A keresett érték nem található");
            }
            else throw new Exception("#Hibás tartományhivatkozás");
        }
        private string Index() {
            if (Statics.CellTerritoryRegex.Match(Parameters["Tartomány"]).Success) {
                string[] startAndEnd = Parameters["Tartomány"].Split(":");
                (int minx, int miny) = getCoordsFromText(startAndEnd[0]);
                (int maxx, int maxy) = getCoordsFromText(startAndEnd[1]);
                if ((maxy - miny) < 0 || (maxx - minx) < 0) throw new Exception("#Hibás tartomány");
                bool isColumn = maxx - minx == 0;
                bool isRow = maxy - miny == 0;
                if (!isColumn && !isRow) throw new Exception("#Tartomány csak egy sor vagy egy oszlop lehet");
                if (isRow) return Statics.CellPropertiesModels[miny][minx + int.Parse(Parameters["Index"]) - 1].Text;
                return Statics.CellPropertiesModels[miny + int.Parse(Parameters["Index"]) - 1][minx].Text;
            }
            else throw new Exception("#Hibás tartományhivatkozás");
        }
        private string LeftOrRight(bool left = true) { 
            if (Parameters["Szöveg"][0] == '\"' && Parameters["Szöveg"][^1] == '\"') {
                Parameters["Szöveg"] = RemoveFirstChar(Parameters["Szöveg"]);
                Parameters["Szöveg"].Remove(Parameters["Szöveg"].Length - 1);
            }
            if (left) {
                return string.Join(string.Empty, Parameters["Szöveg"].ToList().Take(int.Parse(Parameters["n"])));
            }
            return string.Join(string.Empty, Parameters["Szöveg"].ToList().Skip(Parameters["Szöveg"].Length - int.Parse(Parameters["n"])));
        }
        private static double Evaluate(string expression)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("expression", string.Empty.GetType(), expression);
            System.Data.DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
        public static Function Compile(string arg) {
            try {
                string[] pcs = arg.Split('('); 
                string _name;
                if (pcs[0][0] == '=') _name = RemoveFirstChar(pcs[0]);
                else _name = pcs[0];
                string _params = string.Join(string.Empty, string.Join('(', pcs.Skip(1)));
                if (_params[^1] == ')') _params = _params.Remove(_params.Length - 1);
                else throw new Exception("invalid parentheses"); // azer meg tesztelem
                Function f = new Function(_name, _params);
                f.raw = arg;
                return f;
            }
            catch {
                return new Function(arg, secretCharacter);
            }
        }
    }
}
