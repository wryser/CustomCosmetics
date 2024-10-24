using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class HoldableDescriptor : MonoBehaviour
{
	// Token: 0x0400000C RID: 12
	public string Name = "";

	// Token: 0x0400000D RID: 13
	public string Author = "";

	// Token: 0x0400000E RID: 14
	public string Description = "";

	// Token: 0x0400000F RID: 15
	public GameObject leftHandObject;

	// Token: 0x04000010 RID: 16
	public GameObject rightHandObject;

	// Token: 0x04000011 RID: 17
	public bool customColours = false;

	// Token: 0x04000012 RID: 18
	public string id;

	// Token: 0x04000013 RID: 19
	public List<CosmeticBehaviour> behaviours = new List<CosmeticBehaviour>();

	// Token: 0x04000014 RID: 20
	public bool gunEnabled = false;

	// Token: 0x04000015 RID: 21
	public bool audioMode;

	// Token: 0x04000016 RID: 22
	public AudioClip shootSound;

	// Token: 0x04000017 RID: 23
	public float bulletSpeed;

	// Token: 0x04000018 RID: 24
	public float bulletCooldown;

	// Token: 0x04000019 RID: 25
	public GameObject bulletObject;

	// Token: 0x0400001A RID: 26
	public bool vibra;

	// Token: 0x0400001B RID: 27
	public float strenth = 0.25f;

	// Token: 0x0400001C RID: 28
	public float sTime = 0.5f;

	// Token: 0x0400001D RID: 29
	public float bulletMultiply;
}
