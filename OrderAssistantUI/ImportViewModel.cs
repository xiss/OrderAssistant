using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System;
using System.Data.Entity;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using DevExpress.Data.Helpers;
using System.Windows.Forms;
using NLog;

namespace OrderAssistantUI
{
	[Export(typeof(ImportViewModel))]
	public class ImportViewModel : PropertyChangedBase, IImport
	{
		private readonly IWindowManager _windowManager;
		private CancellationTokenSource _cancelToken = new CancellationTokenSource();
		private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		[ImportingConstructor]
		public ImportViewModel(IWindowManager windowManager)
		{
			_windowManager = windowManager;
		}

		public async void ButtonStartImport()
		{
			var openFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Title = "�������� ������ '��������� ������� � �������'"
			};
			openFileDialog.ShowDialog();
			// ���� ������ �� ������� �������
			if (openFileDialog.FileName == string.Empty)
			{
				return;
			}
			try
			{
				await OrderStocksAndTrafficAsync(openFileDialog.FileNames, _cancelToken.Token);
			}
			catch (OperationCanceledException e)
			{
				Logger.Info("������ ������� �������������({0})", e.Message);
			}
			catch (Exception e)
			{
				Logger.Error("�������������� ������ ��� ������� ({0})", e.Message);
			}
			finally
			{
				IoC.Get<ImportViewModel>().SetDefault();
			}
		}

		private  async Task OrderStocksAndTrafficAsync(string[] fileNames, CancellationToken cancelToken)
		{
			await Task.Run(() =>
			{
				
				ButtonStartImportIsEnabled = false;
				ProgressBarMaximum = fileNames.Length;
				foreach (var fileName in fileNames)
				{
					LabelProgress = fileName;
					object[,] dataArr;
					try
					{
						dataArr = GetData(fileName);
						Logger.Info("������ ����� ({0})", fileName);
						ProgressBarSubMaximum = dataArr.GetUpperBound(0);
					}
					catch (Exception e)
					{
						Logger.Error("������ ������ ����� ({0}), {1}", fileName, e.Message);
						ProgressBarValue++;
						continue;
					}

					var curStock = new stock();
					var curDate = new DateTime();
					var curRow = Config.Inst.Imports.OrderStocksAndTraffic.FirstRow;
					// �������� �������� ������
					try
					{
						if (dataArr[Config.Inst.Imports.OrderStocksAndTraffic.RowSign, Config.Inst.Imports.OrderStocksAndTraffic.ColSign].ToString()
							!=
							Config.Inst.Imports.OrderStocksAndTraffic.Sign)
						{
							throw new Exception($"������ �������� �������� {dataArr[Config.Inst.Imports.OrderStocksAndTraffic.RowSign, Config.Inst.Imports.OrderStocksAndTraffic.ColSign]}");
						}
					}
					catch (Exception e)
					{
						Logger.Error("��������� ����� ({0}) �� ��������� � ���������� ({1}) {2}", fileName, Config.Inst.Imports.OrderStocksAndTraffic.Sign, e.Message);
						ProgressBarValue++;
						continue;
					}

					var context = IoC.Get<OrderAssistantEntities>();

					context.Configuration.LazyLoadingEnabled = false;
					context.Configuration.AutoDetectChangesEnabled = false;

					// �������� ���������
					try
					{
						context.items.Load();
						context.brands.Load();
						context.manufacturers.Load();
						context.balances.Load();
						context.stocks.Load();
					}
					catch (Exception e)
					{
						Logger.Error("������ �������� ��������� �� ��. {0}", e.Message);
						return;
					}

					while (curRow <= dataArr.GetUpperBound(0) - Config.Inst.Imports.OrderStocksAndTraffic.LastRowCorrection)
					{
						cancelToken.ThrowIfCancellationRequested();

						ProgressBarSubValue = curRow;
						LabelProgressSub = curRow.ToString();
						//Refresh();

						// Date
						if (dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColDate] != null)
						{
							try
							{
								curDate = DateTime.Parse(dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColDate].ToString());
								curRow++;
								continue;
							}
							catch (FormatException e)
							{
								Logger.Error("���� �� �������� �����. ������ {0}. {1}", curRow, e.Message);
								curRow++;
								continue;
							}
						}
						// Stock
						if (dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColStock] != null)
						{
							try
							{
								curStock = TakeStock(dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColStock].ToString(), context);
								curRow++;
								continue;
							}
							catch (Exception e)
							{
								Logger.Error("����� �� ������ � ��. ������ {0}. {1}", curRow, e.Message);
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
							catNumber = dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColCatNumber].ToString();
							name = dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColName].ToString();
							count = Convert.ToDecimal(dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColCount]);
							id1C = dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.Col1CId].ToString();
							manufacturerStr = dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColManufacturer].ToString();
							brendStr = dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColBrend].ToString();
							cost = Convert.ToDecimal(dataArr[curRow, Config.Inst.Imports.OrderStocksAndTraffic.ColCost]);
						}
						catch (Exception e)
						{
							Logger.Warn("������ ����������� ������ � ������ {0}. {1}", curRow, e.Message);
							curRow++;
							continue;
						}

						// �������� �� ������������� �������
						if (cost <= 0 || count <= 0)
						{
							Logger.Warn("������������� ��� ���������� ������ ��� ����� ����. ������ {0}", curRow);
							curRow++;
							continue;
						}

						// �������� �� ������ ������
						if (string.IsNullOrEmpty(catNumber) ||
							string.IsNullOrEmpty(name) ||
							string.IsNullOrEmpty(id1C) ||
							string.IsNullOrEmpty(manufacturerStr) ||
							string.IsNullOrEmpty(brendStr))
						{
							Logger.Warn("������ {0} �������� ������ ���������", curRow);
							curRow++;
							continue;
						}

						var item = GetItem(name, id1C, manufacturerStr, brendStr, catNumber, context);
						item.balances.Add(GetBalance(curDate, curStock, cost, item, count, context));

						if (curRow % Config.Inst.Imports.LoadAfter == 0)
						{
							context.SaveChanges();
						}
						curRow++;
					}
					context.SaveChanges();
					Logger.Info("������� �������� ������ ����� ({0}), ���������� ����� {1}", fileName, curRow);
					ProgressBarValue++;
				}
			}, cancelToken);
		}

		/// <summary>
		/// ���������� ������ � ������� ��� �������
		/// </summary>
		/// <param name="fileName">���� � �����</param>
		/// <returns></returns>
		private static object[,] GetData(string fileName)
		{
			object[,] dataArr;
			//TODO ����� ������ ���� ������ MXL
			//if (System.IO.Path.GetExtension(fileName).ToLower() == ".mxl")
			//{
			//	var curWb  = new SpreadsheetDocument();
			//	curWb.Open(fileName);


			//	var a = curWb.Area(1,1,10,10).Value;
			//	return null;
			//}
			//else
			//{
			var curWb = new Microsoft.Office.Interop.Excel.Application().Workbooks.Open(fileName);
			dynamic curSheet = curWb.Worksheets.Item[1];
			var lastRow = curSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Row;
			var lastCol = curSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Column;

			// ��������� ���� � ������
			var range = (Microsoft.Office.Interop.Excel.Range)curSheet.Range[curSheet.Cells[1, 1], curSheet.Cells[lastRow, lastCol]];
			dataArr = (object[,])range.Value;
			curWb.Close();
			//}
			if (dataArr == null)
			{
				throw new Exception("���� �� �������� ������.");
			}
			return dataArr;
		}

		/// <summary>
		/// ���������� ������ �� ����� ���� �������� ���� ���������, ���� ��� ����� ��������.
		/// </summary>
		/// <param name="nameStr">��������</param>
		/// <param name="context"></param>
		/// <returns>������ �� �����</returns>
		private static brand GetBrand(string nameStr, OrderAssistantEntities context)
		{
			// ���������, ���� �� ����� �����
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
		/// ���������� ������ �� ������������� ���� ��������� ���� ����������, ���� ��� ����� ��������.
		/// </summary>
		/// <param name="nameStr">�������� �������������</param>
		/// <param name="context"></param>
		/// <returns>������ �� �������������</returns>
		private static manufacturer GetManufacturer(string nameStr, OrderAssistantEntities context)
		{
			// ���������, ���� �� ����� �����
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
		/// ���������� ������ �� item ���� ��������� ���� ���������
		/// </summary>
		/// <param name="name">�������� ��</param>
		/// <param name="id1C">��� 1�</param>
		/// <param name="manufacturerStr">������������� ��������</param>
		/// <param name="brendStr">�������� ������</param>
		/// <param name="catNumber">�������</param>
		/// <param name="context"></param>
		/// <returns>������ �� item</returns>
		private static item GetItem(string name, string id1C, string manufacturerStr, string brendStr, string catNumber,
			OrderAssistantEntities context)
		{
			// ��������� ���� �� ����� item
			var item = (from i in context.items.Local
						where i.id1C == id1C
						select i).FirstOrDefault();
			// ���� ������ item ���, �������
			if (item == null)
			{
				var newItem = new item()
				{
					id1C = id1C,
					manufacturer = GetManufacturer(manufacturerStr, context),
					brand = GetBrand(brendStr, context),
					catNumber = catNumber,
					name = name,
					ABCgroup = "D" //TODO ������ ���� � ���� �������������. �� �������� �� �����
				};
				item = context.items.Add(newItem);
				return item;
			}
			// ����� ���������
			item.manufacturer = GetManufacturer(manufacturerStr, context);
			item.brand = GetBrand(brendStr, context);
			item.catNumber = catNumber;
			item.name = name;
			return item;
		}

		/// <summary>
		/// ���������� ������ �� balance ���� ��������� ���� ���������
		/// </summary>
		/// <param name="date">���� �������</param>
		/// <param name="stock">�����</param>
		/// <param name="cost">�������������</param>
		/// <param name="item">��������</param>
		/// <param name="count">���������� �� ������</param>
		/// <param name="context"></param>
		/// <returns></returns>
		private static balance GetBalance(DateTime date, stock stock, decimal cost, item item, decimal count,
			OrderAssistantEntities context)
		{
			// ��������� ���� ����� ������ ��� ���
			var balance = (from b in context.balances.Local
						   where b.stock.id == stock.id && b.dateCount == date && b.item.id1C == item.id1C
						   select b).FirstOrDefault();
			// ���� ��� �������
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
			// ��� ���������
			else
			{
				balance.cost = cost;
				balance.count = count;
			}
			return balance;
		}
		/// <summary>
		/// ���������� Stock ������� �������� ��� ������������ ���������, ��� ����� ��������
		/// </summary>
		/// <param name="stockStr">������ � ��������� �������</param>
		/// <param name="context"></param>
		/// <returns></returns>
		private static stock TakeStock(string stockStr, OrderAssistantEntities context)
		{
			if (String.IsNullOrEmpty(stockStr))
			{
				throw new Exception("������ ����� ��� NULL.");
			}
			return Enumerable.FirstOrDefault(context.stocks, stock => stockStr.ToLower().Contains(stock.signature));
		}

		public void ButtonCancelImport()
		{
			_cancelToken.Cancel();
		}

		private bool _buttonStartImportIsEnabled = true;
		public bool ButtonStartImportIsEnabled
		{
			get => _buttonStartImportIsEnabled;
			set
			{
				_buttonStartImportIsEnabled = value;
				NotifyOfPropertyChange(() => ButtonStartImportIsEnabled);
			}
		}

		private string _labelProgress;
		public string LabelProgress
		{
			get => _labelProgress;
			set
			{
				_labelProgress = value;
				NotifyOfPropertyChange(() => LabelProgress);
			}
		}

		private string _labelProgressSub;
		public string LabelProgressSub
		{
			get => _labelProgressSub;
			set
			{
				_labelProgressSub = value;
				NotifyOfPropertyChange(() => LabelProgressSub);
			}
		}

		private int _progressBarMaximum;
		public int ProgressBarMaximum
		{
			get => _progressBarMaximum;
			set
			{
				_progressBarMaximum = value;
				NotifyOfPropertyChange(() => ProgressBarMaximum);
			}
		}

		private int _progressBarSubMaximum;
		public int ProgressBarSubMaximum
		{
			get => _progressBarSubMaximum;
			set
			{
				_progressBarSubMaximum = value;
				NotifyOfPropertyChange(() => ProgressBarSubMaximum);
			}
		}

		private int _progressBarValue;
		public int ProgressBarValue
		{
			get => _progressBarValue;
			set
			{
				_progressBarValue = value;
				NotifyOfPropertyChange(() => ProgressBarValue);
			}
		}

		private int _progressBarSubValue;
		public int ProgressBarSubValue
		{
			get => _progressBarSubValue;
			set
			{
				_progressBarSubValue = value;
				NotifyOfPropertyChange(() => ProgressBarSubValue);
			}
		}

		private string _textBlockConfig;
		public string TextBlockConfig
		{
			get => _textBlockConfig;
			set
			{
				var serializer = new XmlSerializer(typeof(OrderStocksAndTraffic));
				StringWriter textWriter = new StringWriter();
				serializer.Serialize(textWriter, Config.Inst.Imports.OrderStocksAndTraffic);

				_textBlockConfig = textWriter.ToString(); textWriter.Close();
				NotifyOfPropertyChange(() => TextBlockConfig);
			}
		}

		private string _textBlockTimer;
		public string TextBlockTimer
		{
			get => _textBlockTimer;
			set
			{
				_textBlockTimer = value;
				NotifyOfPropertyChange(() => TextBlockTimer);
			}
		}

		private string _textBlockLog;
		public string TextBlockLog
		{
			get => _textBlockLog;
			set
			{
				_textBlockLog = _textBlockLog + value + "\r\n";
				NotifyOfPropertyChange(() => TextBlockLog);
			}
		}

		public void SetDefault()
		{
			LabelProgress = "";
			LabelProgressSub = "";
			ButtonStartImportIsEnabled = true;
			ProgressBarMaximum = 100;
			ProgressBarSubMaximum = 100;
			ProgressBarValue = 0;
			ProgressBarSubValue = 0;
			TextBlockTimer = "";
			_cancelToken = new CancellationTokenSource();
		}
	}
}





