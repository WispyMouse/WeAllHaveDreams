﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : SingletonBase<TurnManager>
{
    public static bool GameIsInProgress { get; set; } = true;

    int playerIndex { get; set; }
    SortedDictionary<int, PlayerSide> playerSides = new SortedDictionary<int, PlayerSide>();

    public static PlayerSide CurrentPlayer => Singleton.playerSides[Singleton.playerIndex];

    public MobHolder MobHolderController;
    public PlayerInputPhaseController PlayerInputPhaseControllerInstance;
    public AIInputPhaseController AIInputPhaseControllerInstance;

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
            PlayerInputPhaseControllerInstance.ResetPhases();
        }
        else
        {
            AIInputPhaseControllerInstance.StartTurn();
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

    public static IEnumerator ResolveEffects()
    {
        var shouldBeRemoved = new List<MapMob>();
        foreach (MapMob mapMob in Singleton.MobHolderController.ActiveMobs)
        {
            if (mapMob.HitPoints <= 0)
            {
                shouldBeRemoved.Add(mapMob);
                break;
            }
        }

        foreach (MapMob remove in shouldBeRemoved)
        {
            yield return Singleton.MobHolderController.RemoveMob(remove);
        }

        yield break;
    }

    public static void VictoryIsDeclared(int winner)
    {
        Singleton.AIInputPhaseControllerInstance.StopAllInputs();
        Singleton.PlayerInputPhaseControllerInstance.StopAllInputs();
        DebugTextLog.AddTextToLog($"The winner is side #{winner}!");
        GameIsInProgress = false;
    }
}
