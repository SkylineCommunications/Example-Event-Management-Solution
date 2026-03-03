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
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ExampleEventManagerDownloadFile
{
    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
    {
        private InteractiveController controller;

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

            controller = new InteractiveController(engine);

            var dialog = new DownloadButtonDialog(engine);

            dialog.DownloadButton.DownloadStarted += (s, e) => 
            {   engine.Log("Download started");  
                engine.ExitSuccess("File started to download"); 
            };

            controller.ShowDialog(dialog);
        }
    }

    /// <summary>
    /// Represents a dialog that displays a button for downloading a file.
    /// </summary>
    public class DownloadButtonDialog : Dialog
    {
        private DownloadButton downloadButton;

        /// <summary>
        /// Initializes a new instance of the DownloadButtonDialog class, configuring the download button and dialog
        /// title based on the provided engine parameters.
        /// </summary>
        /// <param name="engine">The engine used to retrieve script parameters for configuring the dialog.</param>
        public DownloadButtonDialog(IEngine engine) : base(engine)
        {
            var filePath = engine.GetScriptParam("filePath").Value.Trim('[', ']', '"');

            string fileName = Path.GetFileName(filePath);

            downloadButton = new DownloadButton();
            downloadButton.DownloadedFileName = fileName;
            downloadButton.RemoteFilePath = filePath;
            downloadButton.StartDownloadImmediately = false;
            downloadButton.Style = ButtonStyle.CallToAction;
            downloadButton.Text = "Download";

            Title = "Download file";

            var lableDownloadRequest = new Label("Do you want to download file: " + fileName);

           // AddWidget(lableDownloadRequest, 0, 0);
            AddWidget(downloadButton, 1,0);
        }

        /// <summary>
        /// Gets the download button control.
        /// </summary>
        public DownloadButton DownloadButton { get { return downloadButton; } }
    }
}
