using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using NLog;

namespace OrderAssistant
{
	[Serializable]
	public class Config
	{
		public Import Import = Import.Inst;
		/// <summary>
		/// Загрузить настройки
		/// </summary>
		static Config()
		{
			try
			{
				var serializer = new XmlSerializer(typeof(Config));
				using (var stream = File.OpenRead("config.xml"))
				{
					_inst = (Config) serializer.Deserialize(stream);
				}
			}
			catch (Exception e)
			{
				LogManager.GetCurrentClassLogger().Error("Ошибка загрузки настроек. {0}", e.Message);
				//TODO как закончить выполнение функции
			}
		}
		/// <summary>
		/// Загрузить настройки
		/// </summary>
		public static void Save()
		{
			try
			{
				var serializer = new XmlSerializer(typeof(Config));
				var i = new Config();
				Stream writer = new FileStream("config.xml", FileMode.OpenOrCreate);
				serializer.Serialize(writer, i);
				writer.Close();
			}
			catch (Exception e)
			{
				LogManager.GetCurrentClassLogger().Error("Ошибка сохранения настроек. {0}", e.Message);
			}
		}

		//Singleton
		private Config() { }
		private static Config _inst;
		public static Config Inst => _inst ?? (_inst = new Config());
	}
	[Serializable]
	public class Import
	{
		/// <summary>
		/// Количество строк после которых осуществляется загрузка в базу
		/// </summary>
		public int LoadAfter;

		public OrderStocksAndTraffic OrderStocksAndTraffic = OrderStocksAndTraffic.Inst;

		//Singleton
		private Import() { }
		private static Import _inst;
		public static Import Inst => _inst ?? (_inst = new Import());
	}
	[Serializable]
	public class OrderStocksAndTraffic
	{
		/// <summary>
		/// Импорт. Отчет остатки и обороты. Полное имя файла.
		/// </summary>
		public string FileName;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Первая строка с которой начинаются данные.
		/// </summary>
		public int FirstRow;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая название запчасти.
		/// </summary>
		public int ColName;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая каталожный номер запчасти.
		/// </summary>
		public int ColCatNumber;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая код 1С.
		/// </summary>
		public int Col1CId;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая бренд.
		/// </summary>
		public int ColBrend;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая производителя.
		/// </summary>
		public int ColManufacturer;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая количество.
		/// </summary>
		public int ColCount;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая себестоимость.
		/// </summary>
		public int ColCost;

		/// <summary>   
		/// Импорт. Отчет остатки и обороты. Колонка содержащая дату остатка.
		/// </summary>
		public int ColDate;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая название склада.
		/// </summary>
		public int ColStock;

		//Singleton
		private OrderStocksAndTraffic() { }
		private static OrderStocksAndTraffic _inst;
		public static OrderStocksAndTraffic Inst => _inst ?? (_inst = new OrderStocksAndTraffic());
	}

}
