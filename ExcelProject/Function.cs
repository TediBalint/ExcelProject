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

namespace ExcelProject
{
    public class Function {
        public string Name { get; private set; }
        public string[] ParameterNames { get; private set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Description { get; private set; }
        private static readonly string secretCharacter = "🜲";
        private Function(string _name, string _params) {
            Name = _name;
            Parameters = new();
            InitializeParamNames();
            if (_params == secretCharacter) return;
            List<string> paramVals = new();
            bool readyToSplit = true;
            int lastIdx = 0;
            for (int i = 0; i < _params.Length;i++)
            {
                if (_params[i] == '(') {
                    readyToSplit = false;
                }
                else if (_params[i] == ';' && readyToSplit) {
                    paramVals.Add(_params.Substring(lastIdx, i - lastIdx));
                    lastIdx = i + 1;
                }
                else if(_params[i] == ')')
                {
                    readyToSplit = true;
                }
            }
            paramVals.Add(_params.Substring(lastIdx, _params.Length - lastIdx));
            if (ParameterNames[0].StartsWith("*")) { 
                int minus = ParameterNames.Length - 1;
                for (int i = 0; i < paramVals.Count - minus; i++) {
                    try
                    {
                        string evaluatedParam = evaluateParameter(paramVals[i]);
                        Parameters.Add($"{ParameterNames[0].Replace("*", "")}{i + 1}", evaluatedParam); // tartományt valahogy handle-elni
                    }
                    catch
                    {
                        throw new Exception("A paraméterek száma nem elegendő"); // compile miatt meg egy catch ág?
                    }
                }
                for (int i = 0; i < minus; i++) {
                    Parameters.Add(ParameterNames[i + 1], paramVals[i + paramVals.Count - minus]);
                    //itt is kéne hibát dobni
                }
            }
            else {
                if (paramVals.Count != ParameterNames.Length) {
                    throw new Exception("A paraméterek száma nem elegendő");
                }
                for (int i = 0; i < paramVals.Count; i++) {
                    //ide is try
                    string evaluatedParam = evaluateParameter(paramVals[i]);
                    Parameters.Add(ParameterNames[i], evaluatedParam);
                }
            }
        }
        private string evaluateParameter(string param)
        {
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
            return ( x, y );
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
                    for(int i = miny; i <= maxy; i++) {
                        for(int j = minx; j <= maxx; j++) {
                            sum += double.Parse(Statics.CellPropertiesModels[i][j].Text);
                            count++;
                        }
                    }
                    count--;
                    // hibas tartomany thorwoljon
                }
                else sum += double.Parse(param.Value);
                count++;
            }
            if (sumOnly) return sum;
            return sum / count;
        }
        private double SumIfOrAvgIf(bool sumOnly = true) {
            return 0;
        }
        private int Count() {
            return 0;
        }
        private int CountIf() {
            return 0;
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
                    for (int i = miny; i <= maxy; i++) {
                        for (int j = minx; j <= maxx; j++) {
                            nums.Add(double.Parse(Statics.CellPropertiesModels[i][j].Text));
                        }
                    }
                    // hibas tartomany thorwoljon
                }
                else nums.Add(double.Parse(param.Value));
            }
            if (max) return nums.Max();
            return nums.Min();
        }
        private int WhereIs() {
            return 0;
        }
        private string Index() {
            return "";
        }
        private string LeftOrRight(bool left = true) { // "
            if(left) {
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
                _params = _params.Remove(_params.Length - 1);
                return new Function(_name, _params);
            }
            catch {
                return new Function(arg, secretCharacter);
            }
        }
    }
}
