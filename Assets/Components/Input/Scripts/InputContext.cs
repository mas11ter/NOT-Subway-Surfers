
using System;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "InputContext")]
public class InputContext : ScriptableObject, InputMapping.IPlayerActions, InputMapping.IUIActions
{

    InputMapping MyinputMapping;
    private static string InteractionKey;
    

    public static string GetInteractionKey()
    {
        return InteractionKey;
    }

    private void OnEnable()
    {
        if (MyinputMapping == null)
        {
            MyinputMapping = new InputMapping();
            MyinputMapping.Player.SetCallbacks(this);
            MyinputMapping.UI.SetCallbacks(this);

            SetGameInput();

            foreach (var I in MyinputMapping.Player.Interact.bindings)
            {
                if (I.groups == "Keyboard&Mouse")
                {
                    InteractionKey = I.path.Split("/", StringSplitOptions.None)[1].ToUpper();
                }
            }
        }
    }

    private void OnDisable()
    {
        MyinputMapping.Player.RemoveCallbacks(this);
        MyinputMapping.UI.RemoveCallbacks(this);

        MyinputMapping.UI.Disable();
        MyinputMapping.Player.Disable();
    }

    public void SetGameInput()
    {
        MyinputMapping.Player.Enable();
        MyinputMapping.UI.Disable();
    }

    public void SetUIInput()
    {
        MyinputMapping.Player.Disable();
        MyinputMapping.UI.Enable();
    }

    public event Action<Vector2> OnMoveEvent;

    public event Action OnAttackEvent;
    public event Action OnAttackReleaseEvent;

    public event Action OnInteractEvent;

    public event Action OnJumpEvent;
    public event Action OnJumpReleaseEvent;

    public event Action OnSprintEvent;
    public event Action OnSprintReleaseEvent;

    public event Action OnPauseEvent;
    public event Action OnResumeEvent;

    public event Action OnClickEvent;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAttackEvent?.Invoke();
        }
        else if (context.canceled)
        {
            OnAttackReleaseEvent?.Invoke();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInteractEvent?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            OnJumpEvent?.Invoke();
        }
        else if (context.canceled)
        {
            OnJumpReleaseEvent?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSprintEvent?.Invoke();
        }
        else if (context.canceled)
        {
            OnSprintReleaseEvent?.Invoke();
        }
    }

    // UI

    public void OnClick(InputAction.CallbackContext context)
    {
        OnClickEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPauseEvent?.Invoke();
        }
        else if (context.canceled)
        {
            OnResumeEvent?.Invoke();
        }
    }
}
