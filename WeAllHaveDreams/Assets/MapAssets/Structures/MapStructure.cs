using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapStructure : MapObject
{
    public int PlayerSideIndex;
    public bool UnCaptured;

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

    // TEMPORARY: This should definitely be in its own class
    public SpriteRenderer CapturePointsVisual;
    public Sprite[] CapturePointsNumerics;

    private void Awake()
    {
        _curCapturePoints = MaxCapturePoints;
    }

    public bool IsNotOwnedByMyTeam(int myTeam)
    {
        return UnCaptured || PlayerSideIndex != myTeam;
    }

    public void ProceedCapture(MapMob capturing)
    {
        int newCapturePoints = (int)System.Math.Max(0, CurCapturePoints - System.Math.Ceiling(capturing.HitPoints));
        DebugTextLog.AddTextToLog($"Mob <mobname> captures; {CurCapturePoints} => {newCapturePoints}");
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
        this.PlayerSideIndex = capturing.PlayerSideIndex;
        DebugTextLog.AddTextToLog("Base captured!");
        DebugTextLog.AddTextToLog("This is where the sprite would change, but that hasn't been implemented.");
    }
}
