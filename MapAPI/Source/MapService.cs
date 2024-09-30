#nullable enable
using Eco.Core.Serialization;
using Eco.Plugins.MapAPI.model;
using Eco.WorldGenerator;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Tar;

namespace Eco.Plugins.MapAPI
{
    public class MapService
    {
        private const string MapServerEndpoint = "https://maps.mightymoose.net";
        private const string DevLocalEndpoint = "http://localhost:8080";
        private const string DevServerEndpoint = "http://192.168.178.100:8080";

        private const string ENDPOINT = DevLocalEndpoint;
        private readonly HttpClient _mapRequester = new();

        private async Task<MapQueueDTO?> GetNextMap()
        {
            try
            {
                HttpResponseMessage response = await _mapRequester.GetAsync(ENDPOINT + "/api/maps/queue/preview/next");
                return response.StatusCode != HttpStatusCode.OK
                    ? null
                    : SerializationUtils.DeserializeJson<MapQueueDTO>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                // noop
            }

            return null;
        }

        async private Task<HttpResponseMessage> PostGeneratedMap(byte[] tarBody, string id)
        {
            HttpContent binaryContent = new ByteArrayContent(tarBody);
            return await _mapRequester.PutAsync(ENDPOINT + "/api/maps/preview/" + id, binaryContent);
        }

        public async Task Tick()
        {
            try
            {
                MapQueueDTO? mapQueueDto = await GetNextMap();
                if (mapQueueDto != null)
                {
                    Console.Write(
                        $"Took Generation Task: Size {mapQueueDto.mapSize} with Seed {mapQueueDto.seed} and id '{mapQueueDto.id}'...");
                    var mapSize = mapQueueDto.mapSize * 10;
                    if (mapSize > 2000)
                    {
                        return;
                    }

                    GeneratedMapPreview generatedMapPreview = GenerateMapPreview(mapQueueDto.seed, mapSize);
                    byte[] postDat = CreateTarStream(generatedMapPreview);

                    Console.WriteLine($"Created Tar Archive! Size: {postDat.Length}");
                    await PostGeneratedMap(postDat, mapQueueDto.id);
                }
                else
                {
                    Console.WriteLine("Map queue is empty. Sleeping...");
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private byte[] CreateTarStream(GeneratedMapPreview generatedMapPreview)
        {
            using var terrainMapStream = new MemoryStream();
            using var heightMapStream = new MemoryStream();
            using var waterLevelMapStream = new MemoryStream();
            
            generatedMapPreview.terrainMap.Save(terrainMapStream, ImageFormat.Png);
            generatedMapPreview.heightMap.Save(heightMapStream, ImageFormat.Png);
            generatedMapPreview.waterLevelMap.Save(waterLevelMapStream, ImageFormat.Png);
            
            terrainMapStream.Flush();
            heightMapStream.Flush();
            waterLevelMapStream.Flush();
            
            using (var output = new MemoryStream())
            {
                var tar = new TarOutputStream(output); // This obsolete method works. Can't be bothered to investigate further.

                var tarArchive = TarArchive.CreateOutputTarArchive(tar);
                
                createTarEntry(tar, "terrain.png", terrainMapStream.ToArray());
                createTarEntry(tar, "height.png", heightMapStream.ToArray());
                createTarEntry(tar, "water.png", waterLevelMapStream.ToArray());
                createTarEntry(tar, "mapData.json", Encoding.UTF8.GetBytes(SerializationUtils.SerializeJson(generatedMapPreview.worldResultVars)));

                tar.IsStreamOwner = false;
                tar.Close();

                return output.ToArray();
            }
        }

        private void createTarEntry(TarOutputStream tar, String name, byte[] fileData)
        {
            var tarEntry = TarEntry.CreateTarEntry(name);
            
            var size = fileData.Length;
            tarEntry.Size = size;

            tar.PutNextEntry(tarEntry);
            tar.Write(fileData, 0, size);
            tar.CloseEntry();
        }

        private GeneratedMapPreview GenerateMapPreview(int mapSeed, int mapSize)
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

            var generatedMap = new GeneratedMapPreview(generator.TerrainMap,
                generator.HeightMap, generator.WaterLevelMap, generator.WorldResultVars);

            return generatedMap;
        }
    }
}