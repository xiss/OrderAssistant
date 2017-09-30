namespace OrderAssistantUI {
	public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
	{
		public void ImportOrderStocksAndTraffic()
		{
			Import.ImportOrderStocksAndTraffic();
		}
	}
	
}