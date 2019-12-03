using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Ability {

    new public void init(Map map, BaseTile origin) {
        base.init(map, origin);
        cooldown = 0;
    }

    new public void turnStart() {
        base.turnStart();
        if (cooldown <= 0) {
            gameMap.swapTile(originPos, BaseTile.TileTypes.Bridge);
            Destroy(gameObject);
        }
    }
}
