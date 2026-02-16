/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

16/02/2026	1.0.0.1		TVD, Skyline	Initial version
****************************************************************************
*/

namespace ExampleEventManagerUDAPI
{
    using ExampleEventManager_UDAPI;
    using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.UserDefinableApis.Actions;
    using Skyline.DataMiner.SDM.UserDefinedApi;

    /// <summary>
    /// Represents a DataMiner user-defined API.
    /// </summary>
    public class Script
	{
        private static IUserDefinedApi _api;

        /// <summary>
        /// The API trigger.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        /// <param name="requestData">Holds the API request data.</param>
        /// <returns>An object with the script API output data.</returns>
        [AutomationEntryPoint(AutomationEntryPointType.Types.OnApiTrigger)]
        public ApiTriggerOutput OnApiTrigger(IEngine engine, ApiTriggerInput requestData)
        {
            if (_api is null)
            {
                _api = UserDefinedApi.CreateBuilder()
                    .AddControllers()
                    .AddServices()
                    .Build();
            }

            return _api.Run(engine, requestData);
        }
    }
}
