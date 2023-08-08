using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the main gameplay
/// </summary>
public class GameController : MonoBehaviour
{
    [Tooltip("A reference to the tile we want to spawn")]
    public Transform tile;

    [Tooltip("A reference to the obstacle we want to spawn")]
    public Transform obstacle;

    [Tooltip("Where the first tile should be placed at")]
    public Vector3 startPoint = new Vector3(0, 0, -5);

    [Tooltip("How many tiles should we create in advance")]
    [Range(1, 15)]
    public int initSpawnNum = 10;

    [Tooltip("How many tiles to spawn initially with no obstacles")]
    public int initNoObstacles = 4;

    /// <summary>
    /// Where the next tile should be spawned at
    /// </summary>
    private Vector3 nextTileLocation;

    /// <summary>
    /// How the next tile should be rotated
    /// </summary>
    private Quaternion nextTileRotation;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        //If there is no UnityAdController, we can add it here
        if (!GameObject.FindObjectOfType<UnityAdController>())
        {
            var adController = new GameObject("Unity Ad Manager");
            adController.AddComponent<UnityAdController>();
        }
        //Set our starting point
        nextTileLocation = startPoint;
        nextTileRotation = Quaternion.identity;

        for (int i = 0; i < initSpawnNum; ++i)
        {
            SpawnNextTile(i >= initNoObstacles);
        }
    }

    /// <summary>
    /// Will spawn a tile at a certain location and setup the next position
    /// </summary>
    /// <param name="spawnObstacles">If we should spawn an obstacle</param>
    public void SpawnNextTile(bool spawnObstacles = true)
    {
        var newTile = Instantiate(tile, nextTileLocation, nextTileRotation);

        //Figure out where and at what rotation we should spawn the next item
        var nextTile = newTile.Find("Next Spawn Point");
        nextTileLocation = nextTile.position;
        nextTileRotation = nextTile.rotation;

        if (spawnObstacles)
        {
            SpawnObstacle(newTile);
        }
    }

    /// <summary>
    /// Will spawn an obstacle on a tile at one of its obstacle spawn points, if any
    /// </summary>
    /// <param name="newTile"></param>
    private void SpawnObstacle(Transform newTile)
    {
        //First, collect all of the possible places to spawn the obstacle
        List<GameObject> obstacleSpawnPoints = new List<GameObject>();

        //Go through each of the child game objects in our tile
        foreach (Transform child in newTile)
        {
            //If it has the ObstacleSpawn tag
            if (child.CompareTag("ObstacleSpawn"))
            {
                //We add it as a possibility
                obstacleSpawnPoints.Add(child.gameObject);
            }
        }

        //Make sure there is at least one obstacle spawn point
        if (obstacleSpawnPoints.Count > 0)
        {
            //Get a random object from the ones we have
            var spawnPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Count)];

            //Store its position for us to use
            var spawnPos = spawnPoint.transform.position;

            //Create our obstacle
            var newObstacle = Instantiate(obstacle, spawnPos, Quaternion.identity);

            //Have it parented to the tile
            newObstacle.SetParent(spawnPoint.transform);
        }
    }
}
