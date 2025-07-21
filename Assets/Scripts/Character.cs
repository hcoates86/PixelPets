using System.Collections;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    public GameManager gameManager;
    public Player player;

    public Transform backside;
    // the pixels that make up the character
    public Transform pixelContainer;
    // the character emotes location to be followed
    public Transform emotions;
    public Transform thoughtBubble;
    private Vector3 offset = new Vector3(2, 3, 0);

    public GameObject happySprite;
    public GameObject unhappySprite;
    public GameObject hungrySprite;
    public GameObject deadSprite;
    public TMP_Text hungryText;

    public GameObject currentEmote;

    //lowers over time
    public float hunger;
    public float hungerChangePerSecond = -10f;

    private int hungerThreshold = 50;
    //lowers over time, does not affect happiness
    public float bathroom;
    public float bathroomChangePerSecond = -35f;
    //affected by unclean environment and decorations
    public float environment;
    private float dirtyValue = -15f;
    private float prettyValue = 8f;
    // increased by playing, decreases over time
    public float fun;
    public float funChangePerSecond = -35f;
    //high if all needs are met, lowers if not
    public float happiness;

    float minValue = 0;
    float maxValue = 100;

    private float explosiveForce = 2;
    private float explosiveupwardsModifier = 1.5f;

    public bool hasDied = false;
    private bool deathCountdownOn = false;
    public bool displayingEmote = false;

    private Transform target;
    public Food currentFood;


    void Start()
    {
        hunger = 100;
        bathroom = 100;
        environment = 100;
        gameManager = Data.Instance.gameManager;
        Data.Instance.currentChar = this;
        player = transform.parent.GetComponent<Player>();
    }

    void Update()
    {
        if (!hasDied)
        {
            DecrementNeeds();
        }
    }

    public void Eat(Food food)
    {
        if (food == null) return;

        target = food.transform;
        Vector3 playerPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPositionXZ = new Vector3(target.position.x, 0, target.position.z);

        float distanceToTarget = Vector3.Distance(playerPositionXZ, targetPositionXZ);

        if (food.foodLeft <= 0 || food.isRotten) return;
        if (distanceToTarget <= 4.5f)
        {
            food.rb.isKinematic = true;
            hunger += food.BeEaten();
        }
        food.isBeingEaten = false;
    }

    void DecrementNeeds()
    {
        hunger = Mathf.Clamp(hunger + hungerChangePerSecond * Time.deltaTime, minValue, maxValue);
        bathroom = Mathf.Clamp(bathroom + bathroomChangePerSecond * Time.deltaTime, minValue, maxValue);

        //takes the base environment value and adds/subtracts based on number of items
        environment = Mathf.Clamp(100 + (prettyValue + (float) gameManager.prettyFloorItems.Count) + (dirtyValue * (float) gameManager.dirtyFloorItems.Count), minValue, maxValue);
        happiness = (hunger + environment + fun) / 3;
        
        if (bathroom <= 0)
        {
            player.ExertionAnimation();
            bathroom = 100;
        }

        if (hunger < hungerThreshold)
        {
            if (hungerThreshold == 50  || hungerThreshold == 40 || hungerThreshold == 30)
                hungryText.text = "?";
            if (hungerThreshold == 20  || hungerThreshold == 10)
                hungryText.text = "!";
            StartCoroutine(DisplayEmotion(EmotionList.Hungry));
            hungerThreshold -= 10;
        }
        if (hunger <= 0)
            DeathAnimation();

        if (happiness < 20 && deathCountdownOn)
        {
            StartCoroutine(DisplayEmotion(EmotionList.Dead));
            StartCoroutine(CountDownToDeath(10));
        }
    }

    IEnumerator CountDownToDeath(float seconds)
    {

        deathCountdownOn = true;
        while (deathCountdownOn)
        {
            seconds -= Time.deltaTime;
            if (happiness > 20)
            {
                deathCountdownOn = false;
                StopCoroutine("CountDownToDeath");
            }
            else if (seconds <= 0)
            {
                DeathAnimation();
                deathCountdownOn = false;
            }
            
            yield return null;
        }

    }

    void DeathAnimation()
    {
        hasDied = true;
        Data.Instance.CharacterDied();

        //adds rigidbodies to all pixels and allows them to fall, with a little explosive force
        foreach (Transform g in pixelContainer.GetComponentsInChildren<Transform>())
        {
            Rigidbody rb = g.gameObject.AddComponent<Rigidbody>();
            rb.mass = 1;
            rb.AddExplosionForce(explosiveForce, transform.position, 2f, explosiveupwardsModifier);
        }

        gameManager.newChar.SetActive(true);
    }

    public IEnumerator DisplayEmotion(EmotionList emotion)
    {
        if (displayingEmote)
            currentEmote.SetActive(false);
            
        displayingEmote = true;
        thoughtBubble.gameObject.SetActive(true);

        StartCoroutine(ThoughtBubbleFollow());

        switch (emotion)
        {
            case EmotionList.Happy:
                currentEmote = happySprite;
                break;
            case EmotionList.Hungry:
               currentEmote = hungrySprite;
                break;
            default: break;
        }

        currentEmote.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        currentEmote.SetActive(false);
        thoughtBubble.gameObject.SetActive(false);
        displayingEmote = false;

    }

    IEnumerator ThoughtBubbleFollow()
    {
        while (displayingEmote)
        {
            thoughtBubble.position = emotions.position + offset;
            yield return null;
        }
    }
}

public enum EmotionList
{
    None,
    Happy,
    Unhappy,
    Hungry,
    Dead
}
