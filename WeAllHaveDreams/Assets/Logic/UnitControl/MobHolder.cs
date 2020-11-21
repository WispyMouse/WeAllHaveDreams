using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobHolder : MonoBehaviour
{
    public List<MapMob> ActiveMobs { get; set; } = new List<MapMob>();

    private void Awake()
    {
        LoadAllMobsFromScene();
    }

    public IEnumerable<MapMob> MobsOnTeam(int teamIndex)
    {
        return ActiveMobs.Where(mob => mob.PlayerSideIndex == teamIndex);
    }

    public void LoadAllMobsFromScene()
    {
        ActiveMobs = new List<MapMob>();

        foreach (MapMob curMob in GameObject.FindObjectsOfType<MapMob>())
        {
            curMob.SettleIntoGrid();

            if (ActiveMobs.Any(mob => mob.Position == curMob.Position))
            {
                Debug.LogWarning($"Multiple mobs are on the same position: {{{curMob.Position.x}, {curMob.Position.y}, {curMob.Position.z}}}");
            }

            ActiveMobs.Add(curMob);
        }
    }

    public MapMob MobOnPoint(Vector3Int position)
    {
        MapMob mobOnPoint;

        if ((mobOnPoint = ActiveMobs.FirstOrDefault(mob => mob.Position == position)) != null)
        {
            return mobOnPoint;
        }

        return null;
    }

    public void MoveUnit(MapMob toMove, Vector3Int to)
    {
        if (MobOnPoint(to))
        {
            Debug.LogWarning($"A unit is trying to move to an occuppied tile at {{{to.x}, {to.y}, {to.z}}}");
            return;
        }

        toMove.SetPosition(to);
        DebugTextLog.AddTextToLog($"Unit <unitnamehere> moved to {{{to.x}, {to.y}, {to.z}}}");
    }
}
