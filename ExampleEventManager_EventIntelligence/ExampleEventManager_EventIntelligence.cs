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
    using Newtonsoft.Json;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Learning.EventManagement.ApiHelpers;
    using Skyline.DataMiner.Learning.EventManagement.Models;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DocumentIntelligence;
    using Skyline.DataMiner.Net.Apps.DocumentIntelligence.Objects;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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


            engine.Log("INSTRUCTIONS RECEIVED FROM DOCUMENT INTELLIGENCE:\r\n" + analysisResultInstructions);
            var resultInstructions = JsonConvert.DeserializeObject<ResultInstructions>(analysisResultInstructions);

            switch (resultInstructions.HTTPVERB)
            {
                case "GET":
                    engine.AddOrUpdateScriptOutput("ODATAFILTER", resultInstructions.ODATAFILTER);
                    engine.Log("ODATAFILTER: " + resultInstructions.ODATAFILTER);
                    break;
                case "POST":
                    if (resultInstructions.HTTPBODY.StartsWith("["))
                    {
                        eventHelper.Events.CreateOrUpdate(JsonConvert.DeserializeObject<List<Event>>(resultInstructions.HTTPBODY));
                    }
                    else
                    {
                        eventHelper.Events.Create(JsonConvert.DeserializeObject<Event>(resultInstructions.HTTPBODY));
                    }
                    break;
                case "PUT":
                    if (resultInstructions.HTTPBODY.StartsWith("["))
                    {
                        eventHelper.Events.CreateOrUpdate(JsonConvert.DeserializeObject<List<Event>>(resultInstructions.HTTPBODY));
                    }
                    else
                    {
                        eventHelper.Events.Update(JsonConvert.DeserializeObject<Event>(resultInstructions.HTTPBODY));
                    }
                    break;
                case "DELETE":
                    eventHelper.Events.Delete(eventHelper.Events.Read(EventExposers.Identifier.Equal(resultInstructions.MODELID)));
                    break;
            }
        }
	}

    /// <summary>
    /// Represents instructions for processing a result, including OData filter, HTTP verb, HTTP body, and model
    /// identifier.
    /// </summary>
    public class ResultInstructions
    {
        /// <summary>
        /// Gets or sets the OData filter expression used to query data.
        /// </summary>
        public string ODATAFILTER { get; set; }

        /// <summary>
        /// Gets or sets the HTTP verb used for the request.
        /// </summary>
        public string HTTPVERB { get; set; }

        /// <summary>
        /// Gets or sets the HTTP body content, which may contain data for creating or updating events, or other relevant information based on the specified HTTP verb.
        /// </summary>
        public string HTTPBODY { get; set; }

        /// <summary>
        /// Gets or sets the model identifier, which can be used to specify the target event for delete operations, or other relevant information based on the specified HTTP verb.
        /// </summary>
        public string MODELID { get; set; }
    }
}
