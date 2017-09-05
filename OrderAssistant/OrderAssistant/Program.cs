using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace OrderAssistant
{
	class Program
	{
		static void Main(string[] args)
		{
			var curWb = new Excel.Application().Workbooks.Open(Config.ImportOrderStocksAndTrafficFileName);
			var curSheet = curWb.Worksheets.Item[1];
			var curRow = Config.ImportOrderStocksAndTrafficFirstRow;
			var lastRow = curSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
			var curStock = new stock();
			var curDate = new DateTime();
			using (var context = new orderAssistantEntities())
			{
				do
				{
					// Date
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColDate].Value, true, true))
					{
						//TODO Добавить логирование если дата не создалась
						curDate = DateTime.Parse(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColDate].Value);
						Console.WriteLine(curDate.ToString(CultureInfo.InvariantCulture));
						curRow++;
						continue;
					}
					// Stock
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColStock].Value, true, true))
					{
						//TODO Добавить логирование если склад не нашелся
						curStock = TakeStock(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColStock].Value, context);
						Console.WriteLine(curStock.name);
						curRow++;
						continue;
					}
					// CatNumber
					string catNumber;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCatNumber].Value, true, true))
					{
						catNumber = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCatNumber].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке " , curRow , " столбец " , Config.ImportOrderStocksAndTrafficColCatNumber));//TODO в лог
						curRow++;
						continue;
					}
					// Name
					string name;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColName].Value, true, true))
					{
						name = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColName].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке ", curRow, " столбец ", Config.ImportOrderStocksAndTrafficColName));//TODO в лог
						curRow++;
						continue;
					}
					// Count
					decimal count;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCount].Value, isDb: true,
						isNotNegative: true))
					{
						//TODO может быть ошибка
						count = (decimal)curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCount].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке ", curRow, " столбец ", Config.ImportOrderStocksAndTrafficColCount));//TODO в лог
						curRow++;
						continue;
					}
					// 1C Code
					string id1C;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficCol1CId].Value, true, true))
					{
						id1C = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficCol1CId].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке ", curRow, " столбец ", Config.ImportOrderStocksAndTrafficCol1CId));//TODO в лог
						curRow++;
						continue;
					}
					// Manufacturer
					string manufacturerStr;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColManufacturer].Value, true, true))
					{
						manufacturerStr = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColManufacturer].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке ", curRow, " столбец ", Config.ImportOrderStocksAndTrafficColManufacturer));//TODO в лог
						curRow++;
						continue;
					}
					// Brend
					string brendStr;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColBrend].Value, true, true))
					{
						brendStr = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColBrend].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке ", curRow, " столбец ", Config.ImportOrderStocksAndTrafficColBrend));//TODO в лог
						curRow++;
						continue;
					}
					// Cost
					decimal cost;
					if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCost].Value, isDb: true, isNotNegative: true))
					{
						//TODO может быть ошибка
						cost = (decimal)curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCost].Value;
					}
					else
					{
						Console.WriteLine(string.Join("Ошибка в строке ", curRow, " столбец ", Config.ImportOrderStocksAndTrafficColCost));//TODO в лог
						curRow++;
						continue;
					}

					Console.WriteLine(id1C);
					var item = GetItem(name, id1C, manufacturerStr, brendStr, catNumber, context);
					item.balances.Add(GetBalance(curDate, curStock, cost, item, count, context));
					curRow++;
				} while (curRow <= lastRow);
			}
		}

		/// <summary>
		/// Проверяет соответствует ли значение checkable необходимым требованиям, требования задаются в виде набора деректив ДА/НЕТ
		/// </summary>
		/// <param name="checkable">Проверяемое</param>
		/// <param name="isString">Должно быть строкой</param>
		/// <param name="isNotEmptyString">Должно быть не пустой строкой</param>
		/// <param name="isDb">Должно быть double</param>
		/// <param name="isNotNegative">Не должно быть отрицательным</param>
		/// <param name="isDecimal">Должно быть Decimal</param>
		/// <returns></returns>
		public static bool Check(dynamic checkable, bool isString = false, bool isNotEmptyString = false, bool isDb = false,
			bool isNotNegative = false, bool isDecimal = false)
		{
			// Если не строка возвращаем ложь
			if (isString && !(checkable is string))
			{
				return false;
			}

			// Если пустая строка возвращаем ложь
			if (isNotEmptyString && checkable is null)
			{
				return false;
			}

			// Если это не число
			if (isDb && !(checkable is double))
			{
				return false;
			}

			// Если это не decimal
			//TODO не работает
			if (isDecimal && !(checkable is decimal))
			{
				return false;
			}

			// Если число меньше 0
			if (isNotNegative && checkable < 0)
			{
				return false;
			}
			return true;
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
				context.brends.Add(newBrend);
				context.SaveChanges();
			}
			else
			{
				return brend;
			}
			return (from b in context.brends
					where b.name.ToLower().Contains(nameStr.ToLower())
					select b).FirstOrDefault();
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
				context.manufacturers.Add(newManufacturer);
				context.SaveChanges();
			}
			else
			{
				return manufacturer;
			}
			return (from m in context.manufacturers
					where m.name.ToLower().Contains(nameStr.ToLower()) 
					select m).FirstOrDefault();
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
				context.items.Add(newItem);
				context.SaveChanges();

				item = (from i in context.items
						where i.id1C == id1C
						select i).FirstOrDefault();
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
				context.balances.Add(newBalance);
			}
			// Или обновляем
			else
			{
				balance.cost = cost;
				balance.count = count;
			}
			context.SaveChanges();

			return (from b in context.balances
					where b.stock.id == stock.id && b.dateCount == date && b.item.id == item.id
					select b).FirstOrDefault();
		}
		/// <summary>
		/// Возвращает Stock который подходит под определенную сигнатуру, без учета регистра
		/// </summary>
		/// <param name="stockStr">строка с описанием скалада</param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static stock TakeStock(string stockStr, orderAssistantEntities context)
		{
			return Enumerable.FirstOrDefault(context.stocks, stock => stockStr.ToLower().Contains(stock.signature));
		}
	}
}
