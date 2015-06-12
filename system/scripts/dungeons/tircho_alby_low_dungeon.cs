//--- Aura Script -----------------------------------------------------------
// Alby Basic
//--- Description -----------------------------------------------------------
// Script for Alby Basic.
//---------------------------------------------------------------------------

using Aura.Channel.World.Dungeons;

[DungeonScript("tircho_alby_low_dungeon")]
public class AlbyBasicDungeonScript : DungeonScript
{
	public override void OnBoss(Dungeon dungeon)
	{
		dungeon.AddBoss(30007, 1); // Giant Red Spider
		dungeon.AddBoss(30013, 6); // Dark Blue Spider

		foreach (var member in dungeon.Party)
		{
			var cutscene = new Cutscene("bossroom_Albi_GiantSpider_DarkBlueSpider", member);
			cutscene.AddActor("player0", member);
			cutscene.AddActor("#giant_spider", 30007);
			cutscene.AddActor("#darkblue_spider", 30013);
			cutscene.Play();
		}
	}

	public override void OnCleared(Dungeon dungeon)
	{
		var rnd = RandomProvider.Get();

		for (int i = 0; i < dungeon.Party.Count; ++i)
		{
			var treasureChest = new TreasureChest();

			if (i == 0)
			{
				// Dagger
				var item = new Item(40006);
				switch (rnd.Next(3))
				{
					case 0: item.OptionInfo.Prefix = 20501; break; // Simple
					case 1: item.OptionInfo.Prefix = 20502; break; // Scrupulous
					case 2: item.OptionInfo.Prefix = 20201; break; // Hard
				}
				treasureChest.Add(item);
			}

			treasureChest.AddGold(rnd.Next(1072, 3680)); // Gold
			treasureChest.Add(GetRandomTreasureItem(rnd)); // Random item

			dungeon.AddChest(treasureChest);
		}
	}

	DropData[] drops;
	public Item GetRandomTreasureItem(Random rnd)
	{
		if (drops == null)
		{
			drops = new DropData[]
			{
				new DropData(itemId: 62004, chance: 38, amountMin: 1, amountMax: 2), // Magic Powder
				new DropData(itemId: 51102, chance: 38, amountMin: 1, amountMax: 2), // Mana Herb
				new DropData(itemId: 60042, chance: 9, amountMin: 1, amountMax: 5), // Magical Silver Thread
				new DropData(itemId: 63101, chance: 9, amount: 1, expires: 600), // Alby Basic Fomor Pass
				new DropData(itemId: 63116, chance: 2, amount: 1, expires: 480), // Alby Intermediate Fomor Pass for One
				new DropData(itemId: 63117, chance: 2, amount: 1, expires: 480), // Alby Intermediate Fomor Pass for Two
				new DropData(itemId: 63118, chance: 2, amount: 1, expires: 480), // Alby Intermediate Fomor Pass for Four
			};
		}

		return Item.GetRandomDrop(rnd, drops);
	}
}
