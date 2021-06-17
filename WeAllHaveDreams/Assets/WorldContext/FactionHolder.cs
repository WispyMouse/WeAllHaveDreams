using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionHolder : SingletonBase<FactionHolder>
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    SortedDictionary<int, PlayerSide> playerSides = new SortedDictionary<int, PlayerSide>();

    void Awake()
    {
        // TEMPORARY: Hardcode the sides
        PlayerSide humanControlledPlayerSide = new PlayerSide() { Name = "Human Player", PlayerSideIndex = 0, HumanControlled = true, TotalResources = 100 };
        playerSides.Add(humanControlledPlayerSide.PlayerSideIndex, humanControlledPlayerSide);

        PlayerSide aiControlledPlayerSide = new PlayerSide() { Name = "AI Player", PlayerSideIndex = 1, HumanControlled = false, TotalResources = 100 };
        playerSides.Add(aiControlledPlayerSide.PlayerSideIndex, aiControlledPlayerSide);
    }

    public static PlayerSide GetPlayer(int? index)
    {
        if (index == null)
        {
            return null;
        }

        return Singleton.playerSides[index.Value];
    }

    public static PlayerSide GetFirstPlayer()
    {
        return Singleton.playerSides.First().Value;
    }

    public static PlayerSide GetNextPlayer(PlayerSide previousPlayer)
    {
        IEnumerable<PlayerSide> fartherPlayers = Singleton.playerSides.Where(side => side.Key > previousPlayer.PlayerSideIndex).Select(side => side.Value);

        if (!fartherPlayers.Any())
        {
            return GetFirstPlayer();
        }

        return fartherPlayers.OrderBy(side => side.PlayerSideIndex).First();
    }

    public static IEnumerable<PlayerSide> GetPlayers()
    {
        return Singleton.playerSides.Select(side => side.Value);
    }
}
