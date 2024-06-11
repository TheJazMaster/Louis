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
	}

	private static IEnumerable<CodeInstruction> Combat_TryPlayCard_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		return new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(
				ILMatches.Ldloc<CardData>(originalMethod),
				ILMatches.Ldfld("exhaust"),
				ILMatches.Ldarg(4),
				ILMatches.Instruction(OpCodes.Or),
				ILMatches.Stloc<bool>(originalMethod).CreateLdlocInstruction(out var ldLoc).CreateStlocInstruction(out var stLoc)
			)
			.PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
				new CodeInstruction(OpCodes.Ldarg_1),
				ldLoc,
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ApplyRose))),
				stLoc
			)
			.AllElements();
	}

	private static bool ApplyRose(State state, bool exhausts) {
		if (exhausts) {
			foreach (Artifact item in state.EnumerateAllArtifacts()) {
				if (item is BlackRoseArtifact artifact && artifact.active) {
					artifact.active = false;
					artifact.Pulse();
					return false;
				}
			}
		}
		return exhausts;
	}
}