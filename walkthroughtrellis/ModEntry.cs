using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace WalkThroughTrellis
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += OnWarped;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            /*Telling it for every HoeDirt type that is dirt in the locations gathered (Thanks PathosChild <3)
            with the given parameters, make the raised seed setting for the crops false. 
            */
            foreach (HoeDirt dirt in Game1.currentLocation.terrainFeatures.Values.OfType<HoeDirt>().Where(dirt => dirt.crop != null))
                dirt.crop.raisedSeeds.Value = false;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // tell SMAPI we can edit the crops XNB asset
            return asset.AssetNameEquals(@"Data\Crops");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // tell SMAPI what we want to change in the crops XNB asset
            var data = asset.AsDictionary<int, string>().Data;
            foreach (int id in data.Keys.ToArray())
            {
                string[] fields = data[id].Split('/'); // split fields in the crops XNB via the '/' which is how they're separated in the xnb
                fields[7] = "false"; // set 'raised seeds' field to false (This is for all crops, maybe in the future I'll be more explicit)
                data[id] = string.Join("/", fields); // stitch fields back into the XNB file's format
            }
        }
    }
}
