using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ItemLogic;

public class ItemCollectionSystem : MonoBehaviour
{

    PlayerController pc;

    SortedDictionary<int, AgePointCheckpoint> checkpointDictionary;

    private void Start()
    {
        PlayerController pc = GetComponent<PlayerController>();

        // Load all of the item's checkpoints into a dictionary to easily get interpolation points.
        checkpointDictionary = new SortedDictionary<int, AgePointCheckpoint>();
    }


    public void ItemCollected(GameObject item)
    {
        ItemLogic logic = item.GetComponent<ItemLogic>();

        checkpointDictionary.Clear();
        foreach (AgePointCheckpoint point in logic.Checkpoints)
        {
            checkpointDictionary.Add(point.age, point);
        }

        int playerAge = pc.Age;

        AgePointCheckpoint lowerCheckpoint, higherCheckpoint;

        if (checkpointDictionary.ContainsKey(playerAge))
        {
            lowerCheckpoint = higherCheckpoint = checkpointDictionary[playerAge];
        }
        else
        {
            int[] keys = checkpointDictionary.Keys.ToArray();

            int min = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] > min && keys[i] < playerAge) min = keys[i];
            }
            lowerCheckpoint = checkpointDictionary[min];

            int max = 100;
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] < max && keys[i] > playerAge) max = keys[i];
            }
            higherCheckpoint = checkpointDictionary[max];
        }

        float ageDifference = higherCheckpoint.age - lowerCheckpoint.age;
        float ageAboveLower = playerAge - lowerCheckpoint.age;

        float lerpFactor = ageAboveLower / ageDifference; // Clamped from 0 to 1.

        float happinessScore = Mathf.Lerp(lowerCheckpoint.happinessFactor, higherCheckpoint.happinessFactor, lerpFactor);
        float healthScore = Mathf.Lerp(lowerCheckpoint.healthFactor, higherCheckpoint.healthFactor, lerpFactor);
        float intelligenceScore = Mathf.Lerp(lowerCheckpoint.intelligenceFactor, higherCheckpoint.intelligenceFactor, lerpFactor);

        Destroy(item);

    }
}
