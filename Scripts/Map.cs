using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {    
    // Enumerator representing each edge of the map
    enum MapEdges { Top, Bottom, Left, Right};

    // Refernce to the base gameManger class
    public GameManager gameManager;

    // A list of all the grass tile objects
    public List<GameObject> grassTiles;
    // A list of all the mountain tile objects
    public List<GameObject> mountainTiles;
    // A list of all the river tile objects
    public List<GameObject> riverTiles;
    // A list of all the end zone tile objects
    public List<GameObject> endZoneTiles;
    // A list of all the chasm tile objects
    public GameObject chasmTile;
    // A list of all the bridge tile objects
    public List<GameObject> bridgeTiles;
    // A list of all the ice tile objects
    public GameObject iceObject;
    // A list of all the starting tile objects
    public List<GameObject> startingTiles;

    // A 2d list contiaining the map tiles
    [HideInInspector]
    public List<List<BaseTile>> tileMap = new List<List<BaseTile>>();
    // A 2d list containing the id for each tile that the map should have
    private List<List<BaseTile.TileTypes>> idMap = new List<List<BaseTile.TileTypes>>();

    // The range of vision the King unit had
    public int visionRange = 5;

    // The spawn location for player 1
    [HideInInspector]
    public Vector3 spawnPos = new Vector3();

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The map values
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // This boolean determines if the map will update when values have changed
    // ***** This should be set to false for any builds *****
    public bool debugMode = true;

    // The length of the square map
    public int mapSize = 35;

    // Changing this value updates the map
    public bool update = false;
    private bool updateLast = false;

    // The spacing between each tile
    public float tileSpacing = 0.0f;
    private float tileSpacingLast = 0.0f;

    // ***** Mountains

    // List of all the mountains
    //private List<Vector3> mountList = new List<Vector3>();

    // The number of initial mountain ranges the map will generate
    public int numMountRanges = 10;
    private int numMountRangesLast;

    // The base size of the mountain range
    public int baseRangeSize = 10;
    private int baseRangeSizeLast;

    // The number of additional mountains added to each mountain range
    public int additionalMounts = 5;
    private int additionalMountsLast;

    // ***** Rivers
    // List of all the rivers
    // ** Potentially change this to be a list of lists, each list containing a seperate river
    //private List<Vector3> riverList = new List<Vector3>();

    // The number of rivers
    public int riverSources = 1;
    private int riverSourcesLast;

    // The amount the river deviates from a straight line
    public int riverDeviation = 5;
    private int riverDeviationLast;

    // The thickness of the rivers
    public int riverThickness = 3;
    private int riverThicknessLast;


    void Start () {
        // Sets initial values
        updateLast = update;
        tileSpacingLast = tileSpacing;

        numMountRangesLast = numMountRanges;
        baseRangeSizeLast = baseRangeSize;
        additionalMountsLast = additionalMounts;

        riverSourcesLast = riverSources;
        riverDeviationLast = riverDeviation;
    }

    void Update() {
        if (debugMode == true) {
        // If any values have changed
            if (update != updateLast || tileSpacing != tileSpacingLast ||
                numMountRanges != numMountRangesLast || baseRangeSize != baseRangeSizeLast || additionalMounts != additionalMountsLast ||
                riverSources != riverSourcesLast || riverDeviation != riverDeviationLast || riverThickness != riverThicknessLast) {

                // Update Values
                updateLast = update;
                tileSpacingLast = tileSpacing;

                numMountRangesLast = numMountRanges;
                baseRangeSizeLast = baseRangeSize;
                additionalMountsLast = additionalMounts;

                riverSourcesLast = riverSources;
                riverDeviationLast = riverDeviation;
                riverThicknessLast = riverThickness;

                generateMap();
            }
        }
    }

    // Clears and empties each map tile and id
    void clearMap() {
        if (tileMap.Count != 0) {
            foreach (List<BaseTile> row in tileMap) {
                foreach (BaseTile tile in row) {
                    if (tile != null) {
                        Destroy(tile.gameObject);
                    }
                }
            }
        }
        tileMap.Clear();
        idMap.Clear();
    }

    // Calls the functions to clear then generate a map
    public void generateMap() {
        // Clear Map
        clearMap();

        if (GetComponent<ManualMapGen>() != null) {
            GetComponent<ManualMapGen>().assignManualMap();
        }
        else {
            // Generate Map
            createidMap();
            // Create map gameObjects
            setSpawnPoints();
            createMap();
        }
    }

    // Creates the id for map tiles
    void createidMap() {
        // Fills out the tile base ids with grass
        for (int y = 0; y < mapSize; y++) {
            idMap.Add(new List<BaseTile.TileTypes>());
            for (int x = 0; x < mapSize; x++) {
                idMap[y].Add(BaseTile.TileTypes.Grass);
            }
        }

        // Generates the mountains
        genMounts();
        // Generates the rivers
        genRivers();
        // Generate start/end areas
        genZones();
        // Vary grass
        genGrass2();
    }

    // Assigns the mountain ids
    void genMounts() {
        // The max number of times a random generation will loop before giving up
        int maxLoop = 1000;
        int loop = 0;

        // ***** Initial Mountain Point *****
        // The fourth item is the radius
        List<Vector4> circleList = new List<Vector4>();
        // Stops mountains forming where the player/end zone can spawn
        circleList.Add(new Vector4(0, 0, 0, 5));
        circleList.Add(new Vector4(mapSize, 0, 0, 5));
        circleList.Add(new Vector4(0, 0, mapSize, 5));
        circleList.Add(new Vector4(mapSize, 0, mapSize, 5));
        for (int m = 0; m < numMountRanges; m++) {
            Vector3 origin = new Vector3();

            bool keepLoop = false;

            // The base circle size for the random generation of mountain sources
            int circleSize = 2;

            // Loops to ensure random generation doesn't go on forever
            for (loop = 0; loop < maxLoop; loop++) {
                keepLoop = false;
                origin = new Vector3(Random.Range(0, mapSize), 0, Random.Range(0, mapSize));
                circleSize = 2 + (numMountRanges - m);

                for (int circle = 0; circle < circleList.Count; circle++) {
                    // Scale is diameter

                    if (Vector3.Distance(new Vector3 (circleList[circle].x, circleList[circle].y, circleList[circle].z), origin) < circleList[circle].w / 2 + circleSize / 2) {
                        keepLoop = true;
                        break;
                    }
                }
                if (keepLoop == false) {
                    break;
                }
            }

            if (loop < maxLoop) {
                circleList.Add(new Vector4(origin.x, origin.y, origin.z, circleSize));
            }
        }

        for (int i = 4; i < circleList.Count; i++) {
            Vector4 circle = circleList[i];
            Vector2 randPos = Random.insideUnitCircle * circle.w;
            Vector3 origin = new Vector3(circle.x, circle.y, circle.z) + new Vector3(randPos.x, 0, randPos.y);
            origin = clampToBoard(origin);

            idMap[(int)origin.z][(int)origin.x] = BaseTile.TileTypes.Mountain;

            // ***** Initial Mountain Point *****
            // ***** Initial Mountain Line *****
            List<Vector3> mountList = new List<Vector3>();
            mountList.Add(origin);

            randPos = Random.insideUnitCircle;
            Vector3 direc = new Vector3(randPos.x, 0, randPos.y);
            for (int m = 0; m < baseRangeSize; m++) {
                origin += direc;
                if (onBoard(origin) == false) {
                    break;
                }
                if (idMap[(int)origin.z][(int)origin.x] == BaseTile.TileTypes.Grass) {
                    idMap[(int)origin.z][(int)origin.x] = BaseTile.TileTypes.Mountain;
                    mountList.Add(origin);
                    // ***** Initial Mountain Line *****
                    // ***** Additional Mountains *****
                    for (int a = 0; a < additionalMounts; a++) {
                        Vector3 pos = mountList[Random.Range(0, mountList.Count - 1)];
                        List<Vector3> adjTiles = getAdj(pos);

                        while (adjTiles.Count > 0) {
                            Vector3 tempPos = adjTiles[Random.Range(0, adjTiles.Count - 1)];
                            if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                                idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Mountain;
                                break;
                            }
                            else {
                                adjTiles.Remove(tempPos);
                            }
                        }
                    }
                    // ***** Additional Mountains *****
                }
            }
        }
    }

    // Assigns the river ids
    void genRivers() {
        List<Vector3> edges = getEdgeList(new List<MapEdges> { MapEdges.Bottom, MapEdges.Top }, false);
        Vector3 origin;
        Vector3 direc;
        Vector3 tempPos;
        List<Vector3> riverList = new List<Vector3>();
        for (int r = 0; r < riverSources; r++) {
            origin = edges[Random.Range(0, edges.Count)];
            tempPos = origin;

            direc = origin - new Vector3(mapSize / 2, 0, mapSize / 2);
            direc *= -1;
            direc = scaleDown(direc);

            while (onBoard(tempPos)) {
                if (idMap[(int)tempPos.z][(int)tempPos.x] != BaseTile.TileTypes.River) {
                    idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.River;
                    riverList.Add(tempPos);

                    Vector3 target = tempPos + direc * 10;
                    Vector2 rand = Random.insideUnitCircle * riverDeviation;
                    target += new Vector3(rand.x, 0, rand.y);
                    direc = target - tempPos;
                    direc = scaleDown(direc);
                }
                addThickness(tempPos);

                tempPos += direc;
            }
            if (riverList.Count < mapSize) {
                if (riverList.Count < mapSize * 0.5f) {
                    // Create another river
                    origin = edges[Random.Range(0, edges.Count)];

                    direc = origin - new Vector3(mapSize / 2, 0, mapSize / 2);
                    direc *= -1;
                    direc = scaleDown(direc);
                }
                else {
                    // Create split off
                    origin = riverList[riverList.Count / 2];
                    if (Random.Range(0, 2) == 0) {
                        direc.x *= -1;
                    }
                    else {
                        direc.z *= -1;
                    }
                }
                tempPos = origin;
                while (onBoard(tempPos)) {
                    if (idMap[(int)tempPos.z][(int)tempPos.x] != BaseTile.TileTypes.River) {
                        idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.River;
                        riverList.Add(tempPos);

                        Vector3 target = tempPos + direc * 10;
                        Vector2 rand = Random.insideUnitCircle * riverDeviation;
                        target += new Vector3(rand.x, 0, rand.y);
                        direc = target - tempPos;
                        direc = scaleDown(direc);
                    }
                    addThickness(tempPos);

                    tempPos += direc;
                }
            }
            riverList.Clear();
        }
    }

    // Returns a random direction vector which is in an forward direction
    Vector3 randomiseDirection(Vector3 origin, Vector3 direc) {
        Vector3 output = direc;

        Vector3 target = origin + output * 10;
        Vector2 bob = Random.insideUnitCircle * riverDeviation;
        target += new Vector3(bob.x, 0, bob.y);
        output = target - origin;
        output = scaleDown(direc);

        return output;
    }

    // Adds more river tiles on to a position to give that river a thickness
    void addThickness(Vector3 origin) {
        for (int x = 0; x < riverThickness; x++) {
            for (int y = 0; y < riverThickness; y++) {
                origin.y += 1;
                if (onBoard(origin)) {
                    idMap[(int)origin.z][(int)origin.x] = BaseTile.TileTypes.River;
                }
                else {
                    break;
                }
            }
            origin.x += 1;
            if (onBoard(origin)) {
                idMap[(int)origin.z][(int)origin.x] = BaseTile.TileTypes.River;
            }
            else {
                break;
            }
        }
    }

    // Adds the end zone tiles and decides where the starting location for the player will be
    void genZones() {
        foreach (Vector3 tile in getIdsWithin(3, new Vector3(mapSize - (1.5f + tileSpacing / 2), 0, mapSize / 2))){
            if (idMap[(int)tile.z][(int)tile.x] == BaseTile.TileTypes.Mountain) {
                idMap[(int)tile.z][(int)tile.x] = BaseTile.TileTypes.Grass;
            }
        }
        idMap[mapSize/2][mapSize-1] = BaseTile.TileTypes.EndZone;
    }

    // Sets the spawn point for the player
    void setSpawnPoints() {
        List<Vector3> spawnArea1 = new List<Vector3>();
        float spawnArea1Score = 0;
        List<Vector3> spawnArea2 = new List<Vector3>();
        float spawnArea2Score = 0;

        for (int y = 0; y < 5; y++) {
            for (int x = 0; x< 5; x++) {
                spawnArea1.Add(new Vector3(x * (1 + tileSpacing), 0, y * (1 + tileSpacing)));
                if (idMap[y][x] == BaseTile.TileTypes.River) {
                    spawnArea1Score += 3;
                }
                else if (idMap[y][x] == BaseTile.TileTypes.Mountain) {
                    spawnArea1Score += 5;
                }
            }
        }

        for (int y = 34; y > 29; y--) {
            for (int x = 0; x < 5; x++) {
                spawnArea2.Add(new Vector3(x * (1 + tileSpacing), 0, y * (1 + tileSpacing)));
                if (idMap[y][x] == BaseTile.TileTypes.River) {
                    spawnArea2Score += 3;
                }
                else if (idMap[y][x] == BaseTile.TileTypes.Mountain) {
                    spawnArea2Score += 5;
                }
            }
        }

        if (spawnArea1Score <= spawnArea2Score) {
            // Spawn at bottom left
            List<Vector3> tempList = new List<Vector3>();
            foreach (Vector3 pos in spawnArea1) {
                if (idMap[(int)pos.z][(int)pos.x] == BaseTile.TileTypes.Grass) {
                    tempList.Add(pos);
                }
            }

            spawnPos = tempList[Random.Range(0, tempList.Count)];
            foreach (BaseUnit unit in gameManager.GetComponentsInChildren<BaseUnit>()) {
                if (unit.unitType == BaseUnit.UnitTypes.King) {
                    spawnPos.y = unit.transform.position.y;
                    unit.transform.position = spawnPos;
                }
                else if (unit.unitType == BaseUnit.UnitTypes.Monster) {
                    Vector3 temp = new Vector3(spawnPos.x, unit.transform.position.y, 30 + spawnPos.z);
                    temp = clampToBoard(temp);
                    unit.transform.position = temp;
                }
            }
        }
        else {
            // Spawn at top left
            List<Vector3> tempList = new List<Vector3>();
            foreach (Vector3 pos in spawnArea1) {
                if (idMap[(int)pos.z][(int)pos.x] == BaseTile.TileTypes.Grass) {
                    tempList.Add(pos);
                }
            }

            spawnPos = tempList[Random.Range(0, tempList.Count)];
            foreach (BaseUnit unit in gameManager.GetComponentsInChildren<BaseUnit>()) {
                if (unit.unitType == BaseUnit.UnitTypes.King) {
                    spawnPos.y = unit.transform.position.y;
                    unit.transform.position = spawnPos;
                }
                else if (unit.unitType == BaseUnit.UnitTypes.Monster) {
                    Vector3 temp = new Vector3(spawnPos.x, unit.transform.position.y, 30 - spawnPos.z);
                    temp = clampToBoard(temp);
                    unit.transform.position = temp;
                }
            }
            gameManager.cam1.transform.position = new Vector3(gameManager.cam1.transform.position.x, gameManager.cam1.transform.position.y, gameManager.cam1.transform.position.z + 30);
        }
    }

    // Assigns the varied grass tiles
    void genGrass2() {
        int centre = 10;
        int middle = 5;
        int outer = 1;

        Vector3 botLeft = new Vector3(Random.Range(0, 17), 0, Random.Range(0, 17));

        // Assings a different grass tile in clumps at the bottom left
        botLeft = clampToBoard(botLeft);
        if (idMap[(int)botLeft.z][(int)botLeft.x] == BaseTile.TileTypes.Grass) {
            idMap[(int)botLeft.z][(int)botLeft.x] = BaseTile.TileTypes.Grass2;
        }
        for (int i = 0; i < centre; i++) {
            Vector3 tempPos = botLeft;
            Vector3 randDirec = Random.insideUnitSphere;
            randDirec.x += Random.Range(-1, 1);
            randDirec.y = 0;
            randDirec.z += Random.Range(-1, 1);
            tempPos += randDirec;
            tempPos = clampToBoard(tempPos);
            if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
            }
            for (int j = 0; j < middle; j++) {
                tempPos = botLeft;
                randDirec = Random.insideUnitSphere;
                randDirec.x += Random.Range(-3, 3);
                randDirec.y = 0;
                randDirec.z += Random.Range(-3, 3);
                tempPos += randDirec;
                tempPos = clampToBoard(tempPos);
                if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                    idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                }
                for (int k = 0; k < outer; k++) {
                    tempPos = botLeft;
                    randDirec = Random.insideUnitSphere;
                    randDirec.x += Random.Range(-5, 5);
                    randDirec.y = 0;
                    randDirec.z += Random.Range(-5, 5);
                    tempPos += randDirec;
                    tempPos = clampToBoard(tempPos);
                    if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                        idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                    }
                }
            }
        }

        // Assings a different grass tile in clumps at the bottom right
        Vector3 botRight = new Vector3(Random.Range(18, 35), 0, Random.Range(0, 17));
        botRight = clampToBoard(botRight);
        if (idMap[(int)botRight.z][(int)botRight.x] == BaseTile.TileTypes.Grass) {
            idMap[(int)botRight.z][(int)botRight.x] = BaseTile.TileTypes.Grass2;
        }
        for (int i = 0; i < centre; i++) {
            Vector3 tempPos = botRight;
            Vector3 randDirec = Random.insideUnitSphere;
            randDirec.x += Random.Range(-1, 1);
            randDirec.y = 0;
            randDirec.z += Random.Range(-1, 1);
            tempPos += randDirec;
            tempPos = clampToBoard(tempPos);
            if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
            }
            for (int j = 0; j < middle; j++) {
                tempPos = botRight;
                randDirec = Random.insideUnitSphere;
                randDirec.x += Random.Range(-3, 3);
                randDirec.y = 0;
                randDirec.z += Random.Range(-3, 3);
                tempPos += randDirec;
                tempPos = clampToBoard(tempPos);
                if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                    idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                }
                for (int k = 0; k < outer; k++) {
                    tempPos = botRight;
                    randDirec = Random.insideUnitSphere;
                    randDirec.x += Random.Range(-5, 5);
                    randDirec.y = 0;
                    randDirec.z += Random.Range(-5, 5);
                    tempPos += randDirec;
                    tempPos = clampToBoard(tempPos);
                    if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                        idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                    }
                }
            }
        }

        // Assings a different grass tile in clumps at the top left
        Vector3 topLeft = new Vector3(Random.Range(0, 17), 0, Random.Range(18, 35));
        topLeft = clampToBoard(topLeft);
        if (idMap[(int)topLeft.z][(int)topLeft.x] == BaseTile.TileTypes.Grass) {
            idMap[(int)topLeft.z][(int)topLeft.x] = BaseTile.TileTypes.Grass2;
        }
        for (int i = 0; i < centre; i++) {
            Vector3 tempPos = topLeft;
            Vector3 randDirec = Random.insideUnitSphere;
            randDirec.x += Random.Range(-1, 1);
            randDirec.y = 0;
            randDirec.z += Random.Range(-1, 1);
            tempPos += randDirec;
            tempPos = clampToBoard(tempPos);
            if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
            }
            for (int j = 0; j < middle; j++) {
                tempPos = topLeft;
                randDirec = Random.insideUnitSphere;
                randDirec.x += Random.Range(-3, 3);
                randDirec.y = 0;
                randDirec.z += Random.Range(-3, 3);
                tempPos += randDirec;
                tempPos = clampToBoard(tempPos);
                if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                    idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                }
                for (int k = 0; k < outer; k++) {
                    tempPos = topLeft;
                    randDirec = Random.insideUnitSphere;
                    randDirec.x += Random.Range(-5, 5);
                    randDirec.y = 0;
                    randDirec.z += Random.Range(-5, 5);
                    tempPos += randDirec;
                    tempPos = clampToBoard(tempPos);
                    if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                        idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                    }
                }
            }
        }

        // Assings a different grass tile in clumps at the top right
        Vector3 topRight = new Vector3(Random.Range(18, 35), 0, Random.Range(18, 35));
        topRight = clampToBoard(topRight);
        if (idMap[(int)topRight.z][(int)topRight.x] == BaseTile.TileTypes.Grass) {
            idMap[(int)topRight.z][(int)topRight.x] = BaseTile.TileTypes.Grass2;
        }
        for (int i = 0; i < centre; i++) {
            Vector3 tempPos = topRight;
            Vector3 randDirec = Random.insideUnitSphere;
            randDirec.x += Random.Range(-1, 1);
            randDirec.y = 0;
            randDirec.z += Random.Range(-1, 1);
            tempPos += randDirec;
            tempPos = clampToBoard(tempPos);
            if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
            }
            for (int j = 0; j < middle; j++) {
                tempPos = topRight;
                randDirec = Random.insideUnitSphere;
                randDirec.x += Random.Range(-3, 3);
                randDirec.y = 0;
                randDirec.z += Random.Range(-3, 3);
                tempPos += randDirec;
                tempPos = clampToBoard(tempPos);
                if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                    idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                }
                for (int k = 0; k < outer; k++) {
                    tempPos = topRight;
                    randDirec = Random.insideUnitSphere;
                    randDirec.x += Random.Range(-5, 5);
                    randDirec.y = 0;
                    randDirec.z += Random.Range(-5, 5);
                    tempPos += randDirec;
                    tempPos = clampToBoard(tempPos);
                    if (idMap[(int)tempPos.z][(int)tempPos.x] == BaseTile.TileTypes.Grass) {
                        idMap[(int)tempPos.z][(int)tempPos.x] = BaseTile.TileTypes.Grass2;
                    }
                }
            }
        }
    }

    // Fills out the maps with tiles corresponding to the map ids
    void createMap() {
        for (int y = 0; y < mapSize; y++) {
            tileMap.Add(new List<BaseTile>());
            for (int x = 0; x < mapSize; x++) {
                if (x == spawnPos.x && y == spawnPos.z) {
                    int tileChoice = Random.Range(0, startingTiles.Count);
                    GameObject newTile = Instantiate(startingTiles[tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                    tileMap[y].Add(newTile.GetComponent<BaseTile>());
                    newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                    newTile.transform.Rotate(new Vector3(0, Random.Range(1,5)*90, 0));
                }
                else {
                    // Grass
                    if (idMap[y][x] == BaseTile.TileTypes.Grass) {
                        int tileChoice = Random.Range(0, 4);
                        GameObject newTile = Instantiate(grassTiles[tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                    // Forest
                    if (idMap[y][x] == BaseTile.TileTypes.Grass2) {
                        int tileChoice = Random.Range(0, 4);
                        GameObject newTile = Instantiate(grassTiles[5+tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                    // Mountain
                    else if (idMap[y][x] == BaseTile.TileTypes.Mountain) {
                        int tileChoice = Random.Range(0, mountainTiles.Count);
                        GameObject newTile = Instantiate(mountainTiles[tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                    // River
                    else if (idMap[y][x] == BaseTile.TileTypes.River) {
                        int tileChoice = Random.Range(0, riverTiles.Count);
                        GameObject newTile = Instantiate(riverTiles[tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                    // End Zone
                    else if (idMap[y][x] == BaseTile.TileTypes.EndZone) {
                        int tileChoice = Random.Range(0, endZoneTiles.Count);
                        GameObject newTile = Instantiate(endZoneTiles[tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                    // Chasm
                    else if (idMap[y][x] == BaseTile.TileTypes.Chasm) {
                        GameObject newTile = Instantiate(chasmTile, new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                    // Bridge
                    else if (idMap[y][x] == BaseTile.TileTypes.Bridge) {
                        int tileChoice = Random.Range(0, bridgeTiles.Count);
                        GameObject newTile = Instantiate(bridgeTiles[tileChoice], new Vector3(x + x * tileSpacing, 0, y + y * tileSpacing), transform.rotation, transform);
                        tileMap[y].Add(newTile.GetComponent<BaseTile>());
                        newTile.GetComponent<BaseTile>().init(new Vector2(x, y));
                        newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
                    }
                }
            }
        }
        tileMap[(int)spawnPos.z][(int)spawnPos.x].unitOnTile = BaseUnit.UnitTypes.King;
        if (spawnPos.z < 17.5f) {
            tileMap[(int)spawnPos.z + 30][(int)spawnPos.x].unitOnTile = BaseUnit.UnitTypes.Monster;
        }
        else {
            tileMap[(int)spawnPos.z-30][(int)spawnPos.x].unitOnTile = BaseUnit.UnitTypes.Monster;
        }
    }
    
    // Swaps tiles out for others
    public void swapTiles(List<BaseTile> tiles, BaseTile.TileTypes newType) {
        foreach (BaseTile tile in tiles) {
            // Chasm
            if (newType == BaseTile.TileTypes.Chasm) {
                GameObject newTile = Instantiate(chasmTile, new Vector3(tile.tilePos.x + tile.tilePos.x * tileSpacing, 0, tile.tilePos.y + tile.tilePos.y * tileSpacing), transform.rotation, transform);
                newTile.GetComponent<BaseTile>().unitOnTile = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].GetComponent<BaseTile>().unitOnTile;
                // Change the layer to visible
                foreach (Transform child in newTile.GetComponentsInChildren<Transform>()) {
                    child.gameObject.layer = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject.layer;
                }

                Destroy(tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject);
                tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x] = newTile.GetComponent<BaseTile>();
                newTile.GetComponent<BaseTile>().init(new Vector2((int)tile.tilePos.x, (int)tile.tilePos.y));
                newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
            }
            // Bridge
            else if (newType == BaseTile.TileTypes.Bridge) {
                int tileChoice = Random.Range(0, bridgeTiles.Count);
                GameObject newTile = Instantiate(bridgeTiles[tileChoice], new Vector3(tile.tilePos.x + tile.tilePos.x * tileSpacing, 0, tile.tilePos.y + tile.tilePos.y * tileSpacing), transform.rotation, transform);
                newTile.GetComponent<BaseTile>().unitOnTile = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].GetComponent<BaseTile>().unitOnTile;
                // Change the layer to visible
                foreach (Transform child in newTile.GetComponentsInChildren<Transform>()) {
                    child.gameObject.layer = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject.layer;
                }

                Destroy(tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject);
                tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x] = newTile.GetComponent<BaseTile>();
                newTile.GetComponent<BaseTile>().init(new Vector2((int)tile.tilePos.x, (int)tile.tilePos.y));
                newTile.GetComponent<BaseTile>().finishReveal = true;
            }
            // Flooded River
            else if (newType == BaseTile.TileTypes.River) {
                int tileChoice = Random.Range(0, riverTiles.Count);
                GameObject newTile = Instantiate(riverTiles[tileChoice], new Vector3(tile.tilePos.x + tile.tilePos.x * tileSpacing, 0, tile.tilePos.y + tile.tilePos.y * tileSpacing), transform.rotation, transform);
                newTile.GetComponent<BaseTile>().unitOnTile = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].GetComponent<BaseTile>().unitOnTile;
                // Change the layer to visible
                foreach (Transform child in newTile.GetComponentsInChildren<Transform>()) {
                    child.gameObject.layer = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject.layer;
                }

                Destroy(tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject);
                tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x] = newTile.GetComponent<BaseTile>();
                newTile.GetComponent<BaseTile>().init(new Vector2((int)tile.tilePos.x, (int)tile.tilePos.y));
                newTile.GetComponent<BaseTile>().finishReveal = true;
                newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
            }
            // Flattened Mountain 
            else if (newType == BaseTile.TileTypes.Grass) {
                int tileChoice = Random.Range(0, grassTiles.Count);
                GameObject newTile = Instantiate(grassTiles[tileChoice], new Vector3(tile.tilePos.x + tile.tilePos.x * tileSpacing, 0, tile.tilePos.y + tile.tilePos.y * tileSpacing), transform.rotation, transform);
                newTile.GetComponent<BaseTile>().unitOnTile = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].GetComponent<BaseTile>().unitOnTile;
                // Change the layer to visible
                foreach (Transform child in newTile.GetComponentsInChildren<Transform>()) {
                    child.gameObject.layer = tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject.layer;
                }
                Destroy(tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x].gameObject);
                tileMap[(int)tile.tilePos.y][(int)tile.tilePos.x] = newTile.GetComponent<BaseTile>();
                newTile.GetComponent<BaseTile>().init(new Vector2((int)tile.tilePos.x, (int)tile.tilePos.y));
                newTile.GetComponent<BaseTile>().finishReveal = true;
                revealTiles(tile.transform.position, 0.1f);
                newTile.transform.Rotate(new Vector3(0, Random.Range(1, 5) * 90, 0));
            }
        }
    }

    // Swaps a single tile out for another
    public void swapTile(Vector3 tilePos, BaseTile.TileTypes newType) {
        // Convert to tile
        BaseTile tile = tileMap[(int)tilePos.z][(int)tilePos.x];
        // Convert to list
        List<BaseTile> tempList = new List<BaseTile>();
        tempList.Add(tile);
        swapTiles(tempList, newType);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Helper Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Returns true if the map is correctly created
    public bool isMapComplete() {
        List<BaseTile> tempList = new List<BaseTile>(GetComponentsInChildren<BaseTile>());

        if (tempList.Count == mapSize * mapSize) {
            for (int row = 0; row < mapSize; row++) {
                tileMap.Add(new List<BaseTile>());
                for (int col = 0; col < mapSize; col++) {
                    tileMap[row].Add(tempList[0]);
                    //tempList[0].gameManager = gameManager;
                    tempList.RemoveAt(0);
                }
            }
        }
        else {
            return false;
        }

        return true;
    }

    // Returns true if the pos is located on the board
    public bool onBoard(Vector3 pos) {
        // Check X
        if (pos.x < 0 || pos.x >= mapSize) {
            return false;
        }
        // Check Y
        if (pos.z < 0 || pos.z >= mapSize) {
            return false;
        }
        return true;
    }

    // Returns a vector3 that is clamped to the board
    Vector3 clampToBoard(Vector3 position) {
        position.x = Mathf.Clamp(position.x, 0, mapSize - 1);
        position.z = Mathf.Clamp(position.z, 0, mapSize - 1);
        return position;
    }

    // Returns a list of vector3 of all the adjacent tiles to a position
    List<Vector3> getAdj(Vector3 origin, bool allowDiagonal = true) {
        List<Vector3> adj = new List<Vector3>();
        Vector3 pos;
        if (allowDiagonal == true) {
            // Top Left
            pos = new Vector3(origin.x - 1, 0, origin.z + 1);
            if (onBoard(pos) == true) {
                adj.Add(pos);
            }
            // Top Right
            pos = new Vector3(origin.x + 1, 0, origin.z + 1);
            if (onBoard(pos) == true) {
                adj.Add(pos);
            }
            // Bottom Left
            pos = new Vector3(origin.x - 1, 0, origin.z - 1);
            if (onBoard(pos) == true) {
                adj.Add(pos);
            }
            // Bottom Right
            pos = new Vector3(origin.x + 1, 0, origin.z - 1);
            if (onBoard(pos) == true) {
                adj.Add(pos);
            }
        }

        // Top
        pos = new Vector3(origin.x, 0, origin.z + 1);
        if (onBoard(pos) == true) {
            adj.Add(pos);
        }

        // Left
        pos = new Vector3(origin.x - 1, 0, origin.z);
        if (onBoard(pos) == true) {
            adj.Add(pos);
        }
        // Right
        pos = new Vector3(origin.x + 1, 0, origin.z);
        if (onBoard(pos) == true) {
            adj.Add(pos);
        }
        // Bottom
        pos = new Vector3(origin.x, 0, origin.z - 1);
        if (onBoard(pos) == true) {
            adj.Add(pos);
        }

        return adj;
    }

    // Scales down a vector to maintain a ratio but fit within -1 and 1
    public Vector3 scaleDown(Vector3 direction) {
        Vector3 output = direction;
        if (Mathf.Abs(direction.x) > 1 || Mathf.Abs(direction.z) > 1) {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z)) {
                if (direction.x == 0) {
                    output.z = Mathf.Clamp(direction.z, -1, 1);
                }
                else {
                    output.x = direction.x / direction.x;
                    output.z = direction.z / direction.x;
                }
            }
            else {
                if (direction.z == 0) {
                    output.x = Mathf.Clamp(direction.x, -1, 1);
                }
                else {
                    output.x = direction.x / direction.z;
                    output.z = direction.z / direction.z;
                }
            }
        }
        if (direction.x < 0) {
            output.x *= -1;
        }
        if (direction.z < 0) {
            output.z *= -1;
        }
        return output;
    }

    // Returns a list of vector3 of all the tiles along specified edges
    List<Vector3> getEdgeList(List<MapEdges> edges, bool includeCorners = true) {
        int cornerSize = 15;
        List<Vector3> output = new List<Vector3>();
        if (edges.Contains(MapEdges.Top)) {
            for (int i = 0; i < mapSize; i++) {
                if (i < cornerSize || i > mapSize - cornerSize) {
                    if (includeCorners == true) {
                        output.Add(new Vector3(i, 0, mapSize - 1));
                    }
                }
                else {
                    output.Add(new Vector3(i, 0, mapSize - 1));
                }
            }
        }
        if (edges.Contains(MapEdges.Bottom)) {
            for (int i = 0; i < mapSize; i++) {
                if (i < cornerSize || i > mapSize - cornerSize) {
                    if (includeCorners == true) {
                        output.Add(new Vector3(i, 0, 0));
                    }
                }
                else {
                    output.Add(new Vector3(i, 0, 0));
                }
            }
        }
        if (edges.Contains(MapEdges.Left)) {
            for (int i = 0; i < mapSize; i++) {
                if (i < cornerSize || i > mapSize - cornerSize) {
                    if (includeCorners == true) {
                        output.Add(new Vector3(0, 0, i));
                    }
                }
                else {
                    output.Add(new Vector3(0, 0, i));
                }
            }
        }
        if (edges.Contains(MapEdges.Right)) {
            for (int i = 0; i < mapSize; i++) {
                if (i < cornerSize || i > mapSize - cornerSize) {
                    if (includeCorners == true) {
                        output.Add(new Vector3(mapSize - 1, 0, i));
                    }
                }
                else {
                    output.Add(new Vector3(mapSize - 1, 0, i));
                }
            }
        }

        return output;
    }

    // Gets the map ids within certain extents
    List<Vector3> getIdsWithin(float radius, Vector3 origin) {
        List<Vector3> output = new List<Vector3>();
        for (int y = 0; y < mapSize; y++) {
            if (y >= origin.z - radius && y <= origin.z + radius) {
                for (int x = 0; x < mapSize; x++) {
                    if (x >= origin.x - radius && x <= origin.x + radius) {
                        output.Add(new Vector3(x, 0, y));
                    }
                }
            }
        }
        return output;
    }

    // Gets the tiles within a certain distance
    public List<BaseTile> getTilesWithin(float radius, Vector3 origin) {
        List<BaseTile> output = new List<BaseTile>();
        for (int y = (int)Mathf.Max(origin.z - radius, 0); y < (int)Mathf.Min(origin.z + radius, 35); y++) {
            for (int x = (int)Mathf.Max(origin.x - radius, 0); x < (int)Mathf.Min(origin.x + radius, 35); x++) {
                if (Vector3.Distance(new Vector3(x, 0, y), origin) <= radius) {
                    output.Add(tileMap[y][x]);
                }
            }
        }
        return output;
    }

    // Sets the manager
    public void setManager(GameManager manager) {
        gameManager = manager;
    }

    // Reveals tiles within the default vision range
    public void revealTiles(Vector3 position) {
        foreach (BaseTile tile in getTilesWithin(visionRange, position)) {
            if (tile.gameObject.layer != 8) {
                foreach (Transform tileObject in tile.GetComponentsInChildren<Transform>()) {
                    tileObject.gameObject.layer = 8;
                }
            }
        }
    }

    // Reveals tiles within a specified range
    public void revealTiles(Vector3 position, float radius) {
        foreach (BaseTile tile in getTilesWithin(radius, position)) {
            if (tile.gameObject.layer != 8) {
                foreach (Transform tileObject in tile.GetComponentsInChildren<Transform>()) {
                    tileObject.gameObject.layer = 8;
                }
            }
        }
    }
}