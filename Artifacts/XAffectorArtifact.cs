using System.Collections.Generic;

namespace TheJazMaster.Louis.Artifacts;

public interface IXAffectorArtifact
{
	int AffectX(Card card, List<CardAction> actions, State s, Combat c, int xBonus);
}