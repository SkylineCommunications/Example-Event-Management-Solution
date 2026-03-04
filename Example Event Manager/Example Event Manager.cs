/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

16/02/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

using Example_Event_Manager.Installers;
using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.AppPackages;
using System;
using System.Collections.Generic;

/// <summary>
/// DataMiner Script Class.
/// </summary>
internal class Script
{
	/// <summary>
	/// The script entry point.
	/// </summary>
	/// <param name="engine">Provides access to the Automation engine.</param>
	/// <param name="context">Provides access to the installation context.</param>
	[AutomationEntryPoint(AutomationEntryPointType.Types.InstallAppPackage)]
	public void Install(IEngine engine, AppInstallContext context)
	{
		try
		{
			engine.Timeout = new TimeSpan(0, 10, 0);
			engine.GenerateInformation("Starting installation");
			var installer = new AppInstaller(Engine.SLNetRaw, context);
			installer.InstallDefaultContent();

            ////string setupContentPath = installer.GetSetupContentDirectory();

            // Custom installation logic can be added here for each individual install package.

            // Installing demo data by running the ExampleEventManager_InstallDemoData script.
           var demoDataInstaller = new DemoDataInstaller(Engine.SLNetRaw, context);
			demoDataInstaller.InstallDefaultContent(engine);

            // Configure UDAPI route
			var udapiInstaller = new UDAPIInstaller(Engine.SLNetRaw, context);
			udapiInstaller.InstallDefaultContent();
        }
        catch (Exception e)
		{
			engine.ExitFail($"Exception encountered during installation: {e}");
		}
	}
}
