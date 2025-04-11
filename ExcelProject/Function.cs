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

namespace ExcelProject
{
    public class Function {
        public string Name { get; private set; }
        private string[] ParameterNames { get; set; }
        private Dictionary<string, string> Parameters { get; set; } = new();
        public static string[] allFnNames { get; set; }
        public string Description { get; private set; }

        public Function(string _name, string _params) {
            Name = _name;
            InitializeParamNames();
            string[] paramVals = _params.Split(';');
            if (ParameterNames[0].StartsWith("*")) {
                int minus = ParameterNames.Length - 1;
                for (int i = 0; i < paramVals.Length - minus; i++) {
                    try {
                        Parameters.Add($"param{i + 1}", paramVals[i]); // tartományt valahogy handle-elni
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
        private void InitializeParamNames() {
            string jsonstring = File.ReadAllText("paramNames.json");
            var allFns = JsonSerializer.Deserialize<List<ParamSwitcher>>(jsonstring);
            allFnNames = allFns.Select(n => n.Name).ToArray();
            var fn = allFns.Where(x => x.Name == Name).First();
            ParameterNames = fn.Values;
            Description = fn.Description;
        }
        public string Invoke() {
            switch (Name) {
                case "SZUM":
                    return SumOrAvg(true).ToString();
                case "ÁTLAG":
                    return SumOrAvg(false).ToString();
                case "SZUMHA":
                    return SumIfOrAvgIf(true).ToString();
                case "ÁTLAGHA":
                    return SumIfOrAvgIf(false).ToString();
                case "DARAB":
                    return Count().ToString();
                case "DARABHA":
                    return CountIf().ToString();
                case "MAX":
                    return Extreme(true).ToString();
                case "MIN":
                    return Extreme(false).ToString();
                case "INDEX":
                    return Index();
                case "HOL.VAN":
                    return WhereIs().ToString();
                case "BAL":
                    return LeftOrRight(true);
                case "JOBB":
                    return LeftOrRight(true);
                default:
                    throw new Exception("Ezt hogy csináltad, kedves User?!?");
            }
        }
        private double SumOrAvg(bool sumOnly) {
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
        private double SumIfOrAvgIf(bool sumOnly) {
            return 0;
        }
        private int Count() {
            return 0;
        }
        private int CountIf() {
            return 0;
        }
        private double Extreme(bool max) {
            return 0;
        }
        private int WhereIs() {
            return 0;
        }
        private string Index() {
            return "";
        }
        private string LeftOrRight(bool left) {
            return "";
        }
    }
}
