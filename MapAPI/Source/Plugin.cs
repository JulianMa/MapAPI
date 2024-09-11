using Eco.Core;
using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Aliases;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Property;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.WorldGenerator;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Eco.Plugins.MapAPI
{
    [Priority(PriorityAttribute.High)] // Need to start before WorldGenerator in order to listen for world generation finished event
    public class MapAPI : IModKitPlugin, IInitializablePlugin
    {
        public string GetStatus()
        {
            return "OK";
        }
        public string GetCategory()
        {
            return "API";
        }
        public void Initialize(TimedTask timer)
        {
            PluginManager.Controller.RunIfOrWhenInited(OnWorldGenDone);
        }

        private void OnWorldGenDone()
        {
            // Log.WriteLine(Localizer.DoStr("Counting Blocks..."));
            // var ironOreBlocks = 0;
            //
            // void GenerateThread()
            // {
            //     Thread.CurrentThread.Name = "GenerateThread";
            //     VoronoiWorldGeneratorConfig config = WorldGeneratorPlugin.Settings.VoronoiWorldGeneratorConfig;
            //     config.Seed = Random.Shared.Next();
            //     
            //     using (new TimedTask(Localizer.DoStr("Generating terrain preview")))
            //     {
            //             config.Reset();
            //             var generator = config.GenerateWorld(skipSetSpawnLocation: true);
            //             File.Move("Biomes.png", "/app/WebClient/WebBin/Layers/Biomes_new.png");
            //             Log.WriteLine(Localizer.DoStr("GenWorld DONE!"));
            //     }
            // }
            //
            // var threadGenerate = new Thread(GenerateThread);
            // threadGenerate.Start();
            // threadGenerate.Join();
            
            
            
            // var chunks = EW.ChunksInRange(new WorldRange(520));
            //
            // for (int x = 0; x < Eco.World.World.WrappedChunkSize.x; x++)
            // {
            //     for (int y = 0; y < Shared.Voxel.World.WrappedChunkSize.y; y++)
            //     {
            //         for (int z = 0; z < Shared.Voxel.World.WrappedChunkSize.x; z++)
            //         {
            //             var chunk = World.World.GetChunk(new Vector3i(x, y, z));
            //             if (chunk == null)
            //             {
            //                 Log.WriteLine(Localizer.DoStr("No Chunk Found."));
            //             }
            //             else
            //             {
            //                 if (chunk.Blocks == null)
            //                 {
            //                     Log.WriteLine(Localizer.DoStr("No Blocks."));
            //                 }
            //                 else
            //                 {
            //                     ironOreBlocks += chunk.Blocks.Where(block => block != null).OfType<IronOreBlock>().Count();
            //                 }
            //             }
            //            
            //         }
            //     }
            // }
            

            // if (World.World.Chunks == null)
            // {
            //     Log.WriteLine(Localizer.DoStr("No World Chunks."));
            // }
            // else
            // {
            //     foreach(PersistentChunk pc in World.World.Chunks)
            //     {
            //         if (pc == null)
            //         {
            //             Log.WriteLine(Localizer.DoStr("No Chunk."));
            //         }
            //         else
            //         {
            //             if (pc.Blocks == null)
            //             {
            //                 Log.WriteLine(Localizer.DoStr("No Blocks."));
            //             }
            //             else
            //             {
            //                 ironOreBlocks += pc.Blocks.Where(block => block != null).OfType<IronOreBlock>().Count();
            //             }
            //             
            //         }
            //        
            //
            //     }
            // }
           
            
            // for (int x = 0; x < Shared.Voxel.World.VoxelSize.x; x++)
            // {
            //     for (int y = 0; y < Shared.Voxel.World.VoxelSize.y; y++)
            //     {
            //         for (int z = 0; z < Shared.Voxel.World.VoxelSize.x; z++)
            //         {
            //             Block block = World.World.GetBlock(new Vector3i(x, y, z));
            //             if (block is IronOreBlock)
            //             {
            //                 ironOreBlocks++;
            //             }
            //         }
            //     }
            // }
            //Log.WriteLine(Localizer.DoStr($"MapGenDone: {ironOreBlocks}"));
            //PluginManager.Controller.FireShutdown(ApplicationExitCodes.NormalShutdown);
        }
    }
}
