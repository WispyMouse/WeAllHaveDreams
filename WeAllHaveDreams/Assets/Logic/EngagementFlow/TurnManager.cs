using System;
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

    // TEMPORARY: This should not be here! But, it is the easiest place to insert for now.
    public Camera MainCamera;

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public PlayerInputPhaseController PlayerInputPhaseControllerInstance;
    public AIInputPhaseController AIInputPhaseControllerInstance;

    public SideStatistics SideStatisticsInstance;

    public void GameplayReady()
    {
        // TEMPORARY: Hardcode the sides
        PlayerSide humanControlledPlayerSide = new PlayerSide() { Name = "Human Player", PlayerSideIndex = 0, HumanControlled = true, TotalResources = 100 };
        playerSides.Add(humanControlledPlayerSide.PlayerSideIndex, humanControlledPlayerSide);

        PlayerSide aiControlledPlayerSide = new PlayerSide() { Name = "AI Player", PlayerSideIndex = 1, HumanControlled = false, TotalResources = 100 };
        playerSides.Add(aiControlledPlayerSide.PlayerSideIndex, aiControlledPlayerSide);
        AIInputPhaseControllerInstance.LoadSettings();

        WorldContextInstance.FogHolder.Initialize();
        CameraController.CenterCamera();

        GameIsInProgress = true;

        StartCoroutine(StartTurnOfPlayerAtIndex(humanControlledPlayerSide.PlayerSideIndex));
    }

    IEnumerator StartTurnOfPlayerAtIndex(int index)
    {
        playerIndex = index;
        DebugTextLog.AddTextToLog($"START OF TURN: {CurrentPlayer.Name}, engage!");

        foreach (MapStructure structure in WorldContextInstance.StructureHolder.ActiveStructures.Where(structure => structure.PlayerSideIndex == index))
        {
            CurrentPlayer.TotalResources += structure.ContributedResourcesPerTurn;
        }

        foreach (MapMob curMob in WorldContextInstance.MobHolder.MobsOnTeam(CurrentPlayer.PlayerSideIndex))
        {
            curMob.RefreshForStartOfTurn();
        }

        yield return ResolveStartOfTurnEffects();
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
        foreach (MapMob curMob in Singleton.WorldContextInstance.MobHolder.MobsOnTeam(CurrentPlayer.PlayerSideIndex))
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

    public static IEnumerator ResolveStartOfTurnEffects()
    {
        // HACK: For now, ask each Feature under each of our Mobs if they have a start of turn effect
        foreach (MapMob curMob in Singleton.WorldContextInstance.MobHolder.MobsOnTeam(CurrentPlayer.PlayerSideIndex))
        {
            MapFeature onFeature = Singleton.WorldContextInstance.FeatureHolder.FeatureOnPoint(curMob.Position);

            if (onFeature != null && onFeature.HasStartOfTurnEffects)
            {
                onFeature.StartOfTurnEffects(curMob);
            }
        }

        yield break;
    }

    public static IEnumerator ResolveEffects()
    {
        var shouldBeRemoved = new List<MapMob>();
        foreach (MapMob mapMob in Singleton.WorldContextInstance.MobHolder.ActiveMobs)
        {
            if (mapMob.HitPoints <= 0)
            {
                shouldBeRemoved.Add(mapMob);
                break;
            }
        }

        foreach (MapMob remove in shouldBeRemoved)
        {
            Vector3Int position = remove.Position;

            Singleton.WorldContextInstance.MobHolder.RemoveMob(remove);
            Singleton.WorldContextInstance.StructureHolder.MobRemovedFromPoint(position);
        }

        Singleton.WorldContextInstance.FogHolder.UpdateVisibilityForPlayers();

        Singleton.SideStatisticsInstance.UpdateVisuals();
        yield break;
    }

    public static void VictoryIsDeclared(int winner)
    {
        DebugTextLog.AddTextToLog($"The winner is side #{winner}!");
        StopAllInputs();
    }

    public static void StopAllInputs()
    {
        Singleton.AIInputPhaseControllerInstance.StopAllInputs();
        Singleton.PlayerInputPhaseControllerInstance.StopAllInputs();
        GameIsInProgress = false;
    }

    public static IEnumerable<PlayerSide> GetPlayers()
    {
        return Singleton.playerSides.Values;
    }
}
