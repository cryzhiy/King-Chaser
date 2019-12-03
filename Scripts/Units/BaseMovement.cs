using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour {
    // List of Tiles units can move on
    List<BaseTile> selectableTiles = new List<BaseTile>();
    // List of all Tiles in game
    List<BaseTile> tiles = new List<BaseTile>();

    Stack<BaseTile> path = new Stack<BaseTile>();

    // Tile unit is sitting on
    BaseTile currentTile;
    // Tile unit has selected/travelling to
    BaseTile targetTile;

    // Unit that player has selected
    public BaseUnit selectedObject;
    // Gamemap conatining all tiles
    private Map gameMap;

    // A unit has been selected
    public bool unitSelected = false;

    // The cost of the current path
    float pathCost = 0;

    void Update() {
        if (gameMap.gameManager.paused == false) {
            if (unitSelected) {
                if (gameMap.gameManager.selectedAbility == Ability.Abilities.None) {
                    if (!selectedObject.moving) {
                        removeSelectableTiles();
                        selectUnit();
                        findSelectableTiles();
                        clickTile();
                    }
                }
                else {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit)) {
                        BaseTile checkBaseTile = hit.collider.GetComponent<BaseTile>();
                        if (checkBaseTile != null) {
                            LineRenderer line = GetComponent<LineRenderer>();
                            bool valid = false;
                            Vector3 tempPos = checkBaseTile.transform.position;
                            tempPos.y = selectedObject.transform.position.y;

                            if (gameMap.gameManager.selectedAbility == Ability.Abilities.Bridge) {
                                if (Vector3.Distance(selectedObject.transform.position, tempPos) <= gameMap.visionRange) {
                                    if (checkBaseTile.tileType == BaseTile.TileTypes.River) {
                                        if (checkBaseTile.gameObject.layer == 8) {
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            else if (gameMap.gameManager.selectedAbility == Ability.Abilities.Flatten) {
                                if (Vector3.Distance(selectedObject.transform.position, tempPos) <= gameMap.visionRange) {
                                    if (checkBaseTile.tileType == BaseTile.TileTypes.Mountain) {
                                        if (checkBaseTile.gameObject.layer == 8) {
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            else if (gameMap.gameManager.selectedAbility == Ability.Abilities.SpawnBeaver) {
                                if (Vector3.Distance(selectedObject.transform.position, tempPos) <= gameMap.visionRange) {
                                    if (checkBaseTile.tileType == BaseTile.TileTypes.Grass) {
                                        if (checkBaseTile.gameObject.layer == 8) {
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            else if (gameMap.gameManager.selectedAbility == Ability.Abilities.SpawnElephant) {
                                if (Vector3.Distance(selectedObject.transform.position, tempPos) <= gameMap.visionRange) {
                                    if (checkBaseTile.tileType == BaseTile.TileTypes.Grass) {
                                        if (checkBaseTile.gameObject.layer == 8) {
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            else if (gameMap.gameManager.selectedAbility == Ability.Abilities.SpawnHawk) {
                                if (Vector3.Distance(selectedObject.transform.position, tempPos) <= gameMap.visionRange) {
                                    if (checkBaseTile.tileType == BaseTile.TileTypes.Grass) {
                                        if (checkBaseTile.gameObject.layer == 8) {
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            else if (gameMap.gameManager.selectedAbility == Ability.Abilities.Earthquake || gameMap.gameManager.selectedAbility == Ability.Abilities.FlashFreeze) {
                                valid = true;
                            }
                            else if (gameMap.gameManager.selectedAbility == Ability.Abilities.Flood) {
                                if (checkBaseTile.tileType == BaseTile.TileTypes.River) {
                                    valid = true;
                                }
                            }
                            if (valid == true) {
                                line.material.color = new Color(0, 0, 1);
                            }
                            else {
                                line.material.color = new Color(1, 0, 0);
                            }


                            line.positionCount = 2;
                            line.SetPosition(0, selectedObject.transform.position);
                            line.SetPosition(1, tempPos);
                        }
                    }

                }
            }
            else {
                selectUnit();
            }
        }
    }

    public void init() {
        // When it launches finds all tiles and put them in a list
        gameMap = GetComponent<GameManager>().gameMap;
        tiles = new List<BaseTile>(gameMap.GetComponentsInChildren<BaseTile>());
    }

    // Get the position of the Tile that player is sitting on
    public void getCurrentTile() {
        //***** Change the king object to object in list
        currentTile = getTargetTile();
        currentTile.current = true;
    }

    // Find the postion of selected tile and return it's coordinates
    public BaseTile getTargetTile() {
        BaseTile baseTile = null;

        // Get the Tile that has been selected with its position so that it can later be used to find the path for the unit
        baseTile = gameMap.tileMap[(int)selectedObject.transform.position.z][(int)selectedObject.transform.position.x];
        // return the postion of the target tile
        return baseTile;
    }

    // Check the neighbor tiles and add them to a list
    public void computeAdjacencyLists() {
        // Create a List for all tiles adjacent to the unit
        tiles = new List<BaseTile>(gameMap.GetComponentsInChildren<BaseTile>());

        // Check each tile in the list to find the ones that are adjacent to it
        foreach (var baseTile in tiles) {
            BaseTile checkBaseTile = baseTile.GetComponent<BaseTile>();
            checkBaseTile.findNeighbors();
        }
    }

    // Create the path for the character to follow
    public void findSelectableTiles() {
        // Get all the neighbor tiles around it and the tile it is sitting on top of
        computeAdjacencyLists();
        getCurrentTile();

        // Puts the tiles into a queue
        Queue<BaseTile> process = new Queue<BaseTile>();

        // starts the queue at the current Tile so it doesnt get put into the search
        process.Enqueue(currentTile);
        currentTile.visited = true;


        while (process.Count > 0) {
            BaseTile checkBaseTile = process.Dequeue();
            GameManager gameManager = GetComponent<GameManager>();
            Player player1 = gameManager.player1;
            Player player2 = gameManager.player2;

            selectableTiles.Add(checkBaseTile);

            if (checkBaseTile.unitOnTile == BaseUnit.UnitTypes.None || selectedObject.unitType == BaseUnit.UnitTypes.Monster) {
                checkBaseTile.selectable = true;
            }

            if (selectedObject.unitType == BaseUnit.UnitTypes.King || selectedObject.unitType == BaseUnit.UnitTypes.Hawk || selectedObject.unitType == BaseUnit.UnitTypes.Elephant || selectedObject.unitType == BaseUnit.UnitTypes.Beaver) {
                if (checkBaseTile.distance <= player1.ap * selectedObject.moveAmount) {
                    foreach (BaseTile baseTile in checkBaseTile.adjacencyList) {
                        if (!baseTile.visited) {
                            baseTile.parent = checkBaseTile;
                            baseTile.visited = true;
                            if (selectedObject.unitType == BaseUnit.UnitTypes.Hawk) {
                                baseTile.distance = 2 + checkBaseTile.distance;
                            }
                            else {
                                baseTile.distance = baseTile.moveCost + checkBaseTile.distance;
                            }
                            if (baseTile.distance <= player1.ap * selectedObject.moveAmount) {
                                process.Enqueue(baseTile);
                            }
                        }
                    }
                }
            }
            else if (selectedObject.unitType == BaseUnit.UnitTypes.Monster) {
                if (checkBaseTile.distance <= player2.ap * selectedObject.moveAmount) {
                    foreach (BaseTile baseTile in checkBaseTile.adjacencyList) {
                        if (!baseTile.visited) {
                            baseTile.parent = checkBaseTile;
                            baseTile.visited = true;
                            baseTile.distance = 2 + checkBaseTile.distance;
                            if (baseTile.distance <= player2.ap * selectedObject.moveAmount) {
                                process.Enqueue(baseTile);
                            }
                        }
                    }
                }
            }
        }
    }

    // Unit follows path to tile
    public void moveToTile(BaseTile baseTile) {
        GameManager gameManager = GetComponent<GameManager>();
        Player player1 = gameManager.player1;
        Player player2 = gameManager.player2;

        path.Clear();
        pathCost = 0;
        baseTile.target = true;

        // Looks for new tile in the "path" each time it travels to a tile
        BaseTile next = baseTile;

        while (next != null) {
            if (path.Count != 0) {
                if (next.moveCost == Mathf.Infinity) {
                    pathCost = 2;
                }
                if (selectedObject.unitType == BaseUnit.UnitTypes.Monster || selectedObject.unitType == BaseUnit.UnitTypes.Hawk) {
                    pathCost += 2 / selectedObject.moveAmount;
                }
                else {
                    pathCost += next.moveCost / selectedObject.moveAmount;
                }

            }
            path.Push(next);
            next = next.parent;
        }
        if (selectedObject.unitType == BaseUnit.UnitTypes.King || selectedObject.unitType == BaseUnit.UnitTypes.Hawk || selectedObject.unitType == BaseUnit.UnitTypes.Elephant || selectedObject.unitType == BaseUnit.UnitTypes.Beaver) {
            player1.useAP(pathCost);
        }
        else if (selectedObject.unitType == BaseUnit.UnitTypes.Monster) {
            player2.useAP(pathCost);
        }
        gameManager.updateHUD();

        selectedObject.initMove(path, targetTile, currentTile);
    }

    // Shows where the player will go and where they will place their ability
    void displayPath(BaseTile targetTile) { 
        LineRenderer line = GetComponent<LineRenderer>();
        line.material.color = new Color(0, 1, 0);

        path.Clear();
        pathCost = 0;
        targetTile.target = true;

        // Looks for new tile in the "path" each time it travels to a tile
        BaseTile next = targetTile;

        while (next != null) {
            if (path.Count != 0) {
                pathCost += next.moveCost / selectedObject.moveAmount;
            }
            path.Push(next);
            next = next.parent;
        }

        List<BaseTile> tempList = new List<BaseTile>(path);
        line.positionCount = tempList.Count;
        for (int i = 0; i< path.Count; i++) { 
            Vector3 tempPos = tempList[i].transform.position;
            tempPos.y = selectedObject.transform.position.y;
            line.SetPosition(i, tempPos);
        }
    }

    // Removes list of selectable tiles once its finsihed moving to a new current tile
    protected void removeSelectableTiles() {
        // changes the position of the current tile
        if (currentTile != null) {
            currentTile.current = false;
            currentTile = null;
        }
        // resets selectable tiles from the List
        foreach (BaseTile baseTile in selectableTiles) {
            baseTile.Reset();
        }
        // resets selectable tiles on objects
        selectableTiles.Clear();
    }
    
    // Check the postion of the mouse and if it clicks on a tile select it so the player cna move to it 
    void clickTile() {
        // This if statement is required otherwise units and tile can be selected though the UI
        if (gameMap.gameManager.buttonSelectedFrame == false) {
            // check if left Mouse button has been clicked
            if (Input.GetMouseButton(0)) {
                // find the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    BaseTile checkBaseTile = hit.collider.GetComponent<BaseTile>();

                    if (checkBaseTile != null) {
                        displayPath(checkBaseTile);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0)) {
                GetComponent<LineRenderer>().positionCount = 0;

                // find the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    BaseTile checkBaseTile = hit.collider.GetComponent<BaseTile>();
                    
                    if (checkBaseTile != null) {
                        if (selectedObject.unitType != BaseUnit.UnitTypes.Monster) {
                            if (selectedObject.unitType == BaseUnit.UnitTypes.Hawk) {
                                if (checkBaseTile.gameObject.layer == 8) {
                                    if (checkBaseTile.finishReveal == true) {
                                        if (checkBaseTile.selectable) {
                                            targetTile = checkBaseTile;
                                            moveToTile(checkBaseTile);
                                        }
                                        else {
                                            gameMap.gameManager.flashAPBar(true);
                                        }
                                    }
                                }
                            }
                            else if (checkBaseTile.tileType != BaseTile.TileTypes.Mountain && checkBaseTile.tileType != BaseTile.TileTypes.Chasm) {
                                if (checkBaseTile.gameObject.layer == 8) {
                                    if (checkBaseTile.finishReveal == true) {
                                        if (checkBaseTile.selectable) {
                                            targetTile = checkBaseTile;
                                            moveToTile(checkBaseTile);
                                        }
                                        else {
                                            gameMap.gameManager.flashAPBar(true);
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            if (checkBaseTile.selectable) {
                                targetTile = checkBaseTile;
                                moveToTile(checkBaseTile);
                            }
                            else {
                                gameMap.gameManager.flashAPBar(true);
                            }
                        }
                    }
                }
            }
        }
    }

    // Click on a unit in the list of units, and make the unit "selected"
    public void selectUnit() {
        // This if statement is required otherwise units and tile can be selected though the UI
        if (gameMap.gameManager.buttonSelectedFrame == false) {
            // if player turn...
            if (Input.GetMouseButtonDown(0)) {
                // Find the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // if raycast player collider set unit selected to be true
                if (Physics.Raycast(ray, out hit)) {
                    BaseUnit baseUnit = hit.collider.GetComponent<BaseUnit>();
                    if (baseUnit != null) {
                        GetComponent<LineRenderer>().positionCount = 0;
                        // if it is player 1's turn and they select one of player 1's units, make unit slected = true
                        if (gameMap.gameManager.playerTurn == 1) {
                            if (baseUnit.unitType == BaseUnit.UnitTypes.King || baseUnit.unitType == BaseUnit.UnitTypes.Beaver || baseUnit.unitType == BaseUnit.UnitTypes.Elephant || baseUnit.unitType == BaseUnit.UnitTypes.Hawk) {
                                selectedObject = baseUnit;
                                gameMap.gameManager.updateHUD();
                                unitSelected = true;
                                selectedObject.GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        // if it is player 2's turn and they select one of player 2's units, make unit slected = true
                        else if (gameMap.gameManager.playerTurn == 2) {
                            if (baseUnit.unitType == BaseUnit.UnitTypes.Monster) {
                                selectedObject = baseUnit;
                                unitSelected = true;
                                selectedObject.GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        else {
                            selectedObject = null;
                            unitSelected = false;
                        }
                    }
                }
            }
        }
    }
}