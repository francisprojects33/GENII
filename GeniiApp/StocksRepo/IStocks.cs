using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.StocksRepo
{
    public interface IStocks
    {
        Task<bool> IsStocksLevelLowAsync(IList<StockProductItems> stockProductItems);
        Task<string> SendNotificationsAsync();
    }
}
