using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Net.AppPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_Event_Manager.Installers
{
    public class DemoDataInstaller
    {
        public DemoDataInstaller(IConnection connection, AppInstallContext context)
        {
        }
        public void InstallDefaultContent(IEngine engine)
        {
            // Custom installation logic can be added here for each individual install package.
            Logger.Log("Starting installation of demo data");
            try
            {
                var subScript = engine.PrepareSubScript("ExampleEventManager_InstallDemoData");
                subScript.StartScript();
                Logger.Log("Demo data installation completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to install demo data: " + ex.Message);
            }
          
        }
    }
}
