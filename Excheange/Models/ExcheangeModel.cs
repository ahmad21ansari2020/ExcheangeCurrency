using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excheange.Models
{
    public class ExcheangeModel
    {
        public ExcheangeModel()
        {
            CurrencyList = new List<string>();
        }
        public double Amount { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public List<string> CurrencyList { get; set; }
    }
}
