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
            string[] paramVals = _params.Split(';');
            if (ParameterNames[0].StartsWith("*")) { 
                int minus = ParameterNames.Length - 1;
                for (int i = 0; i < paramVals.Length - minus; i++) {
                    try {
                        Parameters.Add($"{ParameterNames[i].Replace("*", "")}{i + 1}", paramVals[i]); // tartományt valahogy handle-elni
                    }
                    catch {
                        throw new Exception("A paraméterek száma nem elegendő");
                    }
                }
                for (int i = 0; i < minus; i++) {
                    Parameters.Add(ParameterNames[i + 1], paramVals[i + paramVals.Length - minus]);
                    //itt is kéne hibát dobni
                }
            }
            else {
                if (paramVals.Length != ParameterNames.Length) {
                    throw new Exception("A paraméterek száma nem elegendő");
                }
                for (int i = 0; i < paramVals.Length; i++) {
                    Parameters.Add(ParameterNames[i], paramVals[i]);
                }
            }
        }
        public static Function?[] getAllFunctions() {
            string jsonstring = File.ReadAllText("paramNames.json");
            List<ParamSwitcher>? allFns = JsonSerializer.Deserialize<List<ParamSwitcher>>(jsonstring);
            return allFns.Select(n => Compile($"={n.Name}({secretCharacter})")).ToArray();
        }
        private void InitializeParamNames() {
            string jsonstring = File.ReadAllText("paramNames.json");
            List<ParamSwitcher>? allFns = JsonSerializer.Deserialize<List<ParamSwitcher>>(jsonstring);
            ParamSwitcher fn = allFns.Where(x => x.Name == Name).First();
            ParameterNames = fn.Values;
            Description = fn.Description;
        }
        public string Invoke() {
            switch (Name) {
                case "SZUM":
                    return SumOrAvg().ToString();
                case "ÁTLAG":
                    return SumOrAvg(false).ToString();
                case "SZUMHA":
                    return SumIfOrAvgIf().ToString();
                case "ÁTLAGHA":
                    return SumIfOrAvgIf(false).ToString();
                case "DARAB":
                    return Count().ToString();
                case "DARABHA":
                    return CountIf().ToString();
                case "MAX":
                    return Extreme().ToString();
                case "MIN":
                    return Extreme(false).ToString();
                case "INDEX":
                    return Index();
                case "HOL.VAN":
                    return WhereIs().ToString();
                case "BAL":
                    return LeftOrRight();
                case "JOBB":
                    return LeftOrRight(false);
                default:
                    throw new Exception("Ezt hogy csináltad, kedves User?!?");
                    //sima eval?
            }
        }
        private double SumOrAvg(bool sumOnly = true) {
            int sum = 0;
            int count = 0;
            foreach (var param in Parameters) {
                try {
                    sum += int.Parse(param.Value);
                    count++;
                }
                catch { }
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
            return 0;
        }
        private int WhereIs() {
            return 0;
        }
        private string Index() {
            return "";
        }
        private string LeftOrRight(bool left = true) {
            if(left) {
                return string.Join(string.Empty, Parameters["Szöveg"].ToList().Take(int.Parse(Parameters["n"])));
            }
            return string.Join(string.Empty, Parameters["Szöveg"].ToList().Skip(Parameters["Szöveg"].Length - int.Parse(Parameters["n"])));
        }
        //private static double Evaluate(string expression) {
        //    System.Data.DataTable table = new System.Data.DataTable();
        //    table.Columns.Add("expression", string.Empty.GetType(), expression);
        //    System.Data.DataRow row = table.NewRow();
        //    table.Rows.Add(row);
        //    return double.Parse((string)row["expression"]);
        //} -- meg kesobb jol johet, ha megertettem
        public static Function? Compile(string arg) {
            if (arg.Length == 0) return null; // nem feltetlen = vel kezdodik
            try {
                if (!arg.Contains('(') || arg[^1] != ')') return null;
                string[] pcs = string.Join(string.Empty, arg.ToList().Skip(1)).Split('('); // egyenloseg jelet leveszi
                string _name = pcs[0];
                string _params = string.Join(string.Empty, pcs[1].Take(pcs[1].Length - 1));
                return new Function(_name, _params);
            }
            catch {
                return null;
            }
        }
    }
}
