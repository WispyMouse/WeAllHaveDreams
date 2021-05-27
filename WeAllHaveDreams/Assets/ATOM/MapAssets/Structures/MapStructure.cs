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
    }

    public virtual PlayerInput DoLazyBuildingThing(WorldContext worldContext)
    {
        return null;
    }

    public virtual IEnumerable<PlayerInput> GetPossiblePlayerInputs(WorldContext worldContext)
    {
        return Enumerable.Empty<PlayerInput>();
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

        StructureUpdated.Invoke(this);
    }
}
