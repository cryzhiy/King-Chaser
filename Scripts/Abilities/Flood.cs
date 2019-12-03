using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : Ability {

    new public void init(Map map, BaseTile origin) {
        base.init(map, origin);
        cooldown = 2;

        getAffected();
    }

    public void getAffected() {
        foreach (BaseTile tile in gameMap.getTilesWithin(2, originPos)) {
            if (tile.tileType == BaseTile.TileTypes.Grass) {
                affectedTiles.Add(tile);
            }
        }
    }

    new public void turnStart() {
        base.turnStart();
        if (cooldown <= 0) {
            gameMap.swapTiles(affectedTiles, BaseTile.TileTypes.River);
            Destroy(gameObject);
        }
    }
}
