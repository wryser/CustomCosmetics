using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCosmetics.Extensions
{
	internal class AssetLoader
	{
		public static async Task<AssetBundle> LoadBundle(string bundle)
		{
			AssetBundleCreateRequest loadBundle = AssetBundle.LoadFromFileAsync(bundle);
			await YieldExtensions.Yield(loadBundle);
			return loadBundle.assetBundle;
		}
	}
}
