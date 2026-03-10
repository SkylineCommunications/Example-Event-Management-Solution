/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

04/03/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

namespace ExampleEventManager_ConfigureUDAPIToken
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.UserDefinableApis;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string API_TOKEN_NAME = "eventManagerToken";
		private const string API_ROUTE_PATH = "eventmanager/events";
		private const string EVENT_UDAPI_AUTOMATION_SCRIPT_NAME = "ExampleEventManager_UDAPI";

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
			var helper = new UserDefinableApiHelper(engine.SendSLNetMessages);

			var allEventMangerEventRoutes = helper.ApiDefinitions.Read(ApiDefinitionExposers.Route.Equal(API_ROUTE_PATH));
			if (allEventMangerEventRoutes.Count == 0)
			{
				engine.ExitFail($"No route found with path '{API_ROUTE_PATH}'. Please check if the UDAPI is correctly configured on script {EVENT_UDAPI_AUTOMATION_SCRIPT_NAME} .");
				return;
			}

			var eventAPIDefinition = allEventMangerEventRoutes.FirstOrDefault();

			// check if token with name "eventmanagerToken" already exists
			var allTokens = helper.ApiTokens.Read(ApiTokenExposers.Name.Equal(API_TOKEN_NAME));
			if (allTokens.Count > 0)
			{
				engine.ExitFail($"A token with the name '{API_TOKEN_NAME}' already exists. Please create the token manually");
				return;
			}

			// create a new token with name "eventManagerToken"
			// Create the token
			var secret = ApiTokenSecretGenerator.GenerateSecret();
			var token = new ApiToken()
			{
				Name = API_TOKEN_NAME,
				Secret = secret
			};
			token = helper.ApiTokens.Create(token);

			// Update the API definition to use the created token
			eventAPIDefinition.SecuritySettings.AllowedTokens.Add(token.ID);
			helper.ApiDefinitions.Update(eventAPIDefinition);

			// Show the token in a dialog so the user can copy it, as it will not be shown again for security reasons
			var controller = new InteractiveController(engine);
			var dialog = new TokenDialog(engine, secret);
			dialog.CloseButton.Pressed += (s, e) =>
			{
				engine.ExitSuccess("Secret provided");
			};

			controller.ShowDialog(dialog);
		}
	}
}
