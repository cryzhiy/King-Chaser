using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMapGen : MonoBehaviour {
    public List<GameObject> row1 = new List<GameObject>();
    public List<GameObject> row2 = new List<GameObject>();
    public List<GameObject> row3 = new List<GameObject>();
    public List<GameObject> row4 = new List<GameObject>();
    public List<GameObject> row5 = new List<GameObject>();
    public List<GameObject> row6 = new List<GameObject>();
    public List<GameObject> row7 = new List<GameObject>();
    public List<GameObject> row8 = new List<GameObject>();
    public List<GameObject> row9 = new List<GameObject>();
    public List<GameObject> row10 = new List<GameObject>();
    public List<GameObject> row11 = new List<GameObject>();
    public List<GameObject> row12 = new List<GameObject>();
    public List<GameObject> row13 = new List<GameObject>();
    public List<GameObject> row14 = new List<GameObject>();
    public List<GameObject> row15 = new List<GameObject>();
    public List<GameObject> row16 = new List<GameObject>();
    public List<GameObject> row17 = new List<GameObject>();
    public List<GameObject> row18 = new List<GameObject>();
    public List<GameObject> row19 = new List<GameObject>();
    public List<GameObject> row20 = new List<GameObject>();
    public List<GameObject> row21 = new List<GameObject>();
    public List<GameObject> row22 = new List<GameObject>();
    public List<GameObject> row23 = new List<GameObject>();
    public List<GameObject> row24 = new List<GameObject>();
    public List<GameObject> row25 = new List<GameObject>();
    public List<GameObject> row26 = new List<GameObject>();
    public List<GameObject> row27 = new List<GameObject>();
    public List<GameObject> row28 = new List<GameObject>();
    public List<GameObject> row29 = new List<GameObject>();
    public List<GameObject> row30 = new List<GameObject>();
    public List<GameObject> row31 = new List<GameObject>();
    public List<GameObject> row32 = new List<GameObject>();
    public List<GameObject> row33 = new List<GameObject>();
    public List<GameObject> row34 = new List<GameObject>();
    public List<GameObject> row35 = new List<GameObject>();

    // Assigns manual map values to the map list
    public void assignManualMap() {
        Debug.Log("Generate map manually");
        Map map = GetComponent<Map>();

        int y = 0;
        int x = 0;

        for (int i = 0; i < map.mapSize; i++) {
            map.tileMap.Add(new List<BaseTile>());
        }

        foreach (GameObject tile in row1) {

            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row2) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row3) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row4) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row5) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row6) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row7) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row8) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row9) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row10) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row11) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row12) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row13) {

            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            x++;
        }
        y++;
        x = 0;
        x = 0;
        foreach (GameObject tile in row14) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row15) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row16) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row17) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row18) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row19) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row20) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row21) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row22) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row23) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row24) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row25) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row26) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row27) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row28) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row29) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row30) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row31) {

            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row32) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row33) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row34) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
        y++;
        x = 0;
        foreach (GameObject tile in row35) {

            map.tileMap[y].Add(tile.GetComponent<BaseTile>());
            tile.GetComponent<BaseTile>().init(new Vector2(x, y));
            x++;
        }
    }
}