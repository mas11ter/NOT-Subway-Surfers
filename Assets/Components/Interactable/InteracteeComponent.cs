using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteracteeComponent : MonoBehaviour
{
    public Canvas GenericCanvas;
    public GameObject GenericInteractionPanel;

    private List<GameObject> HoveredObjects = new();
    private Canvas CustomCanvas;

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.TryGetComponent<IInteractableInterface>(out IInteractableInterface Pickupable))
        {
            Pickupable.OnHoveredStart(gameObject);
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.TryGetComponent<IInteractableInterface>(out IInteractableInterface Pickupable))
        {
            Pickupable.OnHoveredEnd(gameObject);
        }
    }

}
