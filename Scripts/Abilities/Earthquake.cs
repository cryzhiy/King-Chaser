using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : Ability {
    
    public void init(Map map, BaseTile origin, Vector3 direction) {
        base.init(map, origin);
        direc = direction;

        getAffected();
    }

    public void getAffected() {
        Vector3 tempPos = originPos;
        while (Vector3.Distance(tempPos, originPos) < 7.5f) {
            BaseTile tempTile = gameMap.tileMap[(int)tempPos.z][(int)tempPos.x];
            if (tempTile.tileType == BaseTile.TileTypes.EndZone) {
                break;
            }
            if (!affectedTiles.Contains(tempTile)) {
                affectedTiles.Add(tempTile);

                Vector3 target = tempPos + direc * 10;
                Vector2 rand = Random.insideUnitCircle * 5;
                target += new Vector3(rand.x, 0, rand.y);
                direc = target - tempPos;
                direc = gameMap.scaleDown(direc);
            }
            tempPos += direc;
            if (gameMap.onBoard(tempPos) == false) {
                break;
            }
        }
    }

    new public void turnStart() {
        base.turnStart();
        if (cooldown <= 0) {
            gameMap.swapTiles(affectedTiles, BaseTile.TileTypes.Chasm);
            foreach (BaseTile tile in affectedTiles) {
                foreach (BaseUnit unit in gameMap.gameManager.player1.GetComponentsInChildren<BaseUnit>()) {
                    if (unit != null) {
                        if (unit.transform.position.x == tile.transform.position.x && unit.transform.position.z == tile.transform.position.z) {
                            Destroy(unit.gameObject);
                            break;
                        }
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
