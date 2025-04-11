using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExcelProject
{
    public class Function
    {
        public string Name { get; set; }
        public string[] ParameterNames { get; set; }
        //public object[] Parameters { get; set; }
        public string Description { get; set; }
        public Function(string _name, string _params) {
            Name = _name;
            InitializeParamNames();
            //ParameterName
        }
        public void InitializeParamNames()
        {
            
            //fájlból
        }
        public void Invoke(string _paramValues)
        {

        }
    
    }
}
