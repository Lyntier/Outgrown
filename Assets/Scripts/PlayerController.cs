using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ItemLogic;

public class PlayerController : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField] int startingAge;
    [Range(-5f, 0f)]
    [SerializeField] float minScore;
    [Range(0f, 5f)]
    [SerializeField] float maxScore;

    [Space]

    //[SerializeField] TextMeshProUGUI ageElement;
    //[SerializeField] TextMeshProUGUI collectedVeggiesElement;
    //[SerializeField] TextMeshProUGUI healthElement;
    //[SerializeField] TextMeshProUGUI happinessElement;
    //[SerializeField] TextMeshProUGUI intelligenceElement;

    [SerializeField] TextMeshProUGUI ageText;
    [SerializeField] RawImage happinessBar;
    [SerializeField] RawImage healthBar;
    [SerializeField] RawImage intelligenceBar;

    [Space]

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    int age; public int Age => age;

    Rigidbody2D rb;
    Vector2 velocity;

    ItemCollectionSystem ics;

    SortedDictionary<int, AgePointCheckpoint> checkpointDictionary;

    float trueGravity;
    bool isGrounded;

    int collectedVeggies;
    float healthScore = 2f;
    float happinessScore = 4f;
    float intelligenceScore = -3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ics = GetComponent<ItemCollectionSystem>();

        trueGravity = rb.gravityScale;
        age = startingAge;

        // Load all of the item's checkpoints into a dictionary to easily get interpolation points.
        checkpointDictionary = new SortedDictionary<int, AgePointCheckpoint>();
        InvokeRepeating("IncreaseAge", 15f, 15f);
    }

    private void Update()
    {
        // Save previous velocity for modifications
        velocity = rb.velocity;

        velocity.x = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (transform.position.y < -2.5f) isGrounded = true;

        if (Input.GetAxisRaw("Jump") > 0.5f && rb.velocity.y > 0)
        {
            rb.gravityScale = trueGravity;
        }
        else
        {
            rb.gravityScale = trueGravity * 1.8f;
        }

        if (isGrounded && Input.GetAxisRaw("Jump") > 0.5f)
        {
            print("Jumping");
            velocity.y = jumpForce;
            isGrounded = false;
        }

        // Re-apply velocity
        rb.velocity = velocity;

        CheckMousePosition();

        // Update score labels
        UpdateScoreLabels();

    }

    void UpdateScoreLabels()
    {
        ageText.text = age.ToString();
        //collectedVeggiesElement.text = "Collected veggies: " + collectedVeggies.ToString();
        //healthElement.text = "Health: " + healthScore.ToString();
        //happinessElement.text = "Happiness: " + happinessScore.ToString();
        //intelligenceElement.text = "Intelligence: " + intelligenceScore.ToString();

        // Below lines use the scores, clamped from -5f to 5f, to create bars that are up to 300 units wide.
        // Offsetting from -5 to 5 creates 0 to 10, multiply by 30 to get 300.
        healthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (healthScore + 5f) * 30);
        happinessBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (happinessScore + 5f) * 30);
        intelligenceBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (intelligenceScore + 5f) * 30);

        // Determining their color
        SetBarColors();
    }

    void SetBarColors()
    {
        SetBarColor(healthBar, healthScore);
        SetBarColor(happinessBar, happinessScore);
        SetBarColor(intelligenceBar, intelligenceScore);
    }

    void SetBarColor(RawImage bar, float score)
    {
        /// If -5
        /// RedSHOULD = 1
        /// Red = Max(0, (-)-5 / 5) = 1
        /// If 5
        /// RedSHOULD = 0
        /// Red = Max(0, -5/5) = 0
        /// If 0
        /// RedSHOULD = 1
        /// Red = Max(0, 0/5) = 0 ??
        float r = 1 - Mathf.Max(0f, score / 5f);

        float g = Mathf.Max(0f, score / 5f);

        bar.color = new Color(r, g, 0);
    }

    void CheckMousePosition()
    {
        if (Input.mousePosition.x < transform.position.x) GetComponent<SpriteRenderer>().flipX = true;
    }

    public void ItemCollected(GameObject item)
    {
        ItemLogic logic = item.GetComponent<ItemLogic>();

        checkpointDictionary.Clear();
        foreach (AgePointCheckpoint point in logic.Checkpoints)
        {
            checkpointDictionary.Add(point.age, point);
        }

        int playerAge = Age;

        AgePointCheckpoint lowerCheckpoint, higherCheckpoint;

        if (checkpointDictionary.ContainsKey(playerAge))
        {
            lowerCheckpoint = higherCheckpoint = checkpointDictionary[playerAge];
        }
        else
        {
            int[] keys = checkpointDictionary.Keys.ToArray();

            int min = -1;
            int max = 101;

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] > min && keys[i] < playerAge) min = keys[i];
                if (keys[i] < max && keys[i] > playerAge) max = keys[i];
            }

            lowerCheckpoint = checkpointDictionary[min];
            higherCheckpoint = checkpointDictionary[max];
        }

        float ageDifference = higherCheckpoint.age - lowerCheckpoint.age;
        float ageAboveLower = playerAge - lowerCheckpoint.age;

        float lerpFactor;
        if (ageDifference != 0)
            lerpFactor = ageAboveLower / ageDifference; // Clamped from 0 to 1.
        else lerpFactor = 0;

        float happinessScore = Mathf.Lerp(lowerCheckpoint.happinessFactor, higherCheckpoint.happinessFactor, lerpFactor);
        float healthScore = Mathf.Lerp(lowerCheckpoint.healthFactor, higherCheckpoint.healthFactor, lerpFactor);
        float intelligenceScore = Mathf.Lerp(lowerCheckpoint.intelligenceFactor, higherCheckpoint.intelligenceFactor, lerpFactor);

        UpdateCollectionCount();
        UpdateScores(happinessScore, healthScore, intelligenceScore);

        Destroy(item);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject otherItem = other.gameObject;
        if (otherItem.tag == "Collectible") ItemCollected(otherItem);
    }

    private void IncreaseAge()
    {
        age++;
    }

    public void UpdateCollectionCount()
    {
        collectedVeggies++;
    }

    void UpdateScores(float newHappiness, float newHealth, float newIntelligence)
    {
        UpdateScore(ref happinessScore, newHappiness);
        UpdateScore(ref healthScore, newHealth);
        UpdateScore(ref intelligenceScore, newIntelligence);
    }

    void UpdateScore(ref float scoreToUpdate, float itemScore)
    {
        if (Mathf.Approximately(itemScore, 0f)) return; // Do not impact score if attribute is 0.

        float diff = itemScore - scoreToUpdate;

        if (itemScore < 0) // Item impacts player negatively
        {
            if (diff > 0) // Item is better than player's current score; minimal impact
            {
                scoreToUpdate += -0.25f;
            }
            else if (diff < -0.75f) // Item is way worse than player's current score; maximal impact
            {
                scoreToUpdate += -0.75f;
            }
            else // Somewhere inbetween 0 and -0.5, applying should be fine.
            {
                scoreToUpdate += diff;
            }
        }
        else
        {
            if (diff < 0) // Item is worse than player's current score, minimal impact
            {
                scoreToUpdate += 0.1f;
            }
            else if (diff > 0.5f) // Item is way better than player's current score; maximal impact
            {
                scoreToUpdate += 0.5f;
            }
            else // Somewhere inbetween 0 and 0.5, applying should be fine.
            {
                scoreToUpdate += diff;
            }
        }

        scoreToUpdate = Mathf.Clamp(scoreToUpdate, -5f, 5f); // Ensure score doesn't go OOB.
    }
}