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
            string catNumber;
            string name;
            double count;
            string manufacturerStr;
            string brendStr;

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
                if (Check(curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColBrend].Value, true, true))
                {
                    brendStr = curSheet.Cells[curRow, Config.ImportOrderStocksAndTrafficColBrend].Value;
                }
                else
                {
                    curRow++;
                    continue;
                }

                // Проверяем, существует ли такой item
                using (var context = new orderAssistantEntities())
                {
                    //var supplier = new supplier()
                    //{
                    //    name = 100
                    //};
                    //context.suppliers.Add(supplier);

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
                }
                curRow++;
            } while (curRow <= lastRow);
        }

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

        public static brend GetBrend(string brendStr, orderAssistantEntities context)
        {
            using ( context )
            {
                for (var i = 0; i < 2; i++)
                {
                    // Проверяем, есть ли такой бренд
                    var brend = (from b in context.brends
                        where b.name.Contains(brendStr) //TODO Contains ищет с учетом регистра, а нужно видимо без
                        select b).FirstOrDefault();
                    if (brend == null)
                    {
                        var newBrend = new brend()
                        {
                            name = brendStr
                        };
                        context.brends.Add(newBrend);
                        //context.SaveChanges(); 
                    }
                    else
                    {
                        return brend;
                    }
                }
            }
            return null;
        }

        public static manufacturer GetManufacturer(string manufacturerStr,orderAssistantEntities context)
        {
            using ( context )
            {
                for (var i = 0; i < 2; i++)
                {
                    // Проверяем, есть ли такой бренд
                    var manufacturer = (from m in context.manufacturers
                        where m.name.Contains(manufacturerStr) //TODO Contains ищет с учетом регистра, а нужно видимо без
                        select m).FirstOrDefault();
                    if (manufacturer == null)
                    {
                        var newManufacturer = new manufacturer()
                        {
                            name = manufacturerStr
                        };
                        context.manufacturers.Add(newManufacturer);
                        //context.SaveChanges();
                    }
                    else
                    {
                        return manufacturer;
                    }
                }
            }
            return null;
        }
    }
}
