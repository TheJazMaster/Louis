namespace TheJazMaster.Louis;

public interface ITuckerApi
{
	AAttack MakeNewBluntAttack();
	bool IsBluntAttack(AAttack attack);
	AAttack ApplyOffset(AAttack attack, int offset);
	int? GetOffset(AAttack attack);

	Deck TuckerDeck { get; }
	Status BufferStatus { get; }
	Status FuelLeakStatus { get; }
}
