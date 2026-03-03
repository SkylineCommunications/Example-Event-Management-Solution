/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

02/03/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Learning.EventManagement.ApiHelpers;
using Skyline.DataMiner.Learning.EventManagement.Models;
using System;
using System.Collections.Generic;

namespace ExampleEventManagerInstallDemoData
{
    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            try
            {
                RunSafe(engine);
            }
            catch (ScriptAbortException)
            {
                // Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
                throw; // Comment if it should be treated as a normal exit of the script.
            }
            catch (ScriptForceAbortException)
            {
                // Catch forced abort exceptions, caused via external maintenance messages.
                throw;
            }
            catch (ScriptTimeoutException)
            {
                // Catch timeout exceptions for when a script has been running for too long.
                throw;
            }
            catch (InteractiveUserDetachedException)
            {
                // Catch a user detaching from the interactive script by closing the window.
                // Only applicable for interactive scripts, can be removed for non-interactive scripts.
                throw;
            }
            catch (Exception e)
            {
                engine.ExitFail("Run|Something went wrong: " + e);
            }
        }

        private void RunSafe(IEngine engine)
        {
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
    }
}
