/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

16/02/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

namespace ExampleEventManagerEventIntelligence
{
	using System;
	using System.Collections.Generic;
    using System.Text;
	using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.SLSearch.Misc;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Utils.Examples.EventManager.Models;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Apps.DocumentIntelligence;
    using Skyline.DataMiner.Utils.Examples.EventManager.ApiHelpers;
    using Newtonsoft.Json;

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
            var modelIdentifiers = engine.GetScriptParam("ModelIdentifiers").Value.Trim('[', ']').Trim('"')?.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            string prompt = engine.GetScriptParam("Prompt").Value.Trim('[', ']').Trim('"');


            var eventHelper = new EventApiHelper(engine.GetUserConnection());

            var allEvents = Tools.RetrieveBigOrFilter(
                modelIdentifiers,
                id => EventExposers.Identifier.Equal(id),
                filter => eventHelper.Events.Read(filter).ToList());


            engine.Log("ModelIdentifiers: " + String.Join(",", modelIdentifiers));
            engine.Log("Prompt: " + prompt);
            engine.Log(JsonConvert.SerializeObject(allEvents));

            string html = "<html>\r\n<prompt>" + prompt + "</prompt>\r\n<models>" + JsonConvert.SerializeObject(allEvents) + "</models>\r\n</html>";
            byte[] fileBytes = Encoding.UTF8.GetBytes(html);

            var docIntelHelper = new DocumentIntelligenceHelper(engine.SendSLNetMessages);

            engine.Log("DOCUMENT THAT WILL BE ANALYZED:\r\n" + html);

            // Request Document Intelligence analysis
            var analysisResultInstructions = docIntelHelper.AnalyzeDocuments(IntelligenceInstruction.Instruction, new List<Document>()
            {
                new Document()
                {
                    Name = "whatToDo.html",
                    Content = fileBytes,
                },
            });

            var resultInstructions = JsonConvert.DeserializeObject<ResultInstructions>(analysisResultInstructions);

            engine.Log("INSTRUCTIONS RECEIVED FROM DOCUMENT INTELLIGENCE:\r\n" + analysisResultInstructions);

            switch (resultInstructions.HTTPVERB)
            {
                case "GET":
                    engine.AddOrUpdateScriptOutput("ODATAFILTER", resultInstructions.ODATAFILTER);
                    engine.Log("ODATAFILTER: " + resultInstructions.ODATAFILTER);
                    break;
                case "POST":
                    eventHelper.Events.CreateOrUpdate(JsonConvert.DeserializeObject<List<Event>>(resultInstructions.HTTPBODY));
                    break;
                case "PUT":
                    eventHelper.Events.CreateOrUpdate(JsonConvert.DeserializeObject<List<Event>>(resultInstructions.HTTPBODY));
                    break;
                case "DELETE":
                    eventHelper.Events.Delete(eventHelper.Events.Read(EventExposers.Identifier.Equal(resultInstructions.MODELID)));
                    break;
            }
        }
	}

    public class ResultInstructions
    {
        public string ODATAFILTER { get; set; }
        public string HTTPVERB { get; set; }
        public string HTTPBODY { get; set; }
        public string MODELID { get; set; }
    }
}
