using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {
    public enum Abilities { None, SpawnBeaver, SpawnElephant, SpawnHawk, Bridge, Flatten, Earthquake, Flood, FlashFreeze };

    protected Map gameMap;

    protected List<BaseTile> affectedTiles = new List<BaseTile>();

    protected Vector3 originPos;
    protected Vector3 direc;
    public int apCost;
    public int cooldown;

    public  void init(Map map, BaseTile origin) {
        gameMap = map;
        originPos = origin.transform.position;
    }

    public void turnStart() {
        cooldown--;
    }
}
