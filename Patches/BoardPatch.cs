using System;
using CustomCosmetics.Networking;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;

namespace ProfilePictures.Patches
{
	[HarmonyPatch(typeof(GorillaPlayerScoreboardLine))]
	[HarmonyPatch("UpdateLine")]
	[HarmonyWrapSafe]
	public class BoardPatch
	{
		private static bool Prefix(GorillaPlayerScoreboardLine __instance)
		{
			bool inRoom = PhotonNetwork.InRoom;
			if (inRoom)
			{
				bool flag = __instance.playerVRRig.Creator.GetPlayerRef().CustomProperties.ContainsKey("CustomMaterial") && __instance.playerVRRig.Creator.GetPlayerRef().CustomProperties["CustomMaterial"].ToString() != "" && NetworkingBehaviours.nameAssetCache.ContainsKey(__instance.playerVRRig.Creator.GetPlayerRef().CustomProperties["CustomMaterial"].ToString());
				if (flag)
				{
					try
					{
						__instance.playerSwatch.material = __instance.playerVRRig.materialsToChangeTo[__instance.playerVRRig.setMatIndex];
						bool flag2 = __instance.playerNameVisible != __instance.playerVRRig.playerNameVisible;
						if (flag2)
						{
							__instance.UpdatePlayerText();
						}
						bool flag3 = __instance.myRecorder == null;
						if (flag3)
						{
							__instance.myRecorder = NetworkSystem.Instance.LocalRecorder;
						}
						bool flag4 = __instance.playerVRRig != null;
						if (flag4)
						{
							bool flag5 = __instance.playerVRRig.remoteUseReplacementVoice || __instance.playerVRRig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE";
							if (flag5)
							{
								bool flag6 = __instance.playerVRRig.SpeakingLoudness > __instance.playerVRRig.replacementVoiceLoudnessThreshold && !__instance.rigContainer.ForceMute && !__instance.rigContainer.Muted;
								if (flag6)
								{
									__instance.speakerIcon.enabled = true;
								}
								else
								{
									__instance.speakerIcon.enabled = false;
								}
							}
							else
							{
								bool flag7 = (__instance.rigContainer.Voice != null && __instance.rigContainer.Voice.IsSpeaking) || (__instance.playerVRRig.rigSerializer.IsLocallyOwned && __instance.myRecorder != null && __instance.myRecorder.IsCurrentlyTransmitting);
								if (flag7)
								{
									__instance.speakerIcon.enabled = true;
								}
								else
								{
									__instance.speakerIcon.enabled = false;
								}
							}
						}
						else
						{
							__instance.speakerIcon.enabled = false;
						}
						bool flag8 = !__instance.isMuteManual;
						if (flag8)
						{
							bool isPlayerAutoMuted = __instance.rigContainer.GetIsPlayerAutoMuted();
							bool flag9 = __instance.muteButton.isAutoOn != isPlayerAutoMuted;
							if (flag9)
							{
								__instance.muteButton.isAutoOn = isPlayerAutoMuted;
								__instance.muteButton.UpdateColor();
							}
						}
						return false;
					}
					catch
					{
						return true;
					}
				}
				try
				{
					__instance.playerSwatch.material = __instance.playerVRRig.scoreboardMaterial;
					__instance.playerSwatch.sprite = null;
					__instance.playerSwatch.overrideSprite = null;
					__instance.playerSwatch.color = __instance.playerVRRig.playerColor;
				}
				catch
				{
				}
				return true;
			}
			else
			{
				return true;
			}
		}
	}
}
