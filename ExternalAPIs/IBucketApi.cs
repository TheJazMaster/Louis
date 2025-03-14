namespace TheJazMaster.Louis;


public interface IBucketApi
{
	Deck BucketDeck { get; }
	Status IngenuityStatus { get; }
	Status SalvageStatus { get; }
	Status SteamCoverStatus { get; }
}
