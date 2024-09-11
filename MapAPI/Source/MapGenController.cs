using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.WorldGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace Eco.Plugins.MapAPI
{
    [Route("api/v1/plugins/MapAPI")]
    public class MapGenController
    {
        
        [HttpGet("{mapSize}/{mapSeed}")]
        [AllowAnonymous]
        public ActionResult GenerateMap(int mapSize, int mapSeed)
        {

            int calcMapSize = mapSize * 10;
            if (calcMapSize > 3000)
            {
                return new BadRequestResult();
            }
            
            VoronoiWorldGenerator generator = new VoronoiWorldGenerator(false, true);
            
            VoronoiWorldGeneratorConfig config = WorldGeneratorPlugin.Settings.VoronoiWorldGeneratorConfig;
            
            config.Seed = mapSeed;
            config.Reset();
            generator.WorldSize = calcMapSize;
            generator.Generate(config);
            
            var bytes = generator.TerrainPngStream.ToArray();
            generator.TerrainPngStream.Close();
            return new FileContentResult(bytes, new MediaTypeHeaderValue("image/png"));
            
            //return new FileStreamResult(generator.TerrainPngStream, new MediaTypeHeaderValue("image/png"));
        }
    }
    
    
    public class StreamResult : ViewResult
    {
        public MemoryStream Stream { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }

        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.ContentType = ContentType;
            if (ETag != null) context.HttpContext.Response.Headers.Add("ETag", ETag);
            context.HttpContext.Response.Body = Stream;
        }
    }
}
