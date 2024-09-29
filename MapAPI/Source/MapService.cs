#nullable enable
using Eco.Core.Serialization;
using Eco.Plugins.MapAPI.model;
using Eco.Simulation.Agents;
using Eco.WorldGenerator;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Eco.Plugins.MapAPI
{
    public class MapService
    {
        private const string MapServerEndpoint = "https://maps.mightymoose.net";
        private const string DevLocalEndpoint = "http://localhost:8080";
        private const string DevServerEndpoint = "http://192.168.178.100:8080";

        private const string ENDPOINT = MapServerEndpoint;
        private readonly HttpClient _mapRequester = new();

        async private Task<MapQueueDTO?> GetNextMap()
        {

            try
            {
                HttpResponseMessage response = await _mapRequester.GetAsync(ENDPOINT + "/api/maps/queue/preview/next");
                return response.StatusCode != HttpStatusCode.OK ? null : SerializationUtils.DeserializeJson<MapQueueDTO>(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                // noop
            }
            return null;
        }

        async private Task<HttpResponseMessage> PostGeneratedMap(byte[] preview, string id)
        {
            HttpContent binaryContent = new ByteArrayContent(preview);
            return await _mapRequester.PutAsync(ENDPOINT + "/api/maps/preview/" + id, binaryContent);
        }

        async private Task<HttpResponseMessage> PostMapMetadata(WorldResultVars worldResultVars, string id)
        {
            JsonContent jsonContent = JsonContent.Create(worldResultVars);
            return await _mapRequester.PutAsync(ENDPOINT + "/api/maps/metadata/" + id, jsonContent);
        }

        public async Task Tick()
        {
            MapQueueDTO? mapQueueDto = await GetNextMap();
            if (mapQueueDto != null)
            {
                Console.WriteLine($"Took Generation Task: Size {mapQueueDto.mapSize} with Seed {mapQueueDto.seed} and id '{mapQueueDto.id}'");
                var mapSize = mapQueueDto.mapSize * 10;
                if (mapSize > 2000)
                {
                    return;
                }

                byte[] generatedPreviewImage = GenerateMapPreview(mapQueueDto.seed, mapSize, out WorldResultVars worldResultVars);
                var response = await PostGeneratedMap(generatedPreviewImage, mapQueueDto.id);
                
                var resultObject = response.StatusCode != HttpStatusCode.OK ? null : SerializationUtils.DeserializeJson<MapEntity>(await response.Content.ReadAsStringAsync());
                if (resultObject != null)
                {
                    await PostMapMetadata(worldResultVars, resultObject.id);
                }

            }
            else
            {
                Console.WriteLine("Map queue is empty. Sleeping...");
                await Task.Delay(5000);
            }

        }

        private byte[] GenerateMapPreview(int mapSeed, int mapSize, out WorldResultVars worldResultVars)
        {
            if (mapSize > 3000)
            {
                throw new Exception("Map size is too large.");
            }

            VoronoiWorldGenerator generator = new VoronoiWorldGenerator(false, true);

            VoronoiWorldGeneratorConfig config = WorldGeneratorPlugin.Settings.VoronoiWorldGeneratorConfig;
            config.Seed = mapSeed;
            config.Reset();
            generator.WorldSize = mapSize;
            generator.Generate(config);

            var bytes = generator.TerrainPngStream.ToArray();
            generator.TerrainPngStream.Close();
            worldResultVars = generator.WorldResultVars;
            return bytes;
        }
    }
}
