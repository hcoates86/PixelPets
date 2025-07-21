using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour
{
    public GameManager gameManager;
    public ParticleSystem stinkPS;
    public ParticleSystem foodPS;
    public Transform edibleFood;
    public Rigidbody rb;
    private int secondsUntilRotted = 20;

    public bool isBeingEaten;
    //gets 3 bites out of each piece of food
    public int foodLeft = 3;
    public bool isRotten;
    public Collider itemCollider;
    private Vector3 lowestPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isBeingEaten = false;
        foodLeft = 3;

        if (Data.Instance.gameManager != null)
            gameManager = Data.Instance.gameManager;
        else
            Data.Instance.RefreshReferences();

        StartCoroutine(TimeTillRotten(secondsUntilRotted));
    }

    //returns a float for the amount of hunger it satiates
    public float BeEaten()
    { 
        if (foodLeft <= 0) return 0;
        //should be set false when char is done
        isBeingEaten = true;
        float newY;
        foodLeft --;
        foodPS.Play();

        if (foodLeft == 2)
        {
            newY = 0.8f;
            //reduces the visible part of food in the bowl
            edibleFood.localScale = new Vector3(edibleFood.localScale.x, newY, edibleFood.localScale.z);
        }
        if (foodLeft == 1)
        {
            newY = 0.5f;
            edibleFood.localScale = new Vector3(edibleFood.localScale.x, newY, edibleFood.localScale.z);
        }
        //if all food is eaten adds to cleanable items
        if (foodLeft == 0)
        {
            edibleFood.gameObject.SetActive(false);
            gameManager.cleanableNeutralFloorItems.Add(gameObject);

        }

        return 30f;
    }

    //Rots and has to be cleaned up
    IEnumerator TimeTillRotten(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        //if currently being eaten when time expires, waits
        while (isBeingEaten)
        {
            yield return null;
        }
        //after the time will rot even if plate is empty. An empty plate alone won't be dirty though.
        isRotten = true;
        stinkPS.Play();
        gameManager.dirtyFloorItems.Add(gameObject);

    }

    void SnapToGround()
    {
        if (itemCollider == null)
            itemCollider = (Collider) GetComponent("Collider");
        lowestPoint = itemCollider.bounds.min;
        RaycastHit hit;
        //mask ground
        int mask = (1 << 8); 


        if (Physics.Raycast(lowestPoint + Vector3.up * 0.1f, Vector3.down, out hit, 100f, mask))
        {
            float distanceToMoveDown = Vector3.Distance(lowestPoint, hit.point);
            transform.position -= Vector3.up * distanceToMoveDown;
        }
    }

}
