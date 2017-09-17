using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using static System.String;
using Excel = Microsoft.Office.Interop.Excel;
using NLog;


namespace OrderAssistant
{
	class Program
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			var curWb = new Excel.Application().Workbooks.Open(Config.Import.OrderStocksAndTraffic.FileName);
			dynamic curSheet = curWb.Worksheets.Item[1];
			var curRow = Config.Import.OrderStocksAndTraffic.FirstRow;
			var lastRow = curSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
			var curStock = new stock();
			var curDate = new DateTime();

			// Считываем лист в массив
			var range = (Excel.Range)curSheet.Range[curSheet.Cells[1, 1], curSheet.Cells[lastRow, 30]];
			var dataArr = (object[,])range.Value;
			using (var context = new orderAssistantEntities())
			{
				context.items.Load();
				context.brands.Load();
				context.manufacturers.Load();
				context.balances.Load();
				context.stocks.Load();

				while (curRow <= lastRow)
				{
					// Date
					if (dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColDate] != null)
					{
						try
						{
							curDate = DateTime.Parse(dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColDate].ToString());
							curRow++;
							continue;
						}
						catch (FormatException e)
						{
							logger.Error("Дата не является датой. Строка {0}. {1}", curRow, e.Message);
							curRow++;
							continue;
						}
					}
					// Stock
					if (dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColStock] != null)
					{
						try
						{
							curStock = TakeStock(dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColStock].ToString(), context);
							curRow++;
							continue;
						}
						catch (Exception e)
						{
							logger.Error("Склад не найден в БД. Строка {0}. {1}", curRow, e.Message);
							curRow++;
							continue;
						}
					}

					string catNumber;
					string name;
					decimal count;
					string id1C;
					string manufacturerStr;
					string brendStr;
					decimal cost;
					try
					{
						catNumber = dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColCatNumber].ToString();
						name = dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColName].ToString();
						count = Convert.ToDecimal(dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColCount]);
						id1C = dataArr[curRow, Config.Import.OrderStocksAndTraffic.Col1CId].ToString();
						manufacturerStr = dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColManufacturer].ToString();
						brendStr = dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColBrend].ToString();
						cost = Convert.ToDecimal(dataArr[curRow, Config.Import.OrderStocksAndTraffic.ColCost]);
					}
					catch (Exception e)
					{
						logger.Error("Ошибка конвертации данных в строке {0}. {1}", curRow, e.Message);
						curRow++;
						continue;
					}

					// Проверка на отрицательные остатки
					if (cost <= 0 || count <= 0)
					{
						logger.Warn("Себестоимость или количество меньше или равно нулю. Строка {0}", curRow);
						curRow++;
						continue;
					}

					// Проверка на пустые строки
					if (IsNullOrEmpty(catNumber) ||
						IsNullOrEmpty(name) ||
						IsNullOrEmpty(id1C) ||
						IsNullOrEmpty(manufacturerStr) ||
						IsNullOrEmpty(brendStr))
					{
						logger.Warn("Строка {0} содержит пустые параметры", curRow);
						curRow++;
						continue;
					}
					Console.WriteLine(id1C);
					var item = GetItem(name, id1C, manufacturerStr, brendStr, catNumber, context);
					item.balances.Add(GetBalance(curDate, curStock, cost, item, count, context));

					if (curRow % Config.Import.LoadAfter == 0)
					{
						Console.WriteLine("Загрузка в БД строка {0}", curRow);
						context.SaveChanges();
					}
					curRow++;
				}
				Console.WriteLine("Загрузка в БД строка {0}", curRow);
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Возвращает ссылку на бренд либо созданый либо найденный, ищет без учета регистра.
		/// </summary>
		/// <param name="nameStr">Название</param>
		/// <param name="context"></param>
		/// <returns>Ссылка на бренд</returns>
		public static brand GetBrand(string nameStr, orderAssistantEntities context)
		{
			// Проверяем, есть ли такой бренд
			var brend = (from b in context.brands.Local
						 where b.name.ToLower().Contains(nameStr.ToLower())
						 select b).FirstOrDefault();
			if (brend == null)
			{
				var newBrend = new brand()
				{
					name = nameStr
				};
				brend = context.brands.Add(newBrend);
			}
			return brend;
		}

		/// <summary>
		/// Возвращает ссылку на производителя либо созданого либо найденного, ищет без учета регистра.
		/// </summary>
		/// <param name="nameStr">Название производителя</param>
		/// <param name="context"></param>
		/// <returns>Ссылка на производителя</returns>
		public static manufacturer GetManufacturer(string nameStr, orderAssistantEntities context)
		{

			// Проверяем, есть ли такой бренд
			var manufacturer = (from m in context.manufacturers.Local
								where m.name.ToLower().Contains(nameStr.ToLower())
								select m).FirstOrDefault();
			if (manufacturer == null)
			{
				var newManufacturer = new manufacturer()
				{
					name = nameStr
				};
				manufacturer = context.manufacturers.Add(newManufacturer);
			}
			return manufacturer;
		}

		/// <summary>
		/// Возвращает ссылку на item либо созданный либо найденный
		/// </summary>
		/// <param name="name">Название ЗЧ</param>
		/// <param name="id1C">Код 1С</param>
		/// <param name="manufacturerStr">Производитель название</param>
		/// <param name="brendStr">Название бренда</param>
		/// <param name="catNumber">Артикул</param>
		/// <param name="context"></param>
		/// <returns>Ссылка на item</returns>
		public static item GetItem(string name, string id1C, string manufacturerStr, string brendStr, string catNumber,
			orderAssistantEntities context)
		{
			// Проверяем есть ли такой item
			var item = (from i in context.items.Local
						where i.id1C == id1C
						select i).FirstOrDefault();
			// Если такого item нет, создаем
			if (item == null)
			{
				var newItem = new item()
				{
					id1C = id1C,
					manufacturer = GetManufacturer(manufacturerStr, context),
					brand = GetBrand(brendStr, context),
					catNumber = catNumber,
					name = name,
					ABCgroup = "D" //TODO должно само в базе подставляться. но почемуто не хочет
				};
				item = context.items.Add(newItem);
				return item;
			}
			// Иначе обновляем
			item.manufacturer = GetManufacturer(manufacturerStr, context);
			item.brand = GetBrand(brendStr, context);
			item.catNumber = catNumber;
			item.name = name;
			return item;
		}

		/// <summary>
		/// Возвращает ссылку на balance либо созданный либо найденный
		/// </summary>
		/// <param name="date">дата остатка</param>
		/// <param name="stock">склад</param>
		/// <param name="cost">Себестоимость</param>
		/// <param name="item">Запчасть</param>
		/// <param name="count">Количество на складе</param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static balance GetBalance(DateTime date, stock stock, decimal cost, item item, decimal count,
			orderAssistantEntities context)
		{
			// Проверяем есть такая запись или нет
			var balance = (from b in context.balances.Local
						   where b.stock.id == stock.id && b.dateCount == date && b.item.id1C == item.id1C
						   select b).FirstOrDefault();
			// Если нет создаем
			if (balance == null)
			{
				var newBalance = new balance()
				{
					dateCount = date,
					stock = stock,
					cost = cost,
					item = item,
					count = count
				};
				balance = context.balances.Add(newBalance);
			}
			// Или обновляем
			else
			{
				balance.cost = cost;
				balance.count = count;
			}
			return balance;
		}
		/// <summary>
		/// Возвращает Stock который подходит под определенную сигнатуру, без учета регистра
		/// </summary>
		/// <param name="stockStr">строка с описанием скалада</param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static stock TakeStock(string stockStr, orderAssistantEntities context)
		{
			if (IsNullOrEmpty(stockStr))
			{
				throw new Exception("Строка пуста или NULL");
			}
			return Enumerable.FirstOrDefault(context.stocks, stock => stockStr.ToLower().Contains(stock.signature));
		}
	}
}
