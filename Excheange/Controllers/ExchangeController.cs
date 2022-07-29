using Excheange.Models;
using Excheange.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excheange.Controllers
{
    public class ExchangeController : Controller
    {
        public List<ExcheangTo> list = new List<ExcheangTo>
        {
            new ExcheangTo(){FromCurrency="USD",ToCurrency="CAD",Amount=1.34},
            new ExcheangTo(){FromCurrency="CAD",ToCurrency="GBP",Amount=0.58},
            new ExcheangTo(){FromCurrency="USD",ToCurrency="EUR",Amount=0.86},
        };
        private readonly ICurrencyConverter _converter;
        public ExchangeController(ICurrencyConverter converter)
        {
            _converter = converter;
        }
        public IActionResult Index()
        {

            return View(list);
        }

        public IActionResult Convert()
        {
            ExcheangeModel model = new ExcheangeModel();
            model.CurrencyList = list.Select(s => s.ToCurrency).ToList();
            var listFrom = list.Where(x => !list.Select(s => s.ToCurrency).Contains(x.FromCurrency)).Select(s => s.FromCurrency).ToList();
            model.CurrencyList.AddRange(listFrom);
            return PartialView("_Convert",model);
        }
        [HttpPost]
        public IActionResult Convert(ExcheangeModel model)
        {
            _converter.UpdateConfiguration(list);
            var result = _converter.Convert( model.From, model.To, model.Amount);
            string pathConverting = _converter.GetShortestPathConverting();
            return Json(result+$"\n <hr/> path of convert <br/> {pathConverting}");
        }
    }
}
