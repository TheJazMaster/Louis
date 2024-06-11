using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class ACardSelectImproved : ACardSelect
{
	public override List<Tooltip> GetTooltips(State s)
	{
        var list = base.GetTooltips(s);
        list.InsertRange(0, browseAction.GetTooltips(s));
        return list;
	}
}