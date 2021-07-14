using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainInfoPopup : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();

    public GameObject PopupVisual;
    public Text TerrainNameLabel;

    public GameObject UnitSelectedSubpanel;
    public Text MovementCostLabel;
    public Text DefensiveValueLabel;

    MapMob selectedMob;

    public void OnMobSelected(MapMob selected)
    {
        selectedMob = selected;
    }

    public void OnCursorPositionChanged(MapCoordinates? coordinates)
    {
        if (!coordinates.HasValue)
        {
            PopupVisual.gameObject.SetActive(false);
            return;
        }

        PopupVisual.gameObject.SetActive(true);

        GameplayTile gameplayTile = WorldContextInstance.MapHolder.GetGameplayTile(coordinates.Value);
        TerrainNameLabel.text = gameplayTile.TileName;

        if (selectedMob != null)
        {
            UnitSelectedSubpanel.gameObject.SetActive(false);
            return;
        }

        UnitSelectedSubpanel.gameObject.SetActive(true);

        MovementCostAttribute cost = gameplayTile.MovementCosts(selectedMob);
        if (cost == null)
        {
            MovementCostLabel.text = "1";
        }
        else
        {
            MovementCostLabel.text = cost.Value.ToString();
        }

        DefensiveAttributes defenses = gameplayTile.DefensiveAttribute(selectedMob);
        if (cost == null)
        {
            DefensiveValueLabel.text = "100%";
        }
        else
        {
            DefensiveValueLabel.text = defenses.DefensiveRatio.ToString("P");
        }
    }
}
