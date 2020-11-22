﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMeta : MonoBehaviour
{
    public Tilemap MetaMap;
    public MapHolder MapHolderController;

    public Tile MovementTile;
    public Tile AttackTile;

    HashSet<Vector3Int> ActiveMovementTiles { get; set; }
    HashSet<Vector3Int> ActiveAttackTiles { get; set; }

    public void ShowUnitMovementRange(MapMob toShow)
    {
        ClearMetas();

        foreach (Vector3Int tile in MapHolderController.PotentialMoves(toShow))
        {
            MetaMap.SetTile(tile, MovementTile);
            ActiveMovementTiles.Add(tile);
        }
    }

    public void ShowUnitAttackRangePastMovementRange(MapMob toShow)
    {
        ActiveAttackTiles = new HashSet<Vector3Int>();

        foreach (Vector3Int movementTile in ActiveMovementTiles)
        {
            foreach (Vector3Int possibleAttackTile in MapHolderController.PotentialAttacks(toShow, movementTile))
            {
                ActiveAttackTiles.Add(possibleAttackTile);
            }
        }

        foreach (Vector3Int attackTile in ActiveAttackTiles.Except(ActiveMovementTiles))
        {
            MetaMap.SetTile(attackTile, AttackTile);
        }
    }

    public bool TileIsInActiveMovementRange(Vector3Int position)
    {
        return ActiveMovementTiles != null
            && ActiveMovementTiles.Contains(position);
    }

    public void ClearMetas()
    {
        MetaMap.ClearAllTiles();
        ActiveMovementTiles = new HashSet<Vector3Int>();
        ActiveAttackTiles = new HashSet<Vector3Int>();
    }
}
