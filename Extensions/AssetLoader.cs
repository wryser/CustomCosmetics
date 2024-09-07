using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CustomCosmetics.Extensions.YieldExtensions;

namespace CustomCosmetics.Extensions
{
    class AssetLoader
    {
        public static async Task<AssetBundle> LoadBundle(string bundle)
        {
            var loadBundle = AssetBundle.LoadFromFileAsync(bundle);
            await Yield(loadBundle);

            AssetBundle _storedBundle = loadBundle.assetBundle;
            return _storedBundle;
        }
    }
}
