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
				public const string FileName = "D:\\Dropbox\\dev\\git\\OrderAssistant\\OrderAssistant\\OrderAssistant\\bin\\Остатки для БД(новая).xls";

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Первая строка с которой начинаются данные.
				/// </summary>
				public const int FirstRow = 7;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая название запчасти.
				/// </summary>
				public const int ColName = 18;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая каталожный номер запчасти.
				/// </summary>
				public const int ColCatNumber = 4;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая код 1С.
				/// </summary>
				public const int Col1CId = 23;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая бренд.
				/// </summary>
				public const int ColBrend = 30;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая производителя.
				/// </summary>
				public const int ColManufacturer = 29;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая количество.
				/// </summary>
				public const int ColCount = 21;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая себестоимость.
				/// </summary>
				public const int ColCost = 22;

				/// <summary>   
				/// Импорт. Отчет остатки и обороты. Колонка содержащая дату остатка.
				/// </summary>
				public const int ColDate = 2;

				/// <summary>
				/// Импорт. Отчет остатки и обороты. Колонка содержащая название склада.
				/// </summary>
				public const int ColStock = 3;
			}

			/// <summary>
			/// Количество строк после которых осуществляется загрузка в базу
			/// </summary>
		    public const int LoadAfter = 100;
	    }
    }
}
