using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Transform playerTransform;

    InputManager inputManager;

    PlayerMovement_RealGame playerMovement;

    public Transform cameraTransform; 

    public float minPivotAngle= -30;

    public float maxPivotAngle = 30;

    public Transform cameraPivot;

    Vector3 cameraFollowVelocity = Vector3.zero;

    float cameraFollowSpeed = 0.3f;

    public float lookAngle; // Up ANd DOwn Movement OF Camera

    public float pivotAngle; // Left And Rifght Movement of Camera

    public float cameraLookSpeed = 2f;

    public float camPivotSpeed = 2f;

    public LayerMask collisionLayer;

    float defaultPosition;
    
    public float cameraCollisionOffset = 0.2f;

    public float minCollisionOffset = 0.2f;

    public float colliderRadius = 2;

    Vector3 cameraVectorPosition;

    public GameObject scopeCanvas;

    bool isScoped = false;

    public GameObject playerUI;
    public Camera mainCam;

    float originalFOV = 60;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;   
        Cursor.visible = false;
        playerMovement = FindObjectOfType<PlayerMovement_RealGame>();
        playerTransform = FindObjectOfType<PlayerManager>().transform;
        inputManager = FindObjectOfType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllFunctions()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollision();
        isPlayerScoped();
    }
     void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;

    }

     void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        lookAngle = lookAngle + inputManager.horizontalCameraInput * cameraLookSpeed;
        pivotAngle  = pivotAngle + inputManager.verticalCameraInput * cameraLookSpeed;
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);
        rotation = Vector3.zero;
        rotation.y = lookAngle;

        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
        if(!playerMovement.isMoving && !playerMovement.isSprinting )
        {
            playerTransform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
    }

    void HandleCameraCollision()
    {
        float targetPosition = defaultPosition;

        RaycastHit hit;

        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();
       if (Physics.SphereCast(cameraPivot.transform.position, colliderRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);

        }
        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition -= minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    public void  isPlayerScoped()
    {
       /* if(inputManager.scopeInput)
        {
            scopeCanvas.SetActive(true);
            playerUI .SetActive(false);
            mainCam.fieldOfView = 30;
        }
        else
        {
            scopeCanvas.SetActive(false);
            playerUI.SetActive(true);
            mainCam.fieldOfView = originalFOV;
        }*/
    }
}
