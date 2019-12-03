using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour {
    public bool current = false;
    public bool target = false;
    public bool selectable = false;

    public List<BaseTile> adjacencyList = new List<BaseTile>();

    public bool visited = false;
    public BaseTile parent = null;
    public float distance = 0;

    // Enumerator containing the types of tiles to fill the map
    public enum TileTypes { Grass, Grass2, Mountain, River, EndZone, Chasm, Bridge, Ice };

    // Reference to the base GameManger script
    public GameManager gameManager;

    // The default tile type
    public TileTypes tileType = TileTypes.Grass;
    // The list position of the tile
    public Vector2 tilePos = new Vector2(0, 0);

    // The cost of movement to move onto this tile
    public float moveCost = 1;

    public BaseUnit.UnitTypes unitOnTile;

    // 0 is not revealed, 4 is finished revealing
    public bool finishReveal = false;
    private float progress = 0;

    protected void Update() {
        if (finishReveal == false && gameObject.layer == 8) {
            Vector3 tempPos = transform.position;
            tempPos.y = -Mathf.Cos(progress * Mathf.PI) * (5 - progress);
            progress += Time.deltaTime;
            if (progress >= 5) {
                finishReveal = true;
                tempPos.y = 0;
            }
            transform.position = tempPos;
        }
    }

    // Initalisation for this tile
    public void init(Vector2 pos) {
        gameManager = transform.parent.GetComponent<Map>().gameManager;
        tilePos = pos;
    }

    public void newUnitOnTile(BaseUnit.UnitTypes unit) {
        unitOnTile = unit;
    }
    
    public void Reset() {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;
    }

    public void findNeighbors() {
        Reset();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        checkTile(Vector3.forward);
        checkTile(-Vector3.forward);
        checkTile(Vector3.right);
        checkTile(-Vector3.right);
    }

    public void checkTile(Vector3 direction) {
        Vector3 halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        direction.y += 0.25f;
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders) {
            BaseTile baseTile = item.GetComponent<BaseTile>();
            if (baseTile != null) {
                Ray ray = new Ray(transform.position, direction);
                RaycastHit hit;
                LayerMask layerMask = LayerMask.GetMask(new List<string>() { "Visible", "Hidden" }.ToArray());
                layerMask.value = ~(layerMask.value);
                // select tile under player 1 units
                //Debug.Log(gameManager);
                if (gameManager.GetComponent<BaseMovement>().selectedObject != null) {
                    if (gameManager.GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Monster) {
                        adjacencyList.Add(baseTile);
                    }
                    else if (!Physics.Raycast(ray, out hit, 1, layerMask)) {
                        adjacencyList.Add(baseTile);
                    }
                }
            }
        }
    }
}
