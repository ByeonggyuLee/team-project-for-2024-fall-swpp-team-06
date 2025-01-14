using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public GameManager gameManager;
    public InstantMovement instantMovement;
    public float groundedSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -4.9f;
    public float airMovementFactor = 0.5f;

    private LocationTracker locationTracker;
    private bool isGrounded = true;
    private Vector3 displacement;
    private Vector3 velocity;
    private CharacterController characterController;
    private float characterControllerHeight = 0.84f;
    private Animator animator;
    private bool isJumping = false;
    public float currentSpeed { get; private set; }
    private Vector3 startPos;

    // Start is called before the first frame update
    void Awake()
    {
        locationTracker = GetComponent<LocationTracker>();
        gameManager.OnGameOver += GameOver;
        gameManager.OnGameStart += GameStart;
        instantMovement.OnTeleport += Teleport;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsGameStart) return;
        
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -10f;
        }

        if (isGrounded && isJumping)
        {
            isJumping = false;
            animator.SetBool("Jump_b", false);
        }


        if (!gameManager.IsGameOver)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            animator.SetFloat("Horizontal", moveX);
            animator.SetFloat("Vertical", moveZ);

            float speed = isGrounded ? groundedSpeed : groundedSpeed * airMovementFactor;
            
            
            displacement = transform.right * moveX + transform.forward * moveZ;
            var displacementNorm = displacement.magnitude;

            currentSpeed = Mathf.Min(1, displacementNorm);

            var movement = displacementNorm == 0
                ? Vector3.zero
                : speed * Time.deltaTime * Mathf.Min(1f, displacementNorm) / displacementNorm * displacement;
                                                       

            characterController.Move(movement);
        }
        else
        {
            currentSpeed = 0;
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
        }


        if (!gameManager.IsGameOver && Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
            isJumping = true;
            animator.SetBool("Jump_b", true);
        }


        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void GameOver()
    {
        int idx = Random.Range(0, 2);
        animator.SetTrigger(idx == 0 ? "DyeTrig" : "DyeBackwardsTrig");
        StartCoroutine(GameOverCoroutine());
    }

    void GameStart()
    {
        animator.speed = 0.5f;
        animator.SetTrigger("StartTrig");
        StartCoroutine(GameStartCoroutine());
    }
    
    IEnumerator GameStartCoroutine()
    {
        characterController.enabled = false;
        startPos = transform.position;
        float changeSpeed = 0.1f;
        float timer = 0;
        float acceleration = 0.15f * 5 / gameManager.gameOverAnimationDuration;

        yield return new WaitForSeconds(3f);
        animator.speed = 1f;

        while (timer < 1.3f)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.up * changeSpeed * Time.deltaTime);
            changeSpeed += acceleration * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.85f);
        
        while (timer < 2.3f)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.up * changeSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(gameManager.gameStartDuration);
    }

    public void GameStartTrigger()
    {
        StopCoroutine("GameStartCoroutine");
        transform.position = startPos;
        characterController.enabled = true;
    }

    IEnumerator GameOverCoroutine()
    {
        float changeSpeed = -0.1f;
        float timer = 0;
        float acceleration = 0.15f * 5 / gameManager.gameOverAnimationDuration;

        while (timer < gameManager.gameOverAnimationDuration / 2)
        {
            timer += Time.deltaTime;
            characterController.height += changeSpeed * Time.deltaTime;
            changeSpeed -= acceleration * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(gameManager.gameOverAnimationDuration / 2);
        characterController.height = characterControllerHeight;
        characterController.enabled = false;
        transform.position = locationTracker.respawnLocations[locationTracker.lastRespawnIndex];
        characterController.enabled = true;
    }

    void Teleport()
    {
        Vector3 teleportPosition = instantMovement.basePositions[instantMovement.targetBaseIndex].transform.position;
        StartCoroutine(TeleportCoroutine(teleportPosition));
    }

    IEnumerator TeleportCoroutine(Vector3 teleportPosition)
    {
        yield return new WaitForSeconds(gameManager.gameOverAnimationDuration / 2);
        characterController.enabled = false;
        transform.position = teleportPosition;
        Debug.Log($"Teleported to Base {instantMovement.targetBaseIndex + 1} at position {teleportPosition}.");
        characterController.enabled = true;
    }
}
