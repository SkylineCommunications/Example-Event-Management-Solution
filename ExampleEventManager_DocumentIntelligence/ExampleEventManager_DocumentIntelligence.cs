/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

24/02/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

using ExampleEventManager_DocumentIntelligence;
using Newtonsoft.Json;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.Apps.DocumentIntelligence;
using Skyline.DataMiner.Net.Apps.DocumentIntelligence.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ExampleEventManagerDocumentIntelligence
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
            var filePath = engine.GetScriptParam("filePath").Value.Trim('[',']','"');

            engine.Log("FILEPATH: " + filePath);

            // Read file content
            //var filePath = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\Example Event Management\EventRequestForm_ExampleEvent.docx";
            var fileBytes = File.ReadAllBytes(filePath);

            // Write instructions
            // See DocumentInstruction.cs for more details on the instructions.

            // Create Document Intelligence helper
            var docIntelHelper = new DocumentIntelligenceHelper(engine.SendSLNetMessages);
            // Request Document Intelligence analysis
            var analysisResult = docIntelHelper.AnalyzeDocuments(DocumentInstruction.Instruction, new List<Document>()
            {
                new Document()
                {
                    Name = Path.GetFileName(filePath),
                    Content = fileBytes
                }
            });

            engine.Log("RESULT OF DOC ANALYSIS: " + analysisResult);

            var prompResult = JsonConvert.DeserializeObject<PromptResult>(analysisResult);

            engine.AddOrUpdateScriptOutput("PROMPTRESULT", prompResult.Prompt);
        }
    }

    public class PromptResult
    {
        public string Prompt { get; set; }
    }
}
