using UnityEngine;
using UnityEngine.Events;

public class FinishLine : MonoBehaviour, IInteractableInterface
{
    private static FinishLine Instance;
    public UnityEvent OnReachedFinishLine;

    public static FinishLine GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        Instance = this;
    }
    void IInteractableInterface.OnHoveredEnd(GameObject Hovered)
    {

    }

    void IInteractableInterface.OnHoveredStart(GameObject Hovered)
    {
        OnReachedFinishLine.Invoke();
        MovementComponent characterController = Hovered.GetComponent<MovementComponent>();
        characterController.ResetMovement();
    }


}
