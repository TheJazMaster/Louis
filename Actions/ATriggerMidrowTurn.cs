namespace TheJazMaster.Louis.Actions;

public class ATriggerMidrowTurn : CardAction
{
	public required StuffBase thing;

	public override void Begin(G g, State s, Combat c)
	{
		c.QueueImmediate(thing.GetActions(s, c));
	}
}