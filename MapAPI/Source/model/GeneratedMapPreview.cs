using System.Drawing;
using System.IO;
using Eco.WorldGenerator;

namespace Eco.Plugins.MapAPI.model;

public class GeneratedMapPreview
{
    public Bitmap terrainMap { get; set; }
    public Bitmap heightMap { get; set; }
    public Bitmap waterLevelMap { get; set; }
    
    public WorldResultVars worldResultVars { get; set; }

    public GeneratedMapPreview(Bitmap terrainMap, Bitmap heightMap, Bitmap waterLevelMap, WorldResultVars worldResultVars)
    {
        this.terrainMap = terrainMap;
        this.heightMap = heightMap;
        this.waterLevelMap = waterLevelMap;
        this.worldResultVars = worldResultVars;
    }
}