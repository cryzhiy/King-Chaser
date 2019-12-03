using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBeaver : Ability {

    new public void init(Map map, BaseTile origin) {
        base.init(map, origin);
        cooldown = 0;
    }

    new public void turnStart() {
        base.turnStart();
        if (cooldown <= 0) {
            Instantiate(gameMap.gameManager.beaver, new Vector3(originPos.x, 0.2f, originPos.z), transform.rotation, gameMap.gameManager.player1.transform);
            gameMap.tileMap[(int)originPos.z][(int)originPos.x].unitOnTile = BaseUnit.UnitTypes.Beaver;
            Destroy(gameObject);
        }
    }
}