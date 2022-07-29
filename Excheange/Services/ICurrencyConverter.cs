using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excheange.Services
{
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Clears any prior configuration.
        /// </summary>
        void ClearConfiguration();

        /// <summary> 
        /// Updates the configuration. Rates are inserted or replaced
        /// internally.
        /// </summary> 
        /// پیشنهادم اینه که از ابجکت استفاده بشه به جای تاپل
        /// اینسرت و اپدیت اطلاعات را میتونید از طریق وارد کردن فایل اکسل انجام داد
        void UpdateConfiguration(IEnumerable<ExcheangTo>
        conversionRates);

        /// <summary> 
        /// Converts the specified amount to the desired currency. 
        /// </summary>
        double Convert(string fromCurrency, string toCurrency, double amount);
        string GetShortestPathConverting();
    }
}
