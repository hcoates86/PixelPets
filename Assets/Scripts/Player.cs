using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//controls the player. characters can be switched out

public class Player : MonoBehaviour
{
    private PlayerControls playerControls;
    public CharacterAI characterAI;
    public Character character;
    private Rigidbody playerRb;
    public GameManager gameManager;
    public Transform backside;
    private InputAction movement;
    private float rotationSpeed = 100f;
    private float moveSpeed = 15f;
    private float jumpSpeed = 150f;
    public bool isMoving = false;

    [SerializeField]
    private bool isGrounded;
    public bool isBusy = false;

    private void Awake() 
    {
        playerControls = new PlayerControls();
    }

    private void Start() 
    {
        playerRb = GetComponent<Rigidbody>();
        if (characterAI == null)
            GetComponent<CharacterAI>();

    }

    void OnEnable()
    {
        movement = playerControls.Game.Move;
        movement.Enable();

        playerControls.Game.Attack.performed += Attack;
        playerControls.Game.Exert.performed += Exert;
        playerControls.Game.Jump.performed += Jump;
        playerControls.Game.Attack.Enable();
        playerControls.Game.Jump.Enable();
        playerControls.Game.Exert.Enable();
    }

    void OnDisable() 
    {
        movement.Disable();
        playerControls.Game.Attack.Disable();
        playerControls.Game.Jump.Disable();
        playerControls.Game.Exert.Disable();
        playerControls.Game.Attack.performed -= Attack;
        playerControls.Game.Exert.performed -= Exert;
        playerControls.Game.Jump.performed -= Jump;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!character.hasDied)
        {
            ControlCharacterMovement();
        }
    }

    void ControlCharacterMovement()
    {
        Vector2 input = movement.ReadValue<Vector2>();

        float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
        Quaternion rotate = Quaternion.Euler(0f, rotationAmount, 0f);

        // Calculate forward movement
        Vector3 forwardMove = transform.forward * input.y * moveSpeed * Time.fixedDeltaTime;

        // Apply movement and rotation
        Vector3 moveV = playerRb.position + forwardMove;
        Quaternion rotateQ = playerRb.rotation * rotate;

        playerRb.MovePosition(moveV);
        playerRb.MoveRotation(rotateQ);
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (!character.hasDied && !isBusy && character.currentFood != null)
            character.Eat(character.currentFood);
    }

    void Exert(InputAction.CallbackContext context)
    {
        if (!character.hasDied)
        {
            ExertionAnimation();
        }
    }

    public void ExertionAnimation()
    {
        if (!character.hasDied)
            Instantiate(gameManager.exertionObj, backside.position, Quaternion.identity);
    }


    void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && !character.hasDied)
        {
            playerRb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    public IEnumerator RotatePlayer(Transform target)
    {
        isBusy = true;

        yield return RotateToFace(target);

        yield return RotateToAngle(30f, 15f);

        yield return RotateToAngle(40f, 20f);

        yield return RotateToAngle(30f, 15f);        

        yield return RotateToAngle(40f, 15f);

        yield return RotateToAngle(30f, 15f);

        yield return RotateToAngle(0f, 5f);
    }

    IEnumerator RotateToFace(Transform target)
    {
        isMoving = true;
        while (isMoving)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            // Rotate to face the target
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 25 * Time.deltaTime);

            float angle = Vector3.Angle(transform.right, direction) - 90;
            yield return null;

            // Once facing the target, cancel rotation
            if (angle <= 1f)
            {
                isMoving = false;
                yield break;

            }
        }

    }

    IEnumerator RotateToAngle(float targetAngle, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(targetAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        while (Quaternion.Angle(playerRb.rotation, targetRotation) > 0.1f)
        {
            playerRb.rotation = Quaternion.Slerp(playerRb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        if (targetAngle == 0)
        {
            playerRb.rotation = targetRotation; // Ensure exact final rotation

            isBusy = false;

        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Food"))
        {
            character.currentFood = other.GetComponent<Food>();
        }
    }

}
