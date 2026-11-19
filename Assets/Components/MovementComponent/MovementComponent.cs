using UnityEngine;
using UnityEngine.Events;
using System.Collections;


[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour
{
    public float MovementSpeed;
    public float MovementSpeedY = 5;
    public bool bMoveWithRigidbody = false;
    public bool DisableYAxisMovement = true;
    public float Gravity = 10f;
    public bool bCanMove = true;
    public Animator animator;

    private Vector3 LastDirection = Vector3.zero;
    private CharacterController characterController;

    [HideInInspector] public UnityEvent<bool /*Negative axis*/> OnChangedDirection;
    [HideInInspector] public UnityEvent<Vector3 /*Direction*/, Vector3 /*Force*/> OnMovement;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    private float LastDirectionChange = 0f;

    public float GetObjectLookingAt()
    {
        return LastDirectionChange;
    }

    public void Look(Vector3 Direction)
    {
        if (Direction.x >= 0.01f && LastDirectionChange != 1)
        {
            OnChangedDirection.Invoke(false);
            LastDirectionChange = 1;
        }
        else if (Direction.x <= -0.01f && LastDirectionChange != -1)
        {
            OnChangedDirection.Invoke(true);
            LastDirectionChange = -1;
        }
    }

    public void ResetMovement()
    {
        LastDirection = Vector3.zero;
        characterController.transform.position = new Vector3(0, 0, -0.6f);
        bCanMove = false;
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(0.5f);
        bCanMove = true;
    }

    public void Move(Vector3 Direction)
    {
        if (!bCanMove)
        {
            return;
        }    
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.left);

        float curSpeedX = (MovementSpeed) * Direction.x;
        float curSpeedY = (MovementSpeedY) * 1;
        float movementDirectionY = LastDirection.y;
        LastDirection = (forward * curSpeedX) + (right * curSpeedY);


        if (!characterController.isGrounded)
        {
            LastDirection.y -= Gravity * Time.deltaTime;
        }
        animator.SetBool("bIsFalling", !characterController.isGrounded);
        characterController.Move(LastDirection * Time.deltaTime);



    }


    public Vector3 GetDirection()
    {
        return LastDirection;
    }


}
