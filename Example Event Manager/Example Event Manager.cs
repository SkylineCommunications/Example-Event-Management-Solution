/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

16/02/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.AppPackages;
using Skyline.DataMiner.Net.DMSState.Agents;
using Skyline.DataMiner.Utils.Examples.EventManagement.ApiHelpers;
using Skyline.DataMiner.Utils.Examples.EventManagement.Models;
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

            var eventApiHelper = new EventApiHelper(engine.GetUserConnection());

            eventApiHelper.Events.CreateOrUpdate(new List<Event>()
            {
                new Event()
                {
                    Identifier = "46af912d-6534-4450-9447-f764b35977ba",
                    Name = "Cercle vs Standard",
                    Description = "A pro league football match.",
                    Start = new DateTime(2026,03,19,18,0,0),
                    End = new DateTime(2026,03,19,20,0,0),
                    Status = EventStatus.Requested,
                    Type = EventType.Basic,
                    Languages = new List<Language>()
                    {
                        new Language()
                        {
                             AudioType = LanguageAudioType.Surround,
                             CcSupplierCompanyName = "Belgian Cc Supplier",
                             Name = "Dutch"
                        },
                        new Language()
                        {
                             AudioType = LanguageAudioType.Surround,
                             CcSupplierCompanyName = "European Cc Supplier",
                             Name = "Italian"
                        }
                    }

                },
                new Event()
                {
                    Identifier = "9e57f010-0bf0-4bdf-82b6-86ce3aa39134",
                    Name = "Arsenal vs Union",
                    Description = "An UEFA football match.",
                    Start = new DateTime(2026,03,20,18,0,0),
                    End = new DateTime(2026,03,20,20,0,0),
                    Status = EventStatus.Requested,
                    Type = EventType.Advanced,
                    Languages = new List<Language>()
                    {
                        new Language()
                        {
                             AudioType = LanguageAudioType.Surround,
                             CcSupplierCompanyName = "Belgian Cc Supplier",
                             Name = "French"
                        },
                        new Language()
                        {
                             AudioType = LanguageAudioType.Surround,
                             CcSupplierCompanyName = "European Cc Supplier",
                             Name = "English"
                        }
                    }

                },
                new Event()
                {
                    Identifier = "dc279fe3-94c2-4cff-9ac9-04e6fc1f99b7",
                    Name = "Chelsea vs Anderlecht",
                    Description = "A test match between 2 UEFA teams.",
                    Start = new DateTime(2026,03,19,18,0,0),
                    End = new DateTime(2026,03,19,20,0,0),
                    Status = EventStatus.Requested,
                    Type = EventType.Pro,
                    Languages = new List<Language>()
                    {
                        new Language()
                        {
                             AudioType = LanguageAudioType.Surround,
                             CcSupplierCompanyName = "Belgian Cc Supplier",
                             Name = "English"
                        },
                    }

                },
            });
        }
		catch (Exception e)
		{
			engine.ExitFail($"Exception encountered during installation: {e}");
		}
	}
}
