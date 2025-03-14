using System;

namespace TheJazMaster.Louis;

public interface IEssentialsApi
{
	Type? GetExeCardTypeForDeck(Deck deck);
	bool IsExeCardType(Type type);
}