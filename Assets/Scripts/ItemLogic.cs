using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogic : MonoBehaviour
{
    [Tooltip("Whether this item can be shot down by the player")]
    [SerializeField] bool destroyable;

    // Allow other classes to read the age checkpoints.
    // Needed for the spawner to only spawn certain items at certain checkpoints.
    [SerializeField] AgePointCheckpoint[] checkpoints; public AgePointCheckpoint[] Checkpoints => checkpoints;


    public class Checkpoint
    {
        public int age;
        public float healthFactor;
        public float happinessFactor;
        public float intelligenceFactor;
    }

    // Used to interpolate the effects of an item over the lifetime of the player.
    [Serializable]
    public class AgePointCheckpoint : Checkpoint { }

    // Used to guarantee a spawn of this item at a certain time in the life of the player.
    [Serializable]
    public class GuaranteedCheckpoint : Checkpoint { }
}
