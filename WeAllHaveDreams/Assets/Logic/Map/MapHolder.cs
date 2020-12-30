using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHolder : MonoBehaviour
{
    GameMap activeMap { get; set; }

    public Tilemap LoadedMap;

    public MobHolder MobHolderController;

    public GameplayTile GetGameplayTile(Vector3Int position) => activeMap.GetGameplayTile(position);
    public IEnumerable<Vector3Int> GetAllTiles() => activeMap.GetAllTiles();
    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int point) => activeMap.GetNeighbors(point);

    public IEnumerable<Vector3Int> PotentialMoves(MapMob moving) => activeMap.PotentialMoves(moving, MobHolderController);
    public IEnumerable<Vector3Int> PotentialAttacks(MapMob attacking, Vector3Int from) => activeMap.PotentialAttacks(attacking, from);
    public List<Vector3Int> Path(MapMob moving, Vector3Int to) => activeMap.Path(moving, to, MobHolderController);
    public IEnumerable<Vector3Int> CanHitFrom(MapMob attacking, Vector3Int target) => activeMap.CanAttackFrom(attacking, target);

    public async Task LoadFromRealm(Realm toLoad)
    {
        activeMap = await GameMap.LoadFromRealm(toLoad);

        LoadedMap.ClearAllTiles();

        foreach (Vector3Int position in toLoad.AllPositions)
        {
            // TEMPORARY HACK: Tiles are not the only things defined here. For now, get the first tile indicated and discard the rest.
            foreach (GameplayTile curTile in toLoad.KeysAtPositions[position].Select(key => key.GetTileInstance()))
            {
                LoadedMap.SetTile(position, curTile);
                break;
            }
        }
    }

    public void CenterCamera(Camera toCenter)
    {
        Vector3 center = new Vector3(((float)activeMap.GetAllTiles().Min(tile => tile.x) + (float)activeMap.GetAllTiles().Max(tile => tile.x)) / 2f,
            ((float)activeMap.GetAllTiles().Min(tile => tile.y) + (float)activeMap.GetAllTiles().Max(tile => tile.y)) / 2f,
            0);

        toCenter.transform.position = center + Vector3.back * 10;
    }
}
