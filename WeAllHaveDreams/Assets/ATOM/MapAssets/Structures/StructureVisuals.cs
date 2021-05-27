using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureVisuals : MonoBehaviour
{
    public SpriteRenderer Renderer;

    // TEMPORARY: This should definitely be in its own class
    public SpriteRenderer CapturePointsVisual;
    public Sprite[] CapturePointsNumerics;

    public void OnStructureUpdated(MapStructure updated)
    {
        SetOwnership(updated);
        SetCapturePoints(updated);
    }

    void SetOwnership(MapStructure updated)
    {
        Renderer.sprite = StructureLibrary.GetStructureSprite(updated.Configuration.Appearance, updated.PlayerSideIndex);
    }

    void SetCapturePoints(MapStructure updated)
    {
        if (updated.CurCapturePoints == updated.MaxCapturePoints)
        {
            CapturePointsVisual.gameObject.SetActive(false);
            return;
        }

        CapturePointsVisual.gameObject.SetActive(true);
        CapturePointsVisual.sprite = CapturePointsNumerics[updated.CurCapturePoints];
    }
}
