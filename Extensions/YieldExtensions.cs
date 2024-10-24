using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCosmetics.Extensions
{
	// Token: 0x02000022 RID: 34
	public static class YieldExtensions
	{
		// Token: 0x0600007A RID: 122 RVA: 0x000078C0 File Offset: 0x00005AC0
		public static async Task Yield(YieldInstruction instruction)
		{
			TaskCompletionSource<YieldInstruction> completionSource = new TaskCompletionSource<YieldInstruction>();
			Plugin.instance.StartCoroutine(YieldExtensions.AwaitInstructionCorouutine(instruction, completionSource));
			await completionSource.Task;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00007904 File Offset: 0x00005B04
		private static IEnumerator AwaitInstructionCorouutine(YieldInstruction instruction, TaskCompletionSource<YieldInstruction> completionSource)
		{
			yield return instruction;
			completionSource.SetResult(instruction);
			yield break;
		}
	}
}
