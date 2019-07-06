using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static ItemLogic;

public class ItemSpawner : MonoBehaviour
{
    GameObject[] collectibles;

    [Tooltip("Just add the fucking player here please")]
    [SerializeField] GameObject player;

    [Space]
    [Tooltip("Time (in seconds) it takes to spawn at least one item")]
    [SerializeField] float minSpawnFrequency;
    [Tooltip("Time (in seconds) between two conssecutive item spawns, at least")]
    [SerializeField] float maxSpawnFrequency;

    [Space]
    [Tooltip("The minimum speed of the spawned items")]
    [SerializeField] float minItemSpeed;
    [Tooltip("The maximum speed of the spawned items")]
    [SerializeField] float maxItemSpeed;

    [Space]
    [Tooltip("The bias for items spawning horizontally instead of vertically")]
    [Range(0f, 1f)]
    [SerializeField] float horizontalItemBias;

    [Space]
    [SerializeField]
    float directionalPadding = 1; // Spawn items off-screen without them spawning in corners.

    BoxCollider2D itemSpawnField;
    float xMin, xMax, yMin, yMax;

    float itemVelocity;

    Vector2 velocity;

    int playerAge;

    IEnumerator spawnCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        collectibles = Resources.LoadAll<GameObject>("CollectibleItems");

        foreach (GameObject collectible in collectibles)
        {
            print(collectible.name);
        }
        print(collectibles.Length);

        itemSpawnField = GetComponent<BoxCollider2D>();

        Bounds itemSpawnBounds = itemSpawnField.bounds;
        xMin = itemSpawnBounds.min.x; print(xMin);
        xMax = itemSpawnBounds.max.x; print(xMax);
        yMin = itemSpawnBounds.min.y; print(yMin);
        yMax = itemSpawnBounds.max.y; print(yMax);

        spawnCoroutine = SpawnItem();
        StartCoroutine(spawnCoroutine);

        if (player != null) player.GetComponent<PlayerController>().enabled = true;

    }

    private void OnEnable()
    {
        if (spawnCoroutine != null)
            StartCoroutine(spawnCoroutine);
        if (player != null)
            player.GetComponent<PlayerController>().enabled = true;
}

    private void OnDisable()
    {
        StopAllCoroutines();
        player.GetComponent<PlayerController>().enabled = false;
    }


    IEnumerator SpawnItem()
    {
        for (; ; )
        {
            // Randomly decides whether the item should spawn horizontally (moving
            // either left or right) or vertically (moving from the top down).
            bool horizontalItem = Random.value > horizontalItemBias;
            bool movingRight = Random.value > 0.5f;

            Vector2 spawnCoords = Vector2.zero;
            if (horizontalItem)
            {
                spawnCoords.x = (movingRight ? xMin : xMax);
                if (movingRight) spawnCoords.x -= directionalPadding;
                else spawnCoords.x += directionalPadding;
                spawnCoords.y = Random.Range(yMin, yMax);
            }
            else
            {
                spawnCoords.y = yMax + directionalPadding;
                spawnCoords.x = Random.Range(xMin, xMax);
            }

            itemVelocity = Random.Range(minItemSpeed, maxItemSpeed);
            if (horizontalItem)
            {
                if (movingRight) velocity.x = itemVelocity;
                else velocity.x = -itemVelocity;

                velocity.y = 0;
            }
            else
            {
                velocity.y = -itemVelocity;
                velocity.x = 0;
            }

            CreateCollectible(spawnCoords, velocity);

            float waitTime = Random.Range(maxSpawnFrequency, minSpawnFrequency);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private GameObject CreateCollectible(Vector2 appliedPosition, Vector2 appliedVelocity)
    {
        GameObject gameObject = DetermineSpawnable();
        gameObject.transform.position = appliedPosition;

        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = appliedVelocity;
        rb.AddTorque((Random.value + 2f) * 30);


        return gameObject;
    }

    private GameObject DetermineSpawnable()
    {
        List<GameObject> spawnableCollectibles = new List<GameObject>();
        playerAge = player.GetComponent<PlayerController>().Age;
        for (int i = 0; i < collectibles.Length; i++)
        {
            int min = -1;
            int max = 101;

            foreach (AgePointCheckpoint apc in collectibles[i].GetComponent<ItemLogic>().Checkpoints)
            { // Determining the boundaries of the collectible.
                if (apc.age <= playerAge && apc.age > min)
                {
                    print("Adjusting min");
                    min = apc.age;
                }
                if (apc.age >= playerAge && apc.age < max) max = apc.age;
            }


            if (min == -1 || max == 101)
            {
                // Either none of the collectible's spawns are under the playerage, or none are above it.
                // In either case, this means that the player should not encounter this gameobject yet,
                // or should not encounter it anymore.
            }
            else
            {
                spawnableCollectibles.Add(collectibles[i]);
            }
        }
        Assert.IsTrue(spawnableCollectibles.Count > 0);

        GameObject gameObject = Instantiate(spawnableCollectibles[Random.Range(0, spawnableCollectibles.Count)]);

        return gameObject;
    }
}
