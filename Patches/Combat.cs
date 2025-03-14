using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheJazMaster.Louis.Artifacts;

namespace TheJazMaster.Louis;

internal class CombatPatches
{

	private static ModEntry Instance => ModEntry.Instance;

	internal static void ApplyPatches(Harmony harmony)
	{
		harmony.TryPatch(
			logger: Instance.Logger!,
			original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
			transpiler: new HarmonyMethod(typeof(CombatPatches), nameof(Combat_TryPlayCard_Transpiler))
		);
		harmony.TryPatch(
            logger: Instance.Logger!,
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.SendCardToExhaust)),
            prefix: new HarmonyMethod(typeof(CombatPatches), nameof(Combat_SendCardToExhaust_Postfix))
        );
	}

	private static void Combat_SendCardToExhaust_Postfix(Combat __instance, State s, Card card) {
		foreach (Artifact item in s.EnumerateAllArtifacts()) {
            if (item is IOnExhaustArtifact artifact)  {
                artifact.OnExhaustCard(s, __instance, card);
            }
        }
	}

	private static IEnumerable<CodeInstruction> Combat_TryPlayCard_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(
				ILMatches.Ldloc<CardData>(originalMethod).CreateLdlocInstruction(out var ldData),
				ILMatches.Ldfld("singleUse"),
				ILMatches.Brfalse,
				ILMatches.Ldloc(0),
				ILMatches.Ldfld("card"),
				ILMatches.Call("ExhaustFX")
			);
		return new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(
				ILMatches.Ldloc<bool>(originalMethod).CreateLdlocInstruction(out var ldLoc).Anchor(out var anchor),
				ILMatches.Brfalse,
				ILMatches.Ldloc(0),
				ILMatches.Ldfld("card"),
				ILMatches.Call("ExhaustFX")
			)
			.Anchors()
			.PointerMatcher(anchor)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
				new CodeInstruction(OpCodes.Ldarg_1),
				ldData,
				ldLoc,
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ApplyRose))),
				new CodeInstruction(OpCodes.And)
			)
			.AllElements();
	}

	private static bool ApplyRose(State state, CardData data, bool exhausts) {
		if (exhausts && !data.temporary) {
			foreach (Artifact item in state.EnumerateAllArtifacts()) {
				if (item is BlackRoseArtifact artifact && artifact.active) {
					artifact.active = false;
					artifact.Pulse();
					return false;
				}
			}
		}
		return true;
	}
}