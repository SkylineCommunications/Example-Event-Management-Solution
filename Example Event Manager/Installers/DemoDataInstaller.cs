namespace Example_Event_Manager.Installers
{
	using System;

	using Skyline.AppInstaller;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.AppPackages;

	internal class DemoDataInstaller
	{
		private readonly IEngine _engine;

		public DemoDataInstaller(IEngine engine, AppInstallContext context)
		{
			_engine = engine;
		}

		public void InstallDefaultContent()
		{
			// Custom installation logic can be added here for each individual install package.
			Logger.Log("Starting installation of demo data");
			try
			{
				var subScript = _engine.PrepareSubScript("ExampleEventManager_InstallDemoData");
				subScript.StartScript();
				Logger.Log("Demo data installation completed successfully");
			}
			catch (Exception ex)
			{
				Logger.Log("Failed to install demo data: " + ex.Message);
			}
		}
	}
}
