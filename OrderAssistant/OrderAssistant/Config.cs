using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAssistant
{
    public  static class Config
    {
	    public static class Import
	    {
			public static class OrderStocksAndTraffic
			{
				/// <summary>
				/// Импорт. Отчет остатки и обороты. Полное имя файла.
				/// </summary>
				public static string FileName = "D:\\Dropbox\\dev\\git\\OrderAssistant\\OrderAssistant\\OrderAssistant\\bin\\Остатки для БД(новая).xls";

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Первая строка с которой начинаются данные.
				/// </summary>
				public static int FirstRow = 7;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая название запчасти.
				/// </summary>
				public static int ColName = 18;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая каталожный номер запчасти.
				/// </summary>
				public static int ColCatNumber = 4;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая код 1С.
				/// </summary>
				public static int Col1CId = 23;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая бренд.
				/// </summary>
				public static int ColBrend = 30;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая производителя.
				/// </summary>
				public static int ColManufacturer = 29;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая количество.
				/// </summary>
				public static int ColCount = 21;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая себестоимость.
				/// </summary>
				public static int ColCost = 22;

				/// <summary>   
				/// Импорт. Отчет остатки и обороты. Колонка содержащая дату остатка.
				/// </summary>
				public static int ColDate = 2;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая название склада.
				/// </summary>
				public static int ColStock = 3;
			}
	    }
    }
}
