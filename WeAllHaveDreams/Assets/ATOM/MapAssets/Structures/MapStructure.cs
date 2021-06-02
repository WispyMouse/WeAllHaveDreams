using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapStructure : MapObject
{
    public int? PlayerSideIndex;

    public StructureConfiguration Configuration { get; set; }

    public int MaxCapturePoints { get; set; } = 20;
    public int CurCapturePoints
    {
        get
        {
            return _curCapturePoints;
        }
        set
        {
            _curCapturePoints = value;
            StructureUpdated.Invoke(this);
        }
    }
    private int _curCapturePoints { get; set; }

    public int ContributedResourcesPerTurn = 50;
    public int CaptureImportance = 10;

    public string StructureName => Configuration.Name;
    public IEnumerable<StructureConfigurationAbility> Abilities { get; private set; } = new List<StructureConfigurationAbility>();

    public UnityEvent<MapStructure> StructureUpdated;
    
    public void SetOwnership(int? side)
    {
        this.PlayerSideIndex = side;
        CurCapturePoints = MaxCapturePoints;
        StructureUpdated.Invoke(this);
    }

    public bool IsNotOwnedByMyTeam(int myTeam)
    {
        return PlayerSideIndex != myTeam;
    }

    public void ProceedCapture(MapMob capturing)
    {
        int newCapturePoints = (int)System.Math.Max(0, CurCapturePoints - capturing.CurrentCapturePoints);
        DebugTextLog.AddTextToLog($"Mob {capturing.Name} captures; {CurCapturePoints} => {newCapturePoints}");
        CurCapturePoints = newCapturePoints;

        if (CurCapturePoints <= 0)
        {
            CompleteCapture(capturing);
        }
    }

    protected virtual void CompleteCapture(MapMob capturing)
    {
        SetOwnership(capturing.PlayerSideIndex);
        DebugTextLog.AddTextToLog("Base captured!", DebugTextLogChannel.Gameplay);

        foreach (StructureConfigurationAbility structureAbility in Abilities)
        {
            structureAbility.OnCapture(this);
        }
    }

    public IEnumerable<PlayerInput> GetPossiblePlayerInputs(WorldContext worldContext)
    {
        IEnumerable<PlayerInput> possibleActions = new List<PlayerInput>();
        foreach (StructureConfigurationAbility curAbility in Abilities)
        {
            possibleActions = possibleActions.Union(curAbility.GetPossiblePlayerInputs(this));
        }
        return possibleActions;
    }

    public void ClearCapture()
    {
        CurCapturePoints = MaxCapturePoints;
    }

    public StructureMapData GetMapData()
    {
        return new StructureMapData() { Position = this.Position, Ownership = PlayerSideIndex, StructureName = StructureName };
    }

    public void LoadFromConfiguration(Configuration.StructureConfiguration configuration)
    {
        Configuration = configuration;
        gameObject.name = Configuration.Name;

        Abilities = configuration.GetSaturatedAbilities();

        StructureUpdated.Invoke(this);
    }

    public decimal GetDefensiveRatio(MapMob defending)
    {
        // If we have no defensive values here, then there's no defenses
        if (!Configuration.Defenses.Any())
        {
            return 0;
        }

        // Get the best (lowest) defensive value out of the tags that apply to this unit
        decimal? bestDefense = null;
        foreach (DefensiveAttributes defense in Configuration.Defenses)
        {
            if (defense.TagsApply(defending.Tags))
            {
                if (!bestDefense.HasValue || bestDefense.Value > defense.DefensiveRatio)
                {
                    bestDefense = defense.DefensiveRatio;
                }
            }
        }

        return bestDefense.HasValue ? bestDefense.Value : 0;
    }
}
