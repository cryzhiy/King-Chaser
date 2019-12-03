using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneTile : BaseTile {
	
	new void Update () {
        base.Update();

        if (unitOnTile == BaseUnit.UnitTypes.King) {
            gameManager.player1Win();
        }
	}
}
