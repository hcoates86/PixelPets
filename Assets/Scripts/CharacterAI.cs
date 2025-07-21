using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    public Transform waypointContainer;
    public Character character;
    [SerializeField]
    private List<Transform> waypoints;
    [SerializeField]
    private int currentWaypointIndex = 0;
    [SerializeField] private float speed;
    [SerializeField]
    private float rotationSpeed = 1f;
    private Transform target;
    private Vector3 direction;

    [SerializeField] private Transform frontLeg;
    [SerializeField] private Transform hindLeg;
    private Vector3 initFrontLegPos;
    private Vector3 initHindLegPos;

    private float angleThreshold = 3.0f;

    public float secondsBetweenMovement = 1;
    private int randomIndex;
    public bool isMoving = false;
    public bool aiOn = false;

    // Start is called before the first frame update
    void Start()
    {
        //adds all children in container to list
        for (int i = 0; i < waypointContainer.childCount; i++)
        {
            waypoints.Add(waypointContainer.GetChild(i));
        }
        //grabs initial positions for walk animation
        initFrontLegPos = frontLeg.localPosition;
        initHindLegPos= hindLeg.localPosition;
        StateSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isMoving)
        {
            StartCoroutine(MoveToRandomWaypoint());
        }
    }

    public void StateSwitch()
    {
        aiOn = !aiOn;

        if (aiOn && !character.hasDied)
        {
            StartCoroutine(MoveToRandomWaypoint());
        }
        else
        {
            StopAllCoroutines();
            frontLeg.localPosition = initFrontLegPos;
            hindLeg.localPosition = initHindLegPos;

        }
    }

    IEnumerator DelayBeforeWalk()
    {
        yield return new WaitForSeconds(3f);
        if (aiOn && !character.hasDied)
            StartCoroutine(MoveToRandomWaypoint());

    }

    public IEnumerator WalkAnimation()
    {
        while (isMoving)
        {
            float newFZ = initFrontLegPos.z + 1;
            float newHZ = initHindLegPos.z + 1;
            frontLeg.localPosition = new Vector3(frontLeg.localPosition.x, frontLeg.localPosition.y, newFZ);
            hindLeg.localPosition = new Vector3(hindLeg.localPosition.x, hindLeg.localPosition.y, newHZ);
            yield return new WaitForSeconds(secondsBetweenMovement);
            newFZ = initFrontLegPos.z - 1;
            newHZ = initHindLegPos.z - 1;
            frontLeg.localPosition = new Vector3(frontLeg.localPosition.x, frontLeg.localPosition.y, newFZ);
            hindLeg.localPosition = new Vector3(frontLeg.localPosition.x, frontLeg.localPosition.y, newHZ);
            yield return new WaitForSeconds(secondsBetweenMovement);
        }
        //reset legs when it exits while loop
        frontLeg.localPosition = initFrontLegPos;
        hindLeg.localPosition = initHindLegPos;
        yield return null;
    }

    IEnumerator MoveToWaypoint()
    {
        if (waypoints.Count > 0)
            isMoving = true;

        StartCoroutine(WalkAnimation());

        target = waypoints[currentWaypointIndex];

        while (isMoving)
        {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        // Rotate to face the target
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angle = Vector3.Angle(transform.right, direction) - 90;

        // Once facing the target, move towards it
        if (angle <= angleThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);


            Vector3 playerPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPositionXZ = new Vector3(target.position.x, 0, target.position.z);

            float distanceToTarget = Vector3.Distance(playerPositionXZ, targetPositionXZ);

            //checks distance vs the stopping distance
            if (distanceToTarget <= 1) 
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                isMoving = false;
            }
        }

        yield return null; 
        }
    }

    IEnumerator MoveToRandomWaypoint()
    {
        isMoving = true;
        StartCoroutine(WalkAnimation());

        randomIndex = Random.Range(0, waypoints.Count);
        while (target == waypoints[randomIndex])
            randomIndex = Random.Range(0, waypoints.Count);
            
        target = waypoints[randomIndex];

        while (isMoving)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            // Rotate to face the target
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float angle = Vector3.Angle(transform.right, direction) - 90;

            // Once facing the target, move towards it
            if (angle <= angleThreshold)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                Vector3 playerPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetPositionXZ = new Vector3(target.position.x, 0, target.position.z);

                float distanceToTarget = Vector3.Distance(playerPositionXZ, targetPositionXZ);

                //checks distance vs the stopping distance
                if (distanceToTarget <= 1) 
                {
                    isMoving = false;
                    if (aiOn)
                    {
                        yield return new WaitForSeconds(4.5f);
                        StartCoroutine(MoveToRandomWaypoint());
                        yield break;
                    }
                }
            }

            yield return null; 
        }
    }


    public IEnumerator MoveToTargetAndEat(Transform newTarget)
    {
        isMoving = true;
        StartCoroutine(WalkAnimation());

        target = newTarget;

        while (isMoving)
        {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        // Rotate to face the target
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angle = Vector3.Angle(transform.right, direction) - 90;

        // Once facing the target, move towards it
        if (angle <= 1.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);


            Vector3 playerPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPositionXZ = new Vector3(target.position.x, 0, target.position.z);

            float distanceToTarget = Vector3.Distance(playerPositionXZ, targetPositionXZ);

            //if out of food cancels out movement

            if (Data.Instance.gameManager.food == null)
            {
                isMoving = false;
                yield break;
            }

            if (distanceToTarget <= 3.5f) 
            {
                while (Data.Instance.gameManager.food.foodLeft > 0)
                {
                    isMoving = false;
                    character.Eat(Data.Instance.gameManager.food);
                    yield return new WaitForSeconds(1);
                }
                StartCoroutine(DelayBeforeWalk());
                
    }
        }

        yield return null; 
        }
    }
}
