using Eco.Core;
using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Aliases;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Property;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Simulation.Agents;
using Eco.WorldGenerator;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Eco.Plugins.MapAPI
{
    [Priority(PriorityAttribute.High)] // Need to start before WorldGenerator in order to listen for world generation finished event
    public class MapAPI : IModKitPlugin, IInitializablePlugin, IShutdownablePlugin
    {
        private Thread loopThread = null;
        public string GetStatus()
        {
            return "OK";
        }
        public string GetCategory()
        {
            return "API";
        }
        public Task ShutdownAsync()
        {
            if (loopThread != null && loopThread.IsAlive)
            {
                loopThread.Interrupt();
            }
            return Task.CompletedTask;
        }

        public void Initialize(TimedTask timer)
        {
            PluginManager.Controller.RunIfOrWhenInited(OnWorldGenDone);
        }

        private void OnWorldGenDone()
        {
            loopThread = new Thread(RequestLoop);
            loopThread.Start();
        }

        static void RequestLoop()
        {

            MapService mapService = new MapService();
            while (true)
            {
                mapService.Tick().Wait();
            }
        }
    }
}
