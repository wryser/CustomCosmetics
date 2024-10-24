using System;
using UnityEngine;

namespace CustomCosmetics.Extensions
{
	// Token: 0x02000020 RID: 32
	public static class ColorExtensions
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00007580 File Offset: 0x00005780
		public static Color parseColor(string sourceString)
		{
			bool flag = sourceString == null || sourceString == "" || sourceString == "$";
			Color color;
			if (flag)
			{
				color = Color.black;
			}
			else
			{
				string text = sourceString.Replace("(", string.Empty);
				text = text.Replace(")", string.Empty);
				text = text.Replace("RGBA", string.Empty);
				string[] array = text.Split(",", StringSplitOptions.None);
				float num;
				float.TryParse(array[0], out num);
				float num2;
				float.TryParse(array[1], out num2);
				float num3;
				float.TryParse(array[2], out num3);
				Color color2;
				color2.r = num;
				color2.g = num2;
				color2.b = num3;
				color2.a = 1f;
				color = color2;
			}
			return color;
		}
	}
}
