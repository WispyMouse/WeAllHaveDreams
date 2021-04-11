using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMetaHolder : MonoBehaviour
{
    public WorldContext WorldContextInstance => WorldContext.GetWorldContext();
    public Tilemap MetaMap;

    public Tile MovementTile;
    public Tile AttackTile;

    HashSet<MapCoordinates> ActiveMovementTiles { get; set; }
    HashSet<MapCoordinates> ActiveAttackTiles { get; set; }

    public void ShowUnitMovementRange(MapMob toShow)
    {
        ClearMetas();

        foreach (MapCoordinates tile in WorldContextInstance.MapHolder.PotentialMoves(toShow))
        {
            MetaMap.SetTile(tile, MovementTile);
            ActiveMovementTiles.Add(tile);
        }
    }

    public void ShowUnitAttackRange(MapMob toShow, bool overrideExisting = true)
    {
        if (overrideExisting || ActiveAttackTiles == null)
        {
            ActiveAttackTiles = new HashSet<MapCoordinates>();
        }

        foreach (MapCoordinates possibleAttackTile in WorldContextInstance.MapHolder.PotentialAttacks(toShow, toShow.Position))
        {
            ActiveAttackTiles.Add(possibleAttackTile);
        }

        foreach (MapCoordinates attackTile in ActiveAttackTiles)
        {
            MetaMap.SetTile(attackTile, AttackTile);
        }
    }

    public void ShowUnitAttackRangePastMovementRange(MapMob toShow)
    {
        ShowUnitMovementRange(toShow);

        ActiveAttackTiles = new HashSet<MapCoordinates>();

        foreach (MapCoordinates movementTile in ActiveMovementTiles)
        {
            foreach (MapCoordinates possibleAttackTile in WorldContextInstance.MapHolder.PotentialAttacks(toShow, movementTile))
            {
                ActiveAttackTiles.Add(possibleAttackTile);
            }
        }

        foreach (MapCoordinates attackTile in ActiveAttackTiles.Except(ActiveMovementTiles))
        {
            MetaMap.SetTile(attackTile, AttackTile);
        }
    }

    public bool TileIsInActiveMovementRange(MapCoordinates position)
    {
        return ActiveMovementTiles != null
            && ActiveMovementTiles.Contains(position);
    }

    public void ClearMetas()
    {
        MetaMap.ClearAllTiles();
        ActiveMovementTiles = new HashSet<MapCoordinates>();
        ActiveAttackTiles = new HashSet<MapCoordinates>();
    }
}
