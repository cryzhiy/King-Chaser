using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {
    // The component holding the game's map and functions
    public Map gameMap;
    // The gameobject which is the parent of all ui objects
    public GameObject UIObject;

    // The gameobject for spawning the beaver
    public GameObject beaver;
    // The gameobject for spawning the elephant
    public GameObject elephant;
    // The gameobject for spawning the hawk
    public GameObject hawk;

    // A number representing which player's turn it is
    // 0=pregame, 1=player1, 2=player2
    [HideInInspector]
    public int playerTurn = 0;
    // A number showing how many total turns the game has been going for
    private int gameTurn = 0;

    // The object holding the player 1 script
    [HideInInspector]
    public Player player1;
    // The camera used for player 1
    [HideInInspector]
    public Camera cam1;
    // The object holding the player 2 script
    [HideInInspector]
    public Player player2;
    // The camera used for player 2
    [HideInInspector]
    private Camera cam2;

    // Whether the game should be paused or not
    [HideInInspector]
    public bool paused = false;

    // Stores whose turn the apoBar began to flash for
    private int flashTurn = 0;
    // Stroes the value used for flashing the apBar
    private float flashTimer = 0;

    // The ability which is currently selected
    public Ability.Abilities selectedAbility = Ability.Abilities.None;
    // The tile which is currentlyt selected
    public BaseTile selectedTile = null;

    // Whether a button has been selected this frame
    // Used to stop a bug
    [HideInInspector]
    public bool buttonSelectedFrame = false;

    public List<Sprite> apBarSprites = new List<Sprite>();

    Vector3 endPos;
    public GameObject arrow;

    void Start() {
        // Finds the player objects
        List<Player> tempList = new List<Player>(GetComponentsInChildren<Player>());
        if (tempList.Count != 2) {
            Debug.Log("There are not enough players in game manager's children");
        }
        else {
            foreach (Player player in tempList) {
                if (player.name.Contains("1")) {
                    player1 = player;
                }
                else if (player.name.Contains("2")) {
                    player2 = player;
                }
                else {
                    Debug.Log("The players are not named properly, they must have a 1 or 2");
                }
            }
        }
        // Finds the camera objects
        List<Camera> tempCamList = new List<Camera>(GetComponentsInChildren<Camera>());
        if (tempCamList.Count != 2) {
            Debug.Log("There are not enough cameras");
        }
        else {
            foreach (Camera camera in tempCamList) {
                if (camera.name.Contains("1")) {
                    cam1 = camera;
                }
                else if (camera.name.Contains("2")) {
                    cam2 = camera;
                }
                else {
                    Debug.Log("The cameras are not named properly, they must have a 1 or 2");
                }
            }
        }

        // Sets up the game map
        gameMap.setManager(this);
        // Game Map Setup
        if (gameMap == null) {
            Debug.Log("No game map has been assigned");
        }
        generateMap();
        endPos = gameMap.tileMap[34][35 / 2].transform.position;

        // Resets the time scale just in case
        Time.timeScale = 1;
    }

    void FixedUpdate() {
        if (paused == false) {
            if (buttonSelectedFrame == true) {
                buttonSelectedFrame = false;
            }

            // Player Zero, in between only used for game setup
            if (playerTurn == 0) {
                endTurn();
            }
            else if (playerTurn == 1) {
                if (gameMap.tileMap[(int)gameMap.spawnPos.z][(int)gameMap.spawnPos.x].gameObject.layer == 9) {
                    // Reveals the spawn location if it is not already revealled
                    gameMap.revealTiles(gameMap.spawnPos);
                    foreach (BaseTile tile in gameMap.getTilesWithin(gameMap.visionRange, new Vector3(gameMap.spawnPos.x, 0, gameMap.spawnPos.z))) {
                        tile.finishReveal = true;
                    }
                }
            }

            foreach (BaseUnit unit in player1.GetComponentsInChildren<BaseUnit>()) {
                if (unit.unitType == BaseUnit.UnitTypes.King) {
                    arrow.transform.LookAt(new Vector3(endPos.x, arrow.transform.position.y, endPos.x));
                }
            }
        }
    }

    void Update() {
        // Pausing
        if (Input.GetKeyUp(KeyCode.Escape)) {
            paused = !paused;
            if (paused == true) {
                Time.timeScale = 0;
            }
            else {
                Time.timeScale = 1;
            }
            updatePauseMenu();
        }

        if (paused == false) {
            // Misc
            flashAPBar();

            // Both Players
            if (Input.GetMouseButtonUp(0)) {
                handleClick();
            }
            else if (Input.GetMouseButtonDown(1)) {
                // Deselecting
                selectedTile = null;
                selectedAbility = Ability.Abilities.None;
                GetComponent<LineRenderer>().positionCount = 0;
                GetComponent<BaseMovement>().selectedObject = null;
                GetComponent<BaseMovement>().unitSelected = false;
                updateHUD();
            }

            // Camera Movement
            Camera.main.transform.rotation = Quaternion.Euler(0, 45, 0);
            Camera.main.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), Camera.main.transform);
            Camera.main.transform.rotation = Quaternion.Euler(45, 45, 0);

            float height = Camera.main.transform.transform.position.y;
            float top = 35 - height;
            float down = 0 - height;
            float left = 0 - height;
            float right = 35 - height;

            Vector3 tempPos = Camera.main.transform.position;
            tempPos.x = Mathf.Clamp(tempPos.x, left, right);
            tempPos.z = Mathf.Clamp(tempPos.z, down, top);

            Camera.main.transform.position = tempPos;

            if (Input.mouseScrollDelta.y != 0) {
                if ((Camera.main.transform.position.y < 10 && Input.mouseScrollDelta.y < 0) || (Camera.main.transform.position.y > 2 && Input.mouseScrollDelta.y > 0)) {
                    Vector3 scrollDelta = new Vector3(0, 0, Input.mouseScrollDelta.y);
                    Camera.main.transform.Translate(scrollDelta, Camera.main.transform);
                }
            }           
        }
    }

    // Generates the map and intialises the movement script for the map
    void generateMap() {
        gameMap.generateMap();
        GetComponent<BaseMovement>().init();
    }

    // Handles the end of turn for each player
    public void endTurn() {
        if (playerTurn == 0) {
            // Player 0 end
            playerTurn = 1;
            gameTurn++;
            // Do setup for player 1
            updateTurnText();
            updateCamera();
            updatePauseMenu();
            // THIS SHOULD BE DONE AFTER EVERY ACTION
            updateHUD();
        }
        else if (playerTurn == 1) {
            // Player 1 end
            playerTurn = 2;
            // Do setup for player 2
            player2.addTurnAP();
            resetSelections();

            updateTurn2();
            updateTurnText();
            updateCamera();
            updateHUD();
        }
        else if (playerTurn == 2) {
            // Player 2 end
            playerTurn = 1;
            gameTurn++;
            // Do setup for player 1
            player1.addTurnAP();
            resetSelections();

            activatePlayer2Abilities();

            updateTurn1();
            updateTurnText();
            updateCamera();
            updateHUD();
        }
    }

    // Activates the first of player1's abilities
    // There should only ever be one needing to be activated at a time
    void activatePlayer1Abilities() {
        foreach (Ability ability in GetComponentsInChildren<Ability>()) {
            if (ability.name.Contains("SpawnBeaver")) {
                ability.GetComponent<SpawnBeaver>().turnStart();
                break;
            }
            else if (ability.name.Contains("SpawnElephant")) {
                ability.GetComponent<SpawnElephant>().turnStart();
                break;
            }
            else if (ability.name.Contains("SpawnHawk")) {
                ability.GetComponent<SpawnHawk>().turnStart();
                break;
            }
            else if (ability.name.Contains("Bridge")) {
                ability.GetComponent<Bridge>().turnStart();
                break;
            }
            else if (ability.name.Contains("Flatten")) {
                ability.GetComponent<Flatten>().turnStart();
                break;
            }
        }
    }

    // Activates all of player1's abilities
    void activatePlayer2Abilities() {
        foreach (Ability ability in GetComponentsInChildren<Ability>()) {
            if (ability.name.Contains("Earthquake")) {
                ability.GetComponent<Earthquake>().turnStart();
            }
            else if (ability.name.Contains("Flood")) {
                ability.GetComponent<Flood>().turnStart();
            }
            else if (ability.name.Contains("Flash")) {
                ability.GetComponent<FlashFreeze>().turnStart();
            }
        }
    }

    // Handles activating UI for a player 1 win
    public void player1Win() {
        paused = true;
        foreach (Transform bob in UIObject.GetComponentsInChildren<Transform>(true)) {
            if (bob.name == "Player 1 Wins") {
                playerTurn = 0;
                updateHUD();
                bob.gameObject.SetActive(true);
            }
        }
    }

    // Handles activating UI for a player 1 win
    public void player2Win() {
        paused = true;
        foreach (Transform bob in UIObject.GetComponentsInChildren<Transform>(true)) {
            if (bob.name == "Player 2 Wins") {
                playerTurn = 0;
                updateHUD();
                bob.gameObject.SetActive(true);
            }
        }
    }

    // Function for handling clicks related to abilties
    private void handleClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        BaseTile tile;
        BaseMovement baseMovement = GetComponent<BaseMovement>();

        // Initialising the layer mask so rays only hit tiles which are valid for the player
        int layerMask = 0;
        if (playerTurn == 1) {
            // Only allows the raycast to hit visible objects
            layerMask = LayerMask.GetMask(new List<string>() { "Visible" }.ToArray());
        }
        else {
            // Allows the raycast to hit visible or hidden objects
            layerMask = LayerMask.GetMask(new List<string>() { "Visible", "Hidden" }.ToArray());
        }
        // Stops raycasts if a player actually selected a button
        if (buttonSelectedFrame == false) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                tile = hit.transform.gameObject.GetComponent<BaseTile>();
                // Was a tile clicked
                if (tile != null) {
                    // Has the tile finished reveal or is hidden
                    if (tile.finishReveal == true || tile.gameObject.layer == 9) {
                        if (playerTurn == 1) {
                            if (GetComponent<BaseMovement>().selectedObject != null) {
                                if (tile.unitOnTile == BaseUnit.UnitTypes.None) { 
                                    // Something has been selected
                                    if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.King) {
                                        // A King is selected
                                        if (selectedTile == null) {
                                            // Set selected Tile to this tile
                                            selectedTile = tile;
                                        }
                                        if (selectedTile != null) {
                                            if (Vector3.Distance(baseMovement.selectedObject.transform.position, selectedTile.transform.position) <= gameMap.visionRange) {
                                                if (selectedTile.tileType != BaseTile.TileTypes.Mountain && selectedTile.tileType != BaseTile.TileTypes.Chasm && selectedTile.tileType != BaseTile.TileTypes.River) {
                                                    if (selectedAbility == Ability.Abilities.SpawnBeaver) {
                                                        // SpawnBeaver
                                                        GameObject abilityObject = Instantiate(player1.abilities[0], new Vector3(selectedTile.tilePos.x, 1, selectedTile.tilePos.y), transform.rotation, transform);
                                                        abilityObject.GetComponent<SpawnBeaver>().init(gameMap, selectedTile);
                                                        player1.useAP(abilityObject.GetComponent<Ability>().apCost);

                                                        animateAction(GetComponent<BaseMovement>().selectedObject);
                                                        selectedAbility = Ability.Abilities.None;
                                                        GetComponent<LineRenderer>().positionCount = 0;
                                                        selectedTile = null;
                                                        GetComponent<BaseMovement>().selectedObject = null;
                                                        GetComponent<BaseMovement>().unitSelected = false;
                                                        activatePlayer1Abilities();
                                                        updateHUD();
                                                    }
                                                    else if (selectedAbility == Ability.Abilities.SpawnElephant) {
                                                        // SpawnElephant
                                                        GameObject abilityObject = Instantiate(player1.abilities[1], new Vector3(selectedTile.tilePos.x, 1, selectedTile.tilePos.y), transform.rotation, transform);
                                                        abilityObject.GetComponent<SpawnElephant>().init(gameMap, selectedTile);
                                                        player1.useAP(abilityObject.GetComponent<Ability>().apCost);

                                                        animateAction(GetComponent<BaseMovement>().selectedObject);
                                                        selectedAbility = Ability.Abilities.None;
                                                        GetComponent<LineRenderer>().positionCount = 0;
                                                        selectedTile = null;
                                                        GetComponent<BaseMovement>().selectedObject = null;
                                                        GetComponent<BaseMovement>().unitSelected = false;
                                                        activatePlayer1Abilities();
                                                        updateHUD();

                                                    }
                                                    else if (selectedAbility == Ability.Abilities.SpawnHawk) {
                                                        // SpawnHawk
                                                        GameObject abilityObject = Instantiate(player1.abilities[2], new Vector3(selectedTile.tilePos.x, 1, selectedTile.tilePos.y), transform.rotation, transform);
                                                        abilityObject.GetComponent<SpawnHawk>().init(gameMap, selectedTile);
                                                        player1.useAP(abilityObject.GetComponent<Ability>().apCost);

                                                        animateAction(GetComponent<BaseMovement>().selectedObject);
                                                        gameMap.revealTiles(selectedTile.transform.position);
                                                        selectedAbility = Ability.Abilities.None;
                                                        GetComponent<LineRenderer>().positionCount = 0;
                                                        selectedTile = null;
                                                        GetComponent<BaseMovement>().selectedObject = null;
                                                        GetComponent<BaseMovement>().unitSelected = false;
                                                        activatePlayer1Abilities();
                                                        updateHUD();

                                                    }
                                                    else if (selectedAbility != Ability.Abilities.None) {
                                                        // Set selected tile to this tile
                                                        selectedTile = tile;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // A Beaver is selected
                                    else if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Beaver) {
                                        if (selectedTile == null) {
                                            selectedTile = tile;
                                        }
                                        if (selectedTile != null) {
                                            if (Vector3.Distance(baseMovement.selectedObject.transform.position, selectedTile.transform.position) <= gameMap.visionRange) {
                                                if (selectedAbility == Ability.Abilities.Bridge) {
                                                    if (selectedTile.tileType == BaseTile.TileTypes.River || selectedTile.tileType == BaseTile.TileTypes.Chasm)
                                                    {
                                                        // Create Bridge
                                                        GameObject abilityObject = Instantiate(player1.abilities[3], new Vector3(selectedTile.tilePos.x, 1, selectedTile.tilePos.y), transform.rotation, transform);
                                                        abilityObject.GetComponent<Bridge>().init(gameMap, selectedTile);
                                                        player1.useAP(abilityObject.GetComponent<Ability>().apCost);

                                                        animateAction(GetComponent<BaseMovement>().selectedObject);
                                                        selectedAbility = Ability.Abilities.None;
                                                        GetComponent<LineRenderer>().positionCount = 0;
                                                        selectedTile = null;
                                                        GetComponent<BaseMovement>().selectedObject = null;
                                                        GetComponent<BaseMovement>().unitSelected = false;
                                                        activatePlayer1Abilities();
                                                        updateHUD();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // An elephant is selected
                                    else if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Elephant) {
                                        if (selectedTile == null) {
                                            // Set selected Tile to this tile
                                            selectedTile = tile;
                                        }
                                        if (selectedTile != null) {
                                            if (Vector3.Distance(baseMovement.selectedObject.transform.position, selectedTile.transform.position) <= gameMap.visionRange) {
                                                if (selectedAbility == Ability.Abilities.Flatten) {
                                                    if (selectedTile.tileType == BaseTile.TileTypes.Mountain) {
                                                        GameObject abilityObject = Instantiate(player1.abilities[4], new Vector3(selectedTile.tilePos.x, 1, selectedTile.tilePos.y), transform.rotation, transform);

                                                        animateAction(GetComponent<BaseMovement>().selectedObject);
                                                        abilityObject.GetComponent<Flatten>().init(gameMap, selectedTile);
                                                        player1.useAP(abilityObject.GetComponent<Ability>().apCost);

                                                        selectedAbility = Ability.Abilities.None;
                                                        GetComponent<LineRenderer>().positionCount = 0;
                                                        selectedTile = null;
                                                        GetComponent<BaseMovement>().selectedObject = null;
                                                        GetComponent<BaseMovement>().unitSelected = false;
                                                        activatePlayer1Abilities();
                                                        updateHUD();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        // Nothing is selected and so nothing happens
                                    }
                                }
                            }
                        }
                        // ***** PLAYER 2 *****
                        else if (playerTurn == 2) {
                            if (selectedTile == null) {
                                // Set selected Tile to this tile
                                selectedTile = tile;
                            }
                            if (selectedTile != null) {
                                // Earthquake ability
                                if (selectedAbility == Ability.Abilities.Earthquake) {
                                    GameObject abilityObject = Instantiate(player2.abilities[0], new Vector3(selectedTile.tilePos.x, 0, selectedTile.tilePos.y), transform.rotation, transform);
                                    abilityObject.GetComponent<Earthquake>().init(gameMap, selectedTile, tile.transform.position - selectedTile.transform.position);
                                    player2.useAP(abilityObject.GetComponent<Ability>().apCost);

                                    animateAction(GetComponent<BaseMovement>().selectedObject);
                                    selectedAbility = Ability.Abilities.None;
                                    selectedTile = null;
                                    GetComponent<BaseMovement>().selectedObject = null;
                                    GetComponent<BaseMovement>().unitSelected = false;
                                    activatePlayer1Abilities();
                                    updateHUD();
                                }
                                // Flood ability
                                else if (selectedAbility == Ability.Abilities.Flood) {
                                    if (selectedTile.tileType == BaseTile.TileTypes.River) {
                                        GameObject abilityObject = Instantiate(player2.abilities[1], new Vector3(selectedTile.tilePos.x, 0, selectedTile.tilePos.y), transform.rotation, transform);
                                        abilityObject.GetComponent<Flood>().init(gameMap, selectedTile);
                                        player2.useAP(abilityObject.GetComponent<Ability>().apCost);

                                        selectedAbility = Ability.Abilities.None;
                                        selectedTile = null;
                                        GetComponent<BaseMovement>().selectedObject = null;
                                        GetComponent<BaseMovement>().unitSelected = false;
                                        activatePlayer1Abilities();
                                        updateHUD();
                                    }
                                    else {
                                        selectedTile = null;
                                    }
                                }
                                // Flash Freeze ability
                                else if (selectedAbility == Ability.Abilities.FlashFreeze) {
                                    GameObject abilityObject = Instantiate(player2.abilities[2], new Vector3(selectedTile.tilePos.x, 0, selectedTile.tilePos.y), transform.rotation, transform);
                                    abilityObject.GetComponent<FlashFreeze>().init(gameMap, selectedTile);
                                    player2.useAP(abilityObject.GetComponent<Ability>().apCost);

                                    animateAction(GetComponent<BaseMovement>().selectedObject);
                                    selectedAbility = Ability.Abilities.None;
                                    selectedTile = null;
                                    GetComponent<BaseMovement>().selectedObject = null;
                                    GetComponent<BaseMovement>().unitSelected = false;
                                    activatePlayer1Abilities();
                                    updateHUD();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Resets the selection for abilities, tiles, etc.
    void resetSelections() {
        selectedTile = null;
        selectedAbility = Ability.Abilities.None;
        GetComponent<BaseMovement>().selectedObject = null;
        GetComponent<BaseMovement>().unitSelected = false;
        GetComponent<LineRenderer>().positionCount = 0;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // UI Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Updates which camera is active and the position of the cameras
    // Called at the start of each turn
    void updateCamera() {
        if (playerTurn == 1) {
            cam1.enabled = true;
            cam2.enabled = false;

            Vector3 tempPos = new Vector3();
            foreach (BaseUnit unit in player1.GetComponentsInChildren<BaseUnit>()) {
                if (unit.unitType == BaseUnit.UnitTypes.King) {
                    tempPos = unit.transform.position;
                    break;
                }
            }
            tempPos.x -= 2.5f;
            tempPos.y += 5f;
            tempPos.z -= 2.5f;
            cam1.transform.position = tempPos;
        }
        else if (playerTurn == 2) {
            cam1.enabled = false;
            cam2.enabled = true;

            Vector3 tempPos = new Vector3();
            foreach (BaseUnit unit in player2.GetComponentsInChildren<BaseUnit>()) {
                if (unit.unitType == BaseUnit.UnitTypes.Monster) {
                    tempPos = unit.transform.position;
                    break;
                }
            }
            tempPos.x -= 5f;
            tempPos.y += 7f;
            tempPos.z -= 4f;
            cam2.transform.position = tempPos;
        }
    }

    // Checks whether the camera's position is valid
    bool cameraPosGood(GameObject cam) {
        if (playerTurn == 1) {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 9) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else if (playerTurn == 2) {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 9) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        return false;
    }

    // Updates the text for displaying information about the turns
    void updateTurnText() {
        if (playerTurn == 1) {
            foreach (Text text in UIObject.GetComponentsInChildren<Text>()) {
                if (text.name == "Player Turn Text") {
                    text.text = "Player: 1";
                }
                else if (text.name == "Turn Text") {
                    text.text = "Turn: " + gameTurn.ToString();
                }
            }
        }
        else if (playerTurn == 2) {
            foreach (Text text in UIObject.GetComponentsInChildren<Text>()) {
                if (text.name == "Player Turn Text") {
                    text.text = "Player: 2";
                }
                else if (text.name == "Turn Text") {
                    text.text = "Turn: " + gameTurn.ToString();
                }
            }
        }
    }

    // Displays/Hides the pause menu
    public void updatePauseMenu() {
        if (paused == true) {
            foreach (RectTransform item in UIObject.GetComponentsInChildren<RectTransform>()) {

                if (item.name == "Game") {
                    // Hide game
                    item.localScale = new Vector3(0,0,0);
                }
                else if (item.name == "Paused") {
                    // Reveal paused
                    item.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else {
            foreach (RectTransform item in UIObject.GetComponentsInChildren<RectTransform>()) {
                
                if (item.name == "Game") {
                    // Reveal game
                    item.localScale = new Vector3(1, 1, 1);
                }
                else if (item.name == "Paused") {
                    // Hide paused
                    item.localScale = new Vector3(0, 0, 0);
                }
            }
        }
    }

    // Turn Screen for Player 1 shows up
    void updateTurn1() {
        foreach (RectTransform item in UIObject.GetComponentsInChildren<RectTransform>()) {
            if (item.name == "Game") {
                // Hide game
                item.localScale = new Vector3(0, 0, 0);
            }
            else if (item.name == "Turn Transition 1") {
                // Reveal paused
                item.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    // Update transition button for Player 1 
    public void updateTurnButton1() {
        foreach (RectTransform item in UIObject.GetComponentsInChildren<RectTransform>()) {
                
                if (item.name == "Game") {
                    // Reveal game
                    item.localScale = new Vector3(1, 1, 1);
                }
                else if (item.name == "Turn Transition 1") {
                    // Hide paused
                    item.localScale = new Vector3(0, 0, 0);
                }
            }
    }

    // Turn Screen for Player 2 shows up
    void updateTurn2() {
        foreach (RectTransform item in UIObject.GetComponentsInChildren<RectTransform>()) {
            if (item.name == "Game") {
                // Hide game
                item.localScale = new Vector3(0, 0, 0);
            }
            else if (item.name == "Turn Transition 2") {
                // Reveal paused
                item.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    // Update transition button for Player 2
    public void updateTurnButton2() {
        foreach (RectTransform item in UIObject.GetComponentsInChildren<RectTransform>()) {
                
                if (item.name == "Game") {
                    // Reveal game
                    item.localScale = new Vector3(1, 1, 1);
                }
                else if (item.name == "Turn Transition 2") {
                    // Hide paused
                    item.localScale = new Vector3(0, 0, 0);
                }
            }
    }

    // Updates the HUD values
    public void updateHUD() {
        if (playerTurn == 0) {
            foreach (RectTransform rect in UIObject.GetComponentsInChildren<RectTransform>()) {
                if (rect.name == "Game") {
                    rect.localScale = new Vector3(0, 0, 0);
                    break;
                }
            }
        }
        else if (playerTurn == 1) {
            foreach (Image image in UIObject.GetComponentsInChildren<Image>()) {
                if (image.name == "APBar") {
                    player1.clampAP();
                    image.sprite = apBarSprites[Mathf.RoundToInt(player1.ap)];
                    break;
                }
            }
            // Changes the names of the abilities
            if (GetComponent<BaseMovement>().selectedObject != null) {
                if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.King) {
                    hideAbilityButtons();
                    showAbilityButton(Ability.Abilities.SpawnBeaver);
                    showAbilityButton(Ability.Abilities.SpawnElephant);
                    showAbilityButton(Ability.Abilities.SpawnHawk);
                }
                else if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Beaver) {
                    hideAbilityButtons();
                    showAbilityButton(Ability.Abilities.Bridge);
                }
                else if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Elephant) {
                    hideAbilityButtons();
                    showAbilityButton(Ability.Abilities.Flatten);
                }
            }
            else {
                hideAbilityButtons();
            }
        }
        else if (playerTurn == 2) {
            foreach (Image image in UIObject.GetComponentsInChildren<Image>()) {
                if (image.name == "APBar") {
                    player2.clampAP();
                    image.sprite = apBarSprites[Mathf.RoundToInt(player2.ap)];
                    break;
                }
            }
            // Changes the names of the abilities
            hideAbilityButtons();
            showAbilityButton(Ability.Abilities.Earthquake);
            showAbilityButton(Ability.Abilities.Flood);
            showAbilityButton(Ability.Abilities.FlashFreeze);
        }
    }

    // Initialises the flashing of the apBar
    // Also handles the updating of the flashing
    public void flashAPBar(bool begin = false) {
        if (begin == true) {
            flashTurn = playerTurn;
            flashTimer = 1;
            // Change colour of ap bar
            foreach (Image image in UIObject.GetComponentsInChildren<Image>()) {
                if (image.name == "APBar") {
                    image.color = new Color(1, 0, 0);
                    break;
                }
            }
        }
        else if (flashTurn != playerTurn) {
            flashTurn = 0;
            foreach (Image image in UIObject.GetComponentsInChildren<Image>()) {
                if (image.name == "APBar") {
                    image.color = new Color(1, 1, 1);
                    break;
                }
            }
        }
        else {
            if (flashTimer > 0) {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0) {
                    // Change colour of ap bar to white
                    foreach (Image image in UIObject.GetComponentsInChildren<Image>()) {
                        if (image.name == "APBar") {
                            image.color = new Color(1, 1, 1);
                            break;
                        }
                    }
                }
            }
        }
    }

    // Map the ap, to the position of the ap
    float mapAPToSlider(int maxAp, int ap) {
        float indent = 100.0f / maxAp;
        float output = indent * ap - 100;

        return output;
    }

    // Uses the selected abilities
    public void useAbility(int abilityNum) {
        buttonSelectedFrame = true;
        // Turn 1
        if (playerTurn == 1) {
            // Is the nothing selected
            if (GetComponent<BaseMovement>().selectedObject != null) {
                // Is the King selected
                if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.King) {
                    if (abilityNum == 1) {
                        if (selectedAbility != Ability.Abilities.SpawnBeaver) {
                            if (player1.ap >= player1.abilities[0].GetComponent<Ability>().apCost) {
                                // Select spawn beaver ability
                                selectedAbility = Ability.Abilities.SpawnBeaver;
                                selectedTile = null;
                            }
                            else {
                                flashAPBar(true);
                            }
                        }
                        else {
                            // Deselect ability
                            selectedAbility = Ability.Abilities.None;
                            selectedTile = null;
                        }
                    }
                    else if (abilityNum == 2) {
                        if (selectedAbility != Ability.Abilities.SpawnElephant) {
                            if (player1.ap >= player1.abilities[0].GetComponent<Ability>().apCost) {
                                // Select bridge ability
                                selectedAbility = Ability.Abilities.SpawnElephant;
                                selectedTile = null;
                            }
                            else {
                                flashAPBar(true);
                            }
                        }
                        else {
                            // Deselect ability
                            selectedAbility = Ability.Abilities.None;
                            selectedTile = null;
                        }
                    }
                    else if (abilityNum == 3) {
                        if (selectedAbility != Ability.Abilities.SpawnHawk) {
                            if (player1.ap >= player1.abilities[0].GetComponent<Ability>().apCost) {
                                // Select bridge ability
                                selectedAbility = Ability.Abilities.SpawnHawk;
                                selectedTile = null;
                            }
                            else {
                                flashAPBar(true);
                            }
                        }
                        else {
                            // Deselect ability
                            selectedAbility = Ability.Abilities.None;
                            selectedTile = null;
                        }
                    }
                }
                // If a beaver is selected
                else if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Beaver) {
                    if (abilityNum == 1) {
                        if (selectedAbility != Ability.Abilities.Bridge) {
                            if (player1.ap >= player1.abilities[0].GetComponent<Ability>().apCost) {
                                // Select bridge ability
                                selectedAbility = Ability.Abilities.Bridge;
                                selectedTile = null;
                            }
                            else {
                                flashAPBar(true);
                            }
                        }
                        else {
                            // Deselect ability
                            selectedAbility = Ability.Abilities.None;
                            selectedTile = null;
                        }
                    }
                }
                // If an elephant is selected
                else if (GetComponent<BaseMovement>().selectedObject.unitType == BaseUnit.UnitTypes.Elephant) {
                    if (abilityNum == 1) {
                        if (selectedAbility != Ability.Abilities.Flatten) {
                            if (player1.ap >= player1.abilities[0].GetComponent<Ability>().apCost) {
                                // Select bridge ability
                                selectedAbility = Ability.Abilities.Flatten;
                                selectedTile = null;
                            }
                            else {
                                flashAPBar(true);
                            }
                        }
                        else {
                            // Deselect ability
                            selectedAbility = Ability.Abilities.None;
                            selectedTile = null;
                        }
                    }
                }
            }
        }
        // Player 2
        else if (playerTurn == 2) {
            if (abilityNum == 1) {
                if (selectedAbility != Ability.Abilities.Earthquake) {
                    if (player2.ap >= player2.abilities[0].GetComponent<Ability>().apCost) {
                        // Select bridge ability
                        selectedAbility = Ability.Abilities.Earthquake;
                        selectedTile = null;
                    }
                    else {
                        flashAPBar(true);
                    }
                }
                else {
                    // Deselect ability
                    selectedAbility = Ability.Abilities.None;
                    selectedTile = null;
                }
            }
            else if (abilityNum == 2) {
                if (selectedAbility != Ability.Abilities.Flood) {
                    if (player2.ap >= player2.abilities[1].GetComponent<Ability>().apCost) {
                        // Select bridge ability
                        selectedAbility = Ability.Abilities.Flood;
                        selectedTile = null;
                    }
                    else {
                        flashAPBar(true);
                    }
                }
                else {
                    // Deselect ability
                    selectedAbility = Ability.Abilities.None;
                    selectedTile = null;
                }
            }
            else if (abilityNum == 3) {
                if (selectedAbility != Ability.Abilities.FlashFreeze) {
                    if (player2.ap >= player2.abilities[2].GetComponent<Ability>().apCost) {
                        // Select bridge ability
                        selectedAbility = Ability.Abilities.FlashFreeze;
                        selectedTile = null;
                    }
                    else {
                        flashAPBar(true);
                    }
                }
                else {
                    // Deselect ability
                    selectedAbility = Ability.Abilities.None;
                    selectedTile = null;
                }
            }
        }
    }

    // Hides all of the ability buttons
    void hideAbilityButtons() {
        foreach (RectTransform rect in UIObject.GetComponentsInChildren<RectTransform>()) {
            if (rect.name == "Abilities") {
                foreach (Button button in rect.GetComponentsInChildren<Button>()) {
                    button.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                }
                break;
            }
        }
    }

    void showAbilityButton(Ability.Abilities abilityType) {
        foreach (RectTransform rect in UIObject.GetComponentsInChildren<RectTransform>()) {
            if (rect.name == "Abilities") {
                foreach (Button button in rect.GetComponentsInChildren<Button>()) {
                    if (abilityType == Ability.Abilities.SpawnBeaver) {
                        if (button.name.Contains("Beaver")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.SpawnElephant) {
                        if (button.name.Contains("Elephant")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.SpawnHawk) {
                        if (button.name.Contains("Hawk")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.Flatten) {
                        if (button.name.Contains("Flatten")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.Bridge) {
                        if (button.name.Contains("Bridge")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.Earthquake) {
                        if (button.name.Contains("Earthquake")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.Flood) {
                        if (button.name.Contains("Flood")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else if (abilityType == Ability.Abilities.FlashFreeze) {
                        if (button.name.Contains("Freeze")) {
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        }
                    }
                }
                break;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Animations
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Calls the animation for when an ability is used
    void animateAction(BaseUnit unitObject) {
        foreach (BaseUnit unit in player1.GetComponentsInChildren<BaseUnit>()) {
            if (unit == unitObject) {
                unit.GetComponent<Animator>().SetTrigger("useAction");
                break;
            }
        }
    }
}
