using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class HatDescriptor : MonoBehaviour
{
	// Token: 0x04000007 RID: 7
	public string Name = "";

	// Token: 0x04000008 RID: 8
	public string Author = "";

	// Token: 0x04000009 RID: 9
	public string Description = "";

	// Token: 0x0400000A RID: 10
	public string id;

	// Token: 0x0400000B RID: 11
	public List<CosmeticBehaviour> behaviours = new List<CosmeticBehaviour>();
}
