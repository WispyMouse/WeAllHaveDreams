using System.Collections;
using System.Collections.Generic;
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
        activeMap = GameMap.LoadFromRealm(toLoad);

        LoadedMap.ClearAllTiles();

        foreach (Vector3Int position in toLoad.AllPositions)
        {
            GameplayTile matchingTile = TileLibrary.GetTile(toLoad.TileTagAtPosition(position));
            LoadedMap.SetTile(position, matchingTile);
        }
    }
}
