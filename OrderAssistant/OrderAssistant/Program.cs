using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using static System.String;
using Excel = Microsoft.Office.Interop.Excel;

namespace OrderAssistant
{
	class Program
	{
		static void Main(string[] args)
		{
			var curWb = new Excel.Application().Workbooks.Open(Config.Import.OrderStocksAndTraffic.FileName);
			dynamic curSheet = curWb.Worksheets.Item[1];
			var curRow = Config.Import.OrderStocksAndTraffic.FirstRow;
			var lastRow = curSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
			var curStock = new stock();
			var curDate = new DateTime();
			using (var context = new orderAssistantEntities())
			{
				while (curRow <= lastRow)
				{
					// Date
					if (curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColDate].Value != null)
					{
						try
						{
							curDate = DateTime.Parse(curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColDate].Value);
							Console.WriteLine(curDate.ToString(CultureInfo.InvariantCulture));
							curRow++;
							continue;
						}
						catch (FormatException e)
						{
							//TODO Добавить логирование если дата не создалась
							Console.WriteLine("Дата не является датой");
							Console.WriteLine(e);
							curRow++;
							continue;
						}
					}
					// Stock
					if (curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColStock].Value != null)
					{
						try
						{
							curStock = TakeStock(curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColStock].Value, context);
							Console.WriteLine(curStock.name);
							curRow++;
							continue;
						}
						catch (Exception e)
						{
							//TODO Добавить логирование если склад не нашелся
							Console.WriteLine(e);
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
						catNumber = curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColCatNumber].Value;
						name = curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColName].Value;
						count = (decimal)curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColCount].Value;
						id1C = curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.Col1CId].Value;
						manufacturerStr = curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColManufacturer].Value;
						brendStr = curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColBrend].Value;
						cost = (decimal)curSheet.Cells[curRow, Config.Import.OrderStocksAndTraffic.ColCost].Value;
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						Console.WriteLine(Join("Ошибка в строке", curRow));
						curRow++;
						continue;
					}

					// Проверка на отрицательные остатки
					if (cost <= 0 || count <= 0)
					{
						curRow++;
						Console.WriteLine("Себестоимость или количество меньше или равно нулю.");
						continue;
					}

					// Проверка на пустые строки
					if (IsNullOrEmpty(catNumber) ||
						IsNullOrEmpty(name) ||
						IsNullOrEmpty(id1C) ||
						IsNullOrEmpty(manufacturerStr) ||
						IsNullOrEmpty(brendStr))
					{
						curRow++;
						Console.WriteLine("Строка содержит пустые параметры");
						continue;
					}
					Console.WriteLine(id1C);
					var item = GetItem(name, id1C, manufacturerStr, brendStr, catNumber, context);
					item.balances.Add(GetBalance(curDate, curStock, cost, item, count, context));
					curRow++;
				}
			}
		}

		/// <summary>
		/// Возвращает ссылку на бренд либо созданый либо найденный, ищет без учета регистра.
		/// </summary>
		/// <param name="nameStr">Название</param>
		/// <param name="context"></param>
		/// <returns>Ссылка на бренд</returns>
		public static brend GetBrend(string nameStr, orderAssistantEntities context)
		{
			// Проверяем, есть ли такой бренд
			var brend = (from b in context.brends
						 where b.name.ToLower().Contains(nameStr.ToLower())
						 select b).FirstOrDefault();
			if (brend == null)
			{
				var newBrend = new brend()
				{
					name = nameStr
				};
				brend = context.brends.Add(newBrend);
			}
			context.SaveChanges();
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
			var manufacturer = (from m in context.manufacturers
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
			context.SaveChanges();
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
			var item = (from i in context.items
						where i.id1C == id1C
						select i).FirstOrDefault();
			// Если такого item нет, создаем
			if (item == null)
			{
				var newItem = new item()
				{
					id1C = id1C,
					manufacturer = GetManufacturer(manufacturerStr, context),
					brend = GetBrend(brendStr, context),
					catNumber = catNumber,
					name = name,
					ABCgroup = "D" //TODO должно само в базе подставляться. но почемуто не хочет
				};
				item = context.items.Add(newItem);
				return item;
			}
			// Иначе обновляем
			item.manufacturer = GetManufacturer(manufacturerStr, context);
			item.brend = GetBrend(brendStr, context);
			item.catNumber = catNumber;
			item.name = name;
			context.SaveChanges();
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
			var balance = (from b in context.balances
						   where b.stock.id == stock.id && b.dateCount == date && b.item.id == item.id
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
			context.SaveChanges();
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
