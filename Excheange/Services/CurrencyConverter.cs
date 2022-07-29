using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Excheange.Services.CurrencyConverter;

namespace Excheange.Services
{
    public class ExcheangTo
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double Amount { get; set; }
    }
    public class CurrencyConverter : ICurrencyConverter
    {
        private string _pathConverting;
        private List<ExcheangTo> _list;


        public void ClearConfiguration()
        {
            throw new NotImplementedException();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            var result = _list.ShortPath(null, fromCurrency, toCurrency, amount);
            _pathConverting = result.PathOfConverting;
            return result.FinalAmount;
        }

        public string GetShortestPathConverting()
        {
            return _pathConverting;
        }

        public void UpdateConfiguration(IEnumerable<ExcheangTo> conversionRates)
        {
            //_list =
            //    conversionRates.Select(s =>
            //    new ExcheangTo()
            //    {
            //        FromCurrency = s.Item1,
            //        ToCurrency = s.Item2,
            //        Amount = s.Item3,
            //    }).ToList();

            _list = conversionRates.ToList();
        }

    }
    public class Path
    {
        public string befor { get; set; }
        public string current { get; set; }
        public int StepCount { get; set; }
        public double Zarib { get; set; }
    }
    public class OutPut
    {
        public List<Path> ShortestPath { get; set; } = new List<Path>();
        public List<Path> CurrentPath { get; set; } = new List<Path>();
        public List<string> VisitCurrency { get; set; } = new List<string>();
        public string MinPath { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public int step { get; set; }
        public double FinalAmount { get; set; }
        public string PathOfConverting { get; set; }

    }
    public static class Extentions
    {
        #region Try2

        public static OutPut ShortPath(this IEnumerable<ExcheangTo> source, OutPut? beforOutPut, string from, string to, double amount)
        {

            if (beforOutPut == null)
            {
                beforOutPut = new OutPut();
                beforOutPut.StepOne(source, from, to);

                beforOutPut.step++;

                ShortPath(source, beforOutPut, beforOutPut.MinPath, to, amount);

            }
            else if (!(beforOutPut.VisitCurrency.Contains(beforOutPut.FromCurrency) && beforOutPut.VisitCurrency.Contains(beforOutPut.ToCurrency)))
            {

                beforOutPut.VisitCurrency.Add(from);

                beforOutPut.UpdateCurrentPath(source, from);

                if (beforOutPut.CurrentPath.Count != 0)
                {
                    int min = beforOutPut.GetMinPath();

                    beforOutPut.UpdateShortestPath(min);

                    beforOutPut.step++;

                    ShortPath(source, beforOutPut, beforOutPut.MinPath, to, amount);
                }
            }

            beforOutPut.ComputeFinalAmount(amount);

            return beforOutPut;

        }

        public static void UpdateCurrentPath(this OutPut beforOutPut, IEnumerable<ExcheangTo> source, string from)
        {
            foreach (var x in beforOutPut.ShortestPath)
            {
                if (beforOutPut.CurrentPath.Any(a => a == x))
                    beforOutPut.CurrentPath.Remove(x);
            }
            var currentPath = source.Where(x => (x.FromCurrency == from || x.ToCurrency == from)).ToList();
            foreach (var item in currentPath)
            {
                var path = new Path
                {
                    befor = from,
                    current = item.FromCurrency == from ? item.ToCurrency : item.FromCurrency,
                    StepCount = beforOutPut.step,
                    Zarib = item.FromCurrency == from ? item.Amount : 1 / item.Amount
                };
                bool iscurrect = !beforOutPut.VisitCurrency.Contains(path.current);
                bool issmaller = !beforOutPut.CurrentPath.Any(x => (x.current == path.current && x.StepCount < path.StepCount));
                bool isEmptyOrIsnot = beforOutPut.CurrentPath.Count == 0 || !beforOutPut.CurrentPath.Where(x => x == path || x.current == path.current).Any();
                if (iscurrect && (issmaller || isEmptyOrIsnot))
                {
                    beforOutPut.CurrentPath.Add(path);
                }

            }
        }

        public static int GetMinPath(this OutPut beforOutPut)
        {
            var min = beforOutPut.CurrentPath.Select(x => x.StepCount).Min();

            beforOutPut.MinPath = beforOutPut.CurrentPath.Where(x => x.StepCount == min || x.current == beforOutPut.ToCurrency).FirstOrDefault().current;
            return min;
        }

        public static void UpdateShortestPath(this OutPut beforOutPut, int min)
        {
            var shortPath = beforOutPut.CurrentPath.FirstOrDefault(x => x.StepCount == min && x.current == beforOutPut.MinPath);
            beforOutPut.ShortestPath.Add(shortPath);
        }
        public static void StepOne(this OutPut beforOutPut, IEnumerable<ExcheangTo> source, string from, string to)
        {

            beforOutPut.FromCurrency = from;
            beforOutPut.ToCurrency = to;
            beforOutPut.VisitCurrency.Add(from);
            beforOutPut.step = 1;
            var currentPath = source.Where(x => (x.FromCurrency == from || x.ToCurrency == from)).ToList();
            foreach (var item in currentPath)
            {
                beforOutPut.CurrentPath.Add(new Path
                {
                    befor = from,
                    current = item.FromCurrency == from ? item.ToCurrency : item.FromCurrency,
                    StepCount = beforOutPut.step,
                    Zarib = item.FromCurrency == from ? item.Amount : 1 / item.Amount
                });
            }

            beforOutPut.MinPath = beforOutPut.CurrentPath.FirstOrDefault().current;

            beforOutPut.ShortestPath.Add(beforOutPut.CurrentPath.FirstOrDefault(x => x.befor == from && x.current == beforOutPut.MinPath));
        }

        public static void ComputeFinalAmount(this OutPut result, double amount)
        {
            double zarib = 1;
            string to = "";
            if (result.FromCurrency != result.ToCurrency)
            {
                while (to != result.FromCurrency)
                {
                    if (string.IsNullOrEmpty(to))
                    {
                        var node = result.ShortestPath.Find(f => f.current == result.ToCurrency);

                        zarib *= node.Zarib;
                        to = node.befor;
                        result.PathOfConverting = $"' {node.befor} => {node.current} with zarib = {node.Zarib} '";
                    }
                    else
                    {
                        var node = result.ShortestPath.Find(f => f.current == to);
                        zarib *= node.Zarib;
                        to = node.befor;
                        string next = result.PathOfConverting;
                        result.PathOfConverting = $"' {node.befor} => {node.current} with zarib = {node.Zarib} ' <br/> { next}";
                    }
                }
                result.FinalAmount = amount * zarib;
            }
            else
            {
                result.FinalAmount = amount;
            }



        }


        #endregion

    }

}
