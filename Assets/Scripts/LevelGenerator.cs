using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject ceilingPrefab;

	public GameObject characterController;

	public GameObject floorParent;
	public GameObject wallsParent;

	// allows us to see the maze generation from the scene view
	public bool generateRoof = true;

	// number of times we want to "dig" in our maze
	public int tilesToRemove = 50;

	public int mazeSize;

	// spawns at the end of the maze generation
	public GameObject pickup;

	// this will determine whether we've placed the character controller
	private bool characterPlaced = false;

	// enum to represent Tile type
	public enum TileType
	{
		HOLE,
		FLOOR,
		WALL
	}
	// 2D array representing the map
	private TileType[,] mapData;

	// we use these to dig through our maze and to spawn the pickup at the end
	private int mazeX = 4, mazeY = 1;

	// define the number of holes in the generated maze
    public int holesNumber = 5;

	// keeps track of which maze the player is in, increments each time they progress to the next maze
	public static int level = 1; 

    // Use this for initialization
    void Start () {

		// initialize map 2D array
		mapData = GenerateMazeData();

		// create actual maze blocks from maze boolean data
		for (int z = 0; z < mazeSize; z++) {
			for (int x = 0; x < mazeSize; x++) {
				if (mapData[z, x] == TileType.WALL) {
					CreateChildPrefab(wallPrefab, wallsParent, x, 1, z);
					CreateChildPrefab(wallPrefab, wallsParent, x, 2, z);
					CreateChildPrefab(wallPrefab, wallsParent, x, 3, z);
				} else if (mapData[z, x] == TileType.FLOOR && !characterPlaced) {

					// place the character controller on the first empty wall we generate
					characterController.transform.SetPositionAndRotation(
						new Vector3(x, 1, z), Quaternion.identity
					);

					// flag as placed so we never consider placing again
					characterPlaced = true;
				}

				// create floor and ceiling
				if (mapData[z, x] != TileType.HOLE)
				{
					CreateChildPrefab(floorPrefab, floorParent, x, 0, z);
				}
				/*else
				{
                    CreateChildPrefab(floorPrefab, floorParent, x, -20, z);
                }*/

				if (generateRoof) {
					CreateChildPrefab(ceilingPrefab, wallsParent, x, 4, z);
				}
			}
		}

		// spawn the pickup at the end
		var myPickup = Instantiate(pickup, new Vector3(mazeX, 1, mazeY), Quaternion.identity);
		myPickup.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
	}

    // generates the booleans determining the maze, which will be used to construct the cubes
    // actually making up the maze
    TileType[,] GenerateMazeData() {
        TileType[,] data = new TileType[mazeSize, mazeSize];

		// initialize all walls to true
		for (int y = 0; y < mazeSize; y++) {
			for (int x = 0; x < mazeSize; x++) {
				data[y, x] = TileType.WALL;
			}
		}

		// counter to ensure we consume a minimum number of tiles
		int tilesConsumed = 0;

		// iterate our random crawler, clearing out walls and straying from edges
		while (tilesConsumed < tilesToRemove) {
			
			// directions we will be moving along each axis; one must always be 0
			// to avoid diagonal lines
			int xDirection = 0, yDirection = 0;

			if (Random.value < 0.5) {
				xDirection = Random.value < 0.5 ? 1 : -1;
			} else {
				yDirection = Random.value < 0.5 ? 1 : -1;
			}

			// random number of spaces to move in this line
			int numSpacesMove = (int)(Random.Range(1, mazeSize - 1));

			// move the number of spaces we just calculated, clearing tiles along the way
			for (int i = 0; i < numSpacesMove; i++) {
				mazeX = Mathf.Clamp(mazeX + xDirection, 1, mazeSize - 2);
				mazeY = Mathf.Clamp(mazeY + yDirection, 1, mazeSize - 2);

				if (data[mazeY, mazeX] == TileType.WALL) {
					data[mazeY, mazeX] = TileType.FLOOR;
					tilesConsumed++;
				}
			}
		}

        // Spawning holes in the floor of the maze that the player can fall through 
		while(holesNumber > 0)
		{
            int holeX = Random.Range(0, mazeSize);
            int holeY = Random.Range(0, mazeSize);
            // Ensure the hole is not already a hole and is not adjacent to an existing wall
            if (data[holeY, holeX] == TileType.FLOOR)
            {
                data[holeY, holeX] = TileType.HOLE;
                holesNumber--;
            }
        }

        return data;
	}


    // allow us to instantiate something and immediately make it the child of this game object's
    // transform, so we can containerize everything. also allows us to avoid writing Quaternion.
    // identity all over the place, since we never spawn anything with rotation
    void CreateChildPrefab(GameObject prefab, GameObject parent, int x, int y, int z) {
		var myPrefab = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
		myPrefab.transform.parent = parent.transform;
	}
}
