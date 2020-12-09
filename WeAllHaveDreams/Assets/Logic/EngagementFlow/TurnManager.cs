﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TurnManager : SingletonBase<TurnManager>
{
    public static bool GameIsInProgress { get; set; } = false;

    int playerIndex { get; set; }
    SortedDictionary<int, PlayerSide> playerSides = new SortedDictionary<int, PlayerSide>();

    public static PlayerSide CurrentPlayer => Singleton.playerSides[Singleton.playerIndex];

    public FogHolder FogHolderController;
    public MapHolder MapHolderController;

    public MobHolder MobHolderController;
    public StructureHolder StructureHolderInstance;

    public PlayerInputPhaseController PlayerInputPhaseControllerInstance;
    public AIInputPhaseController AIInputPhaseControllerInstance;

    public SideStatistics SideStatisticsInstance;

    private void Start()
    {
        // Kick off the start up sequence, which will start the game when it is done
        _ = HandleGameBootupSequence(GameplayReady);
    }

    async Task HandleGameBootupSequence(Action preparationsReady)
    {
        // Load in the configuration data
        await ConfigurationLoadingEntrypoint.LoadAllConfigurationData();
        preparationsReady();
    }

    void GameplayReady()
    {
        // TEMPORARY: Hardcode the sides
        PlayerSide humanControlledPlayerSide = new PlayerSide() { Name = "Human Player", PlayerSideIndex = 0, HumanControlled = true, TotalResources = 100 };
        playerSides.Add(humanControlledPlayerSide.PlayerSideIndex, humanControlledPlayerSide);

        PlayerSide aiControlledPlayerSide = new PlayerSide() { Name = "AI Player", PlayerSideIndex = 1, HumanControlled = false, TotalResources = 100 };
        playerSides.Add(aiControlledPlayerSide.PlayerSideIndex, aiControlledPlayerSide);

        FogHolderController.Initialize(MapHolderController);

        GameIsInProgress = true;

        StartCoroutine(StartTurnOfPlayerAtIndex(humanControlledPlayerSide.PlayerSideIndex));
    }

    IEnumerator StartTurnOfPlayerAtIndex(int index)
    {
        playerIndex = index;
        DebugTextLog.AddTextToLog($"START OF TURN: {CurrentPlayer.Name}, engage!");

        foreach (MapStructure structure in StructureHolderInstance.ActiveStructures.Where(structure => !structure.UnCaptured && structure.PlayerSideIndex == index))
        {
            CurrentPlayer.TotalResources += structure.ContributedResourcesPerTurn;
        }

        foreach (MapMob curMob in Singleton.MobHolderController.MobsOnTeam(CurrentPlayer.PlayerSideIndex))
        {
            curMob.RefreshForStartOfTurn();
        }

        yield return ResolveEffects();

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
            Singleton.StartCoroutine(Singleton.StartTurnOfPlayerAtIndex(laterSides.First()));
        }
        else
        {
            Singleton.StartCoroutine(Singleton.StartTurnOfPlayerAtIndex(Singleton.playerSides.Keys.Min()));
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

        Singleton.FogHolderController.UpdateVisibilityForPlayers(Singleton.MapHolderController, Singleton.MobHolderController);

        Singleton.SideStatisticsInstance.UpdateVisuals();
    }

    public static void VictoryIsDeclared(int winner)
    {
        Singleton.AIInputPhaseControllerInstance.StopAllInputs();
        Singleton.PlayerInputPhaseControllerInstance.StopAllInputs();
        DebugTextLog.AddTextToLog($"The winner is side #{winner}!");
        GameIsInProgress = false;
    }

    public static IEnumerable<PlayerSide> GetPlayers()
    {
        return Singleton.playerSides.Values;
    }
}
