using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public GameManager gameManager;
    public UIManager uiManager;

    public float horizontalSensitivity = 150f;
    public float verticalSensitivity = 100f;
    public float xRotationLimit = 89f;

    public GameObject player;
    private Transform playerTransform;
    private PlayerMovement playerMovement;

    public Transform headRigTransform;
    private Quaternion headRigRotation = Quaternion.Euler(-94, 90, 0);

    public Transform headTransform;
    private Vector3 headPosition = new(0, -0.17f, -0.03f);

    private float xRotation = -10f;

    // UI 관련
    public GameObject inventoryGameObject;
    public GameObject questGameObject;
    public GameObject minimapCamera;

    
    // minimapActive는 minimapCamera가 비활성화(!activeSelf)일 때 true로 설정

    // isUIActive에 minimapActive 포함
    private bool isUIActive;

    // Crosshair 관련
    public Sprite crosshairSprite;
    private Canvas crosshairCanvas;
    private Image crosshairImage;

    // UI 상호작용
    private EventSystem eventSystem;
    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerData;

    void Start()
    {
        gameManager.OnGameOver += GameOver;
        playerTransform = player.transform;
        playerMovement = player.GetComponent<PlayerMovement>();

        if (gameManager.IsGameStart)
        {
            transform.SetParent(headRigTransform, true);
            transform.localRotation = headRigRotation;

            StartCoroutine(GameStartCoroutine());
        }
    }

    void Update()
    {
        if (gameManager.IsGameOver || gameManager.IsGameStart) return;
        
        // minimapCamera가 비활성화일 때 minimapActive = true

        isUIActive = uiManager.isUIActive;
        
        if (!isUIActive)
        {
            // UI 비활성 상태일 때만 카메라 회전 처리
            float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -xRotationLimit, xRotationLimit);

            playerTransform.Rotate(Vector3.up * mouseX);
            transform.localRotation = Quaternion.Euler(90 + xRotation, 0f, 0f);
        }

        //HandleCrosshairUIInteraction();
    }

    
    

    
/*
    private void HandleCrosshairUIInteraction()
    {
        // UI 활성 상태이면 커서로 자유롭게 UI 상호작용 (Unity 기본 UI 상호작용)
        if (isUIActive || Cursor.visible)
        {
            eventSystem.SetSelectedGameObject(null);
            return;
        }
        
    }
*/
    void GameOver()
    {
        StartCoroutine(HeadAnimationCoroutine(gameManager.gameOverAnimationDuration));
    }

    IEnumerator GameStartCoroutine()
    {
        yield return new WaitForSeconds(gameManager.gameStartAnimationDuration);
        playerMovement.GameStartTrigger();
        transform.SetParent(headTransform, true);
        transform.localPosition = headPosition;
        
        var duration = gameManager.gameStartDuration - gameManager.gameStartAnimationDuration;
        var timer = 0f;
        var startLocalRot = transform.localRotation;
        var endLocalRot = Quaternion.Euler(80, 0, 0);
            
        while (timer < duration)
        {
            transform.localRotation = Quaternion.Slerp(startLocalRot, endLocalRot, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator HeadAnimationCoroutine(float duration)
    {
        transform.SetParent(headRigTransform, true);
        transform.localRotation = headRigRotation;

        yield return new WaitForSeconds(duration);

        transform.SetParent(headTransform, true);
        transform.localPosition = headPosition;
        transform.localRotation = Quaternion.Euler(90, 0, 0f);
    }
}
