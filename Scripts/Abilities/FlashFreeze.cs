using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFreeze : Ability {
    List<GameObject> iceList = new List<GameObject>();

    new public void init(Map map, BaseTile origin) {
        base.init(map, origin);
        cooldown = 1;

        getAffected();
    }

    public void getAffected() {
        foreach (BaseTile tile in gameMap.getTilesWithin(5, originPos)) {
            if (tile.tileType == BaseTile.TileTypes.Grass) {
                affectedTiles.Add(tile);
            }
            else if (tile.tileType == BaseTile.TileTypes.River) {
                affectedTiles.Add(tile);
            }
        }
    }

    new public void turnStart() {
        base.turnStart();
        if (cooldown == 0) {
            foreach (BaseTile tile in affectedTiles) {
                if (tile != null) {
                    tile.tileType = BaseTile.TileTypes.Ice;
                    tile.moveCost *= 2;

                    Vector3 tempPos = tile.transform.position;
                    tempPos.y += 0.175f;
                    iceList.Add(Instantiate(gameMap.iceObject, tempPos, tile.transform.rotation, gameMap.transform));
                    iceList[iceList.Count - 1].gameObject.layer = tile.gameObject.layer;
                    
                }
            }
            GetComponent<ParticleSystem>().Stop();
        }
        else if (cooldown < 0) {
            for (int i = 0; i< iceList.Count; i++) { 
                if (i < iceList.Count) {
                    if (iceList[i] != null || affectedTiles[i] != null) {
                        if (Random.Range(0.0f, 1.0f) <= 0.45f) {
                            if (affectedTiles[i].GetComponent<GrassTile>() != null) {
                                affectedTiles[i].GetComponent<GrassTile>().tileType = BaseTile.TileTypes.Grass;
                            }
                            else if (affectedTiles[i].GetComponent<RiverTile>() != null) {
                                affectedTiles[i].GetComponent<RiverTile>().tileType = BaseTile.TileTypes.River;
                            }
                            affectedTiles[i].GetComponent<BaseTile>().moveCost /= 2;
                            Destroy(iceList[i].gameObject);
                            iceList.RemoveAt(i);
                            affectedTiles.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            if (affectedTiles.Count == 0) {
                Destroy(gameObject);
            }
        }
    }
}