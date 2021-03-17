using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MapStructure : MapObject
{
    public int PlayerSideIndex;
    public bool UnCaptured;

    public SpriteRenderer Renderer;
    public Sprite[] SideSprites;

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
            UpdateCapturePointsVisual();
        }
    }
    private int _curCapturePoints { get; set; }

    public int ContributedResourcesPerTurn = 50;
    public int CaptureImportance = 10;

    // TEMPORARY: This should definitely be in its own class
    public SpriteRenderer CapturePointsVisual;
    public Sprite[] CapturePointsNumerics;

    public string StructureName;

    private void Awake()
    {
        _curCapturePoints = MaxCapturePoints;

        if (!UnCaptured)
        {
            Renderer.sprite = SideSprites[this.PlayerSideIndex];
        }
    }
    
    public void SetOwnership(int side)
    {
        this.PlayerSideIndex = side;
        Renderer.sprite = SideSprites[side];
        UnCaptured = false;
        CurCapturePoints = MaxCapturePoints;
    }

    public bool IsNotOwnedByMyTeam(int myTeam)
    {
        return UnCaptured || PlayerSideIndex != myTeam;
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

    public void UpdateCapturePointsVisual()
    {
        if (CurCapturePoints == MaxCapturePoints)
        {
            CapturePointsVisual.gameObject.SetActive(false);
            return;
        }

        CapturePointsVisual.gameObject.SetActive(true);
        CapturePointsVisual.sprite = CapturePointsNumerics[CurCapturePoints];
    }

    protected virtual void CompleteCapture(MapMob capturing)
    {
        SetOwnership(capturing.PlayerSideIndex);
        DebugTextLog.AddTextToLog("Base captured!");
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
}
