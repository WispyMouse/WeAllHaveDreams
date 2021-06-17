using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapStructure : MapObject
{
    public PlayerSide MyPlayerSide;

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
    
    public void SetOwnership(PlayerSide side)
    {
        this.MyPlayerSide = side;
        CurCapturePoints = MaxCapturePoints;
        StructureUpdated.Invoke(this);
    }

    public bool IsNotOwnedByMyTeam(PlayerSide myTeam)
    {
        return MyPlayerSide != myTeam;
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
        SetOwnership(capturing.MyPlayerSide);
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
        return new StructureMapData() { Position = this.Position, Ownership = MyPlayerSide.PlayerSideIndex, StructureName = StructureName };
    }

    public void LoadFromConfiguration(Configuration.StructureConfiguration configuration)
    {
        Configuration = configuration;
        gameObject.name = Configuration.Name;

        Abilities = configuration.GetSaturatedAbilities();

        StructureUpdated.Invoke(this);
    }
}
