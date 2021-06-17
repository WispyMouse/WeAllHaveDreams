using AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TurnManager : SingletonBase<TurnManager>
{
    public static bool GameIsInProgress { get; set; } = false;

    public static PlayerSide CurrentPlayer { get; set; }

    // TEMPORARY: This should not be here! But, it is the easiest place to insert for now.
    public Camera MainCamera;

    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public PlayerInputPhaseController PlayerInputPhaseControllerInstance;
    public AIInputPhaseController AIInputPhaseControllerInstance;

    public SideStatistics SideStatisticsInstance;

    public void GameplayReady()
    {
        AIInputPhaseControllerInstance.LoadSettings();

        WorldContextInstance.FogHolder.Initialize();
        CameraController.CenterCamera();

        GameIsInProgress = true;

        StartCoroutine(StartTurnOfPlayer(FactionHolder.GetFirstPlayer()));
    }

    IEnumerator StartTurnOfPlayer(PlayerSide player)
    {
        CurrentPlayer = player;
        DebugTextLog.AddTextToLog($"START OF TURN: {CurrentPlayer.Name}, engage!");

        foreach (MapStructure structure in WorldContextInstance.StructureHolder.ActiveStructures.Where(structure => structure.MyPlayerSide == CurrentPlayer))
        {
            CurrentPlayer.TotalResources += structure.ContributedResourcesPerTurn;
        }

        foreach (MapMob curMob in WorldContextInstance.MobHolder.ActiveMobs)
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
        foreach (MapMob curMob in Singleton.WorldContextInstance.MobHolder.MobsOnTeam(CurrentPlayer))
        {
            curMob.ClearForEndOfTurn();
        }

        Singleton.StartCoroutine(Singleton.StartTurnOfPlayer(FactionHolder.GetNextPlayer(CurrentPlayer)));
    }

    public static IEnumerator ResolveStartOfTurnEffects()
    {
        // HACK: For now, ask each Feature under each of our Mobs if they have a start of turn effect
        foreach (MapMob curMob in Singleton.WorldContextInstance.MobHolder.MobsOnTeam(CurrentPlayer))
        {
            MapFeature onFeature = Singleton.WorldContextInstance.FeatureHolder.FeatureOnPoint(curMob.Position);

            if (onFeature != null && onFeature.HasStartOfTurnEffects)
            {
                onFeature.StartOfTurnEffects(curMob);
            }

            MapStructure onStructure = Singleton.WorldContextInstance.StructureHolder.StructureOnPoint(curMob.Position);

            if (onStructure != null)
            {
                foreach (StructureConfigurationAbility ability in onStructure.Abilities)
                {
                    ability.OnTurnStart(onStructure, curMob);
                }
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
            MapCoordinates position = remove.Position;

            Singleton.WorldContextInstance.MobHolder.RemoveMob(remove);
            Singleton.WorldContextInstance.StructureHolder.MobRemovedFromPoint(position);
        }

        Singleton.WorldContextInstance.FogHolder.UpdateVisibilityForPlayers();

        Singleton.SideStatisticsInstance.UpdateVisuals();
        yield break;
    }

    public static void VictoryIsDeclared(PlayerSide winner)
    {
        DebugTextLog.AddTextToLog($"The winner is side #{winner.PlayerSideIndex}!");
        StopAllInputs();
    }

    public static void StopAllInputs()
    {
        Singleton.AIInputPhaseControllerInstance.StopAllInputs();
        Singleton.PlayerInputPhaseControllerInstance.StopAllInputs();
        GameIsInProgress = false;
    }
}
