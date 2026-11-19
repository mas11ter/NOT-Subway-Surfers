using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class DeathBox : MonoBehaviour, IInteractableInterface
{

    void IInteractableInterface.OnHoveredEnd(GameObject Hovered)
    {

    }

    void IInteractableInterface.OnHoveredStart(GameObject Hovered)
    {
        StatusComponent characterController = Hovered.GetComponent<StatusComponent>();
        characterController.TakeDamage(new DamageInfo(99999999999, gameObject, transform.position));
    }


}
