using System;
using System.IO;
using System.Xml.Serialization;
using NLog;

namespace OrderAssistantUI
{
	[Serializable]
	public class Config
	{
		public Imports Imports = Imports.Inst;
		/// <summary>
		/// Загрузить настройки </summary>
		static Config()
		{
			try
			{
				var serializer = new XmlSerializer(typeof(Config)); using (var stream = File.OpenRead("config.xml"))
				{
					_inst = (Config)serializer.Deserialize(stream);
				}
			}
			catch (Exception e)
			{
				LogManager.GetCurrentClassLogger().Error("Ошибка загрузки настроек. {0}", e.Message);
				//TODO как закончить выполнение функции, вызваться может где угодно
			}
		}
		/// <summary>
		/// Загрузить настройки
		/// </summary>
		public static void Save(){
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
	public class Imports
	{
		/// <summary>
		/// Количество строк после которых осуществляется загрузка в базу
		/// </summary>
		public int LoadAfter;

		public OrderStocksAndTraffic OrderStocksAndTraffic = OrderStocksAndTraffic.Inst;

		//Singleton
		private Imports() { }
		private static Imports _inst;
		public static Imports Inst => _inst ?? (_inst = new Imports());
	}

	[Serializable]
	public class OrderStocksAndTraffic : Order
	{
		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая бренд.
		/// </summary>
		public int ColBrend;

		/// <summary>
		/// Импорт. Отчет остатки и обороты. Колонка содержащая производителя.
		/// </summary>
		public int ColManufacturer;

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

	public abstract class Order
	{
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
		/// Импорт. Отчет остатки и обороты. Колонка содержащая количество.
		/// </summary>
		public int ColCount;

		/// <summary>
		/// Поправка на последнюю строку, насколько выше заканчиваются данные от последней заполненной строки.
		/// </summary>
		public int LastRowCorrection;

		/// <summary>
		/// Строка с сигнатурой отчета.
		/// </summary>
		public int RowSign;

		/// <summary>
		/// Колонка с сигнатурой отчета.
		/// </summary>
		public int ColSign;

		/// <summary>
		/// Сигнатура отчета
		/// </summary>
		public string Sign;
	}

}
