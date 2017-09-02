using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            DateTime curDate;
            string curStock;
            double count;

            do
            {
                // Date
                if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColDate].Value, true, true))
                {
                    curDate = DateTime.Parse(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColDate].Value);
                    curRow++;
                    continue;
                }
                // Stock
                if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColStock].Value, true, true))
                {
                    curStock = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColStock].Value;
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
                    curRow++;
                    continue;
                }
                // Count
                if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCount].Value, isDb: true,
                    isNotNegative: true))
                {
                    count = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColCount].Value;
                }
                else
                {
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
                    curRow++;
                    continue;
                }

                using (var context = new orderAssistantEntities())
                {
                    Console.WriteLine( id1C, GetItem(name, id1C, manufacturerStr, brendStr, catNumber, context)); 
                }
                curRow++;
            } while (curRow <= lastRow);
        }
        /// <summary>
        /// Проверяет соответствует ли значение checkable необходимым требованиям, требования задаются в виде набора деректив ДА/НЕТ
        /// </summary>
        /// <param name="checkable">Проверяемое</param>
        /// <param name="isString">Должно быть строкой</param>
        /// <param name="isNotEmptyString">Должно быть не пустой строкой</param>
        /// <param name="isDb">Должно быть double</param>
        /// <param name="isNotNegative">Не должно быть отрицательным</param>
        /// <returns></returns>
        public static bool Check(dynamic checkable, bool isString = false, bool isNotEmptyString = false, bool isDb = false, bool isNotNegative = false)
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

            // Если число меньше 0
            if (isNotNegative && checkable < 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Возвращает ссылку на бренд либо созданый либо найденный
        /// </summary>
        /// <param name="nameStr">Название</param>
        /// <param name="context"></param>
        /// <returns>Ссылка на бренд</returns>
        public static brend GetBrend(string nameStr, orderAssistantEntities context)
        {
            for (var i = 0; i < 2; i++)
            {
                // Проверяем, есть ли такой бренд
                var brend = (from b in context.brends
                             where b.name.Contains(nameStr) //TODO Contains ищет с учетом регистра, а нужно видимо без
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
            }
            return null;
        }

        /// <summary>
        /// Возвращает ссылку на производителя либо созданого либо найденного
        /// </summary>
        /// <param name="nameStr">Название производителя</param>
        /// <param name="context"></param>
        /// <returns>Ссылка на производителя</returns>
        public static manufacturer GetManufacturer(string nameStr, orderAssistantEntities context)
        {
            for (var i = 0; i < 2; i++)
            {
                // Проверяем, есть ли такой бренд
                var manufacturer = (from m in context.manufacturers
                                    where m.name.Contains(nameStr) //TODO Contains ищет с учетом регистра, а нужно видимо без
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
            }
            return null;
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
            for (var a = 0; a < 2; a++)
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
                }
                else
                {
                    return item;
                }
            }
            return null;
        }
    }
}
