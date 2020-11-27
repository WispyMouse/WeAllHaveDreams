using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SideStatistics : MonoBehaviour
{
    public Text SideStatisticsLabel;

    public void UpdateVisuals()
    {
        StringBuilder textString = new StringBuilder();

        foreach (PlayerSide side in TurnManager.GetPlayers())
        {
            textString.Append($"<sidename> // {side.PlayerSideIndex} // {side.TotalResources}");
            textString.AppendLine("");
        }

        SideStatisticsLabel.text = textString.ToString();
    }
}
