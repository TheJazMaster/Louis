using System.Collections.Generic;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class AResetUsedGems : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
		timer = 0;
		GemManager.usedGems = 0;
	}
}