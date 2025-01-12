using System;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class GemManager : IKokoroApi.IV2.IActionCostsApi.IResourceProvider
{
    private static ModEntry Instance => ModEntry.Instance;
    private static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    internal static readonly string LastGemmedTurnKey = "LastGemmedTurn";
    internal static readonly string GemsLeftKey = "GemsLeft";

    internal static ICardTraitEntry GemTrait { get; private set; } = null!;
    internal static ICardTraitEntry PreciousGemTrait { get; private set; } = null!;



    public GemManager()
    {   
        GemTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("Gem", new() {
            Icon = (_, _) => Instance.GemIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "gem"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::Gem")
				{
					Icon = Instance.GemIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["trait", "gem", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["trait", "gem", "description"]),
				}
            ]
        });
        PreciousGemTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("PreciousGem", new() {
            Icon = (_, _) => Instance.PreciousGemIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "preciousGem"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::PreciousGem")
				{
					Icon = Instance.PreciousGemIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["trait", "preciousGem", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["trait", "preciousGem", "description"]),
				}
            ]
        });
        
        ModEntry.Instance.KokoroApi.ActionCosts.RegisterResourceProvider(this, 1000);
        
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.Make)),
			postfix: new HarmonyMethod(GetType(), nameof(Combat_Make_Postfix))
		);
    }
    
    private static void Combat_Make_Postfix(State s) {
        foreach (Card card in s.deck) {
            ModData.RemoveModData(card, LastGemmedTurnKey);
        }
    }

    public static void SetGem(Card card, State s, bool? value, bool permanent = false) {
        CardsHelper.SetCardTraitOverride(s, card, GemTrait, value, permanent);
    }

    public static void SetPreciousGem(Card card, State s, bool? value, bool permanent = false) {
        CardsHelper.SetCardTraitOverride(s, card, PreciousGemTrait, value, permanent);
    }

    public static bool HasGem(Card card, State s)
    {
        return CardsHelper.IsCardTraitActive(s, card, GemTrait);
    }

    public static bool HasPreciousGem(Card card, State s)
    {
        return CardsHelper.IsCardTraitActive(s, card, PreciousGemTrait);
    }

    public static int GetGemHandCount(State s, Combat c, int? excludedId = null) {
        int total = 0;
        foreach (Card card in c.hand) {
            if (excludedId == card.uuid) continue;
            if (CardsHelper.IsCardTraitActive(s, card, GemTrait)) total++;
            else if (CardsHelper.IsCardTraitActive(s, card, PreciousGemTrait)) total += 2;
        }
        return total;
    }

    public int GetCurrentResourceAmount(IKokoroApi.IV2.IActionCostsApi.IResourceProvider.IGetCurrentResourceAmountArgs args)
    {
        if (args.Card is not { } card)
            return 0;
        if (ModEntry.Instance.KokoroApi.ActionCosts.AsStatusResource(args.Resource) is not { Status: Status.shard })
            return 0;
        
        int lastGemmedTurn = args.Combat == DB.fakeCombat ? 0 : ModData.GetModDataOrDefault<int>(card, LastGemmedTurnKey);
        int gemsLeft = lastGemmedTurn < args.Combat.turn
            ? GetGemHandCount(args.State, args.Combat, args.Card?.uuid)
            : ModData.GetModDataOrDefault<int>(card, GemsLeftKey);
        
        return Math.Max(gemsLeft, 0);
    }

    public void PayResource(IKokoroApi.IV2.IActionCostsApi.IResourceProvider.IPayResourceArgs args)
    {
        if (args.Card is not { } card)
            return;
        if (ModEntry.Instance.KokoroApi.ActionCosts.AsStatusResource(args.Resource) is not { Status: Status.shard })
            return;
        
        int lastGemmedTurn = args.Combat == DB.fakeCombat ? 0 : ModData.GetModDataOrDefault<int>(card, LastGemmedTurnKey);
        int gemsLeft;
        
        if (lastGemmedTurn < args.Combat.turn)
        {
            gemsLeft = GetGemHandCount(args.State, args.Combat, args.Card?.uuid);
            ModData.SetModData(card, LastGemmedTurnKey, args.Combat.turn);
        }
        else
        {
            gemsLeft = ModData.GetModDataOrDefault<int>(card, GemsLeftKey);
        }
        
        gemsLeft = Math.Max(gemsLeft - args.Amount, 0);
        ModData.SetModData(card, GemsLeftKey, gemsLeft);
    }
}