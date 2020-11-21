using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : SingletonBase<TurnManager>
{
    int playerIndex { get; set; }
    SortedDictionary<int, PlayerSide> playerSides = new SortedDictionary<int, PlayerSide>();

    public static PlayerSide CurrentPlayer => Singleton.playerSides[Singleton.playerIndex];

    public MobHolder MobHolderController;

    private void Start()
    {
        // TEMPORARY: Hardcode the sides
        PlayerSide humanControlledPlayerSide = new PlayerSide() { Name = "Human Player", PlayerSideIndex = 0, HumanControlled = true };
        playerSides.Add(humanControlledPlayerSide.PlayerSideIndex, humanControlledPlayerSide);

        PlayerSide aiControlledPlayerSide = new PlayerSide() { Name = "AI Player", PlayerSideIndex = 1, HumanControlled = false };
        playerSides.Add(aiControlledPlayerSide.PlayerSideIndex, aiControlledPlayerSide);

        StartTurnOfPlayerAtIndex(humanControlledPlayerSide.PlayerSideIndex);
    }

    void StartTurnOfPlayerAtIndex(int index)
    {
        playerIndex = index;
        DebugTextLog.AddTextToLog($"START OF TURN: {CurrentPlayer.Name}, engage!");

        foreach (MapMob curMob in Singleton.MobHolderController.MobsOnTeam(CurrentPlayer.PlayerSideIndex))
        {
            curMob.RefreshForStartOfTurn();
        }

        if (CurrentPlayer.HumanControlled)
        {
            DebugTextLog.AddTextToLog("Press 'enter' to pass the turn.");
        }
        else
        {
            // TEMPORARY: Just pass the turn, let the human play the game ~
            DebugTextLog.AddTextToLog("The AI passes their turn");
            PassTurnToNextPlayer();
        }
    }

    public static void PassTurnToNextPlayer()
    {
        foreach (MapMob curMob in Singleton.MobHolderController.MobsOnTeam(CurrentPlayer.PlayerSideIndex))
        {
            curMob.ClearForEndOfTurn();
        }

        IEnumerable<int> laterSides = Singleton.playerSides.Keys.Where(side => side > Singleton.playerIndex);

        if (laterSides.Any())
        {
            Singleton.StartTurnOfPlayerAtIndex(laterSides.First());
        }
        else
        {
            Singleton.StartTurnOfPlayerAtIndex(Singleton.playerSides.Keys.Min());
        }
    }
}
