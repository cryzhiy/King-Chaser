using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasmTile : BaseTile {

    new void Update() {
        base.Update();

        if (unitOnTile == BaseUnit.UnitTypes.King) {
            gameManager.player2Win();
        }
    }
}
