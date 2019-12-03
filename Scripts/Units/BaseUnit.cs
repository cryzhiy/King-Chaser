using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {
    // Enumerator containing the types of units
    public enum UnitTypes { None, King, Monster, Beaver, Elephant, Hawk };
    public UnitTypes unitType = UnitTypes.None;

    GameManager gameManager;

    public float moveSpeed = 10;
    public float moveAmount = 5;
    Vector3 velocity;
    Vector3 heading;

    [HideInInspector]
    public bool moving = false;
    Stack<BaseTile> path = new Stack<BaseTile>();
    BaseTile targetTile = null;
    BaseTile currentTile = null;

    // Find where the camera position is when launching
    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<GameManager>();
    }

    void Update() {
        if (moving == true) {
            moveUnit();
        }
        if (gameManager.gameMap.tileMap[(int)transform.position.z][(int)transform.position.x].gameObject.layer != gameObject.layer) {
            gameObject.layer = gameManager.gameMap.tileMap[(int)transform.position.z][(int)transform.position.x].gameObject.layer;
        }
    }

    // When launched gets all Tiles and adds them to list
    public void initMove(Stack<BaseTile> tempPath, BaseTile target, BaseTile current) {
        Stack<BaseTile> bob = new Stack<BaseTile>(tempPath);
        path = new Stack<BaseTile>(bob);
        targetTile = target;
        currentTile = current;
        moving = true;
    }

    void moveUnit() {
        // The path that the unit will go through when trying to move
        if (path.Count > 0) {
            BaseTile checkBaseTile = path.Peek();
            Vector3 target = checkBaseTile.transform.position;

            // Calculate the Units position on top of the tile
            target.y = transform.position.y;

            if (Vector3.Distance(transform.position, target) >= moveSpeed * Time.deltaTime) {
                // calcualtes both the direction and the speed
                calculateHeading(target);

                // moves the unit 
                Vector3 direction = checkBaseTile.transform.position;
                direction.y = transform.position.y;
                transform.LookAt(direction);
                transform.Rotate(transform.up, 0f);

                // The speed of unit is affected by time
                transform.position += velocity * Time.deltaTime;
            }
            else {
                // call fog of war
                if (unitType == UnitTypes.King || unitType == UnitTypes.Hawk) {
                    gameManager.gameMap.revealTiles(transform.position);
                }
                // direction of Unit

                // Player reaches center of tile and looks for next tile in list
                transform.position = target;
                path.Pop();

            }
        }
        else {
            // if kraken lands on tile with another object delete object
            if (unitType == UnitTypes.Monster) {
                // if Monster is on tile of unit destroy the unit
                if (targetTile.unitOnTile == UnitTypes.Elephant || targetTile.unitOnTile == UnitTypes.Beaver || targetTile.unitOnTile == UnitTypes.Hawk) {
                    RaycastHit hit;
                    if (Physics.Raycast(targetTile.transform.position, Vector3.up, out hit, 1)) {
                        if (hit.collider.GetComponent<BaseUnit>() != null) {
                            Debug.Log(targetTile.unitOnTile);
                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
                // Player 2 Wins if Octopus is on the kings tile
                else if (targetTile.unitOnTile == BaseUnit.UnitTypes.King) {
                    gameManager.gameMap.gameManager.player2Win();
                }
            }

            // changes tile state (what unit is on the tile)
            currentTile.newUnitOnTile(UnitTypes.None);
            targetTile.newUnitOnTile(unitType);
            // Makes unit visible
            if (targetTile.gameObject.layer == 8) {
                foreach (Transform part in GetComponentsInChildren<Transform>()) {
                    part.gameObject.layer = 8;
                }
            }
            // Makes unit invisible
            if (targetTile.gameObject.layer == 9) {
                foreach (Transform part in GetComponentsInChildren<Transform>()) {
                    part.gameObject.layer = 9;
                }
            }

            // changes state
            moving = false;
            targetTile = null;
            currentTile = null;
        }
    }

    //calculate the direction
    void calculateHeading(Vector3 target) {
        heading = target - transform.position;
        heading.Normalize();
        velocity = heading * moveSpeed;
    }
}