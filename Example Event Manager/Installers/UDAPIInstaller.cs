using Newtonsoft.Json.Linq;
using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Net.AppPackages;
using Skyline.DataMiner.Net.Apps.UserDefinableApis;
using Skyline.DataMiner.Net.Apps.UserDefinableApis.Actions;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_Event_Manager.Installers
{
    public class UDAPIInstaller
    {
        private IConnection _connection;

        private const string API_ROUTE_PATH = "eventmanager/events";
        private const string EVENT_UDAPI_AUTOMATION_SCRIPT_NAME = "ExampleEventManager_UDAPI";

        public UDAPIInstaller(IConnection connection, AppInstallContext context)
        {
            _connection = connection;
        }

        public void InstallDefaultContent()
        {
            // Setup the helper
            var helper = new UserDefinableApiHelper(_connection.HandleMessages);

            // Check if the api route already exists to avoid duplicate installation
            var allEventMangerEventRoutes = helper.ApiDefinitions.Read(ApiDefinitionExposers.Route.Equal(API_ROUTE_PATH));

            if (allEventMangerEventRoutes.Count > 0)
            {
                Logger.Log($"A  router with path '{API_ROUTE_PATH}' already exists, not configuring anymore.");
                return;
            }

            // Define the API
            var definition = new ApiDefinition()
            {
                Name = "Example Event Manager API",
                Description = "The webapi for the example event manager.",
                Route = API_ROUTE_PATH,
                ActionType = ActionType.AutomationScript,
                ActionMeta = new AutomationScriptActionMeta()
                {
                    InputType = InputType.RawBody,
                    ScriptName = EVENT_UDAPI_AUTOMATION_SCRIPT_NAME
                },
            };

            // Create the definition
            helper.ApiDefinitions.Create(definition);
        }
    }
}
