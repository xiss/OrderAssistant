using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAssistant
{
    public  static class Config
    {
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Полное имя файла.
        /// </summary>
        public static string ImportOrderStocksAndTrafficFileName = "D:\\Dropbox\\dev\\git\\OrderAssistant\\OrderAssistant\\OrderAssistant\\bin\\Остатки для БД(новая).xls";
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Первая строка с которой начинаются данные.
        /// </summary>
        public static int ImportOrderStocksAndTrafficFirstRow = 7;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая название запчасти.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColName = 18;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая каталожный номер запчасти.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColCatNumber = 4; 
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая код 1С.
        /// </summary>
        public static int ImportOrderStocksAndTrafficCol1CId = 23;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая бренд.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColBrend = 30;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая производителя.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColManufacturer = 29;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая количество.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColCount = 21;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая себестоимость.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColCost = 22;
        /// <summary>   
        /// Импорт. Отчет остатки и обороты. Колонка содержащая дату остатка.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColDate = 2;
        /// <summary>
        /// Импорт. Отчет остатки и обороты. Колонка содержащая название склада.
        /// </summary>
        public static int ImportOrderStocksAndTrafficColStock = 3;

    }
}
