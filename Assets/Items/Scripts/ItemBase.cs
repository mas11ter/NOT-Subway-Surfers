using TMPro;
using UnityEngine;

public class ItemBase : MonoBehaviour, IInteractableInterface
{
    //public Canvas Canvas;

    [SerializeField] protected TextMeshProUGUI Item_Name;
    [SerializeField] protected TextMeshProUGUI Item_Description;
    [SerializeField] protected TextMeshProUGUI Item_Pickup;

    public AudioClip[] InteractionSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        //Canvas.enabled = false;
    }


    protected virtual void OnItemInteracted(GameObject Interacted) { }

    void IInteractableInterface.OnHoveredStart(GameObject Hovered)
    {
        if (InteractionSFX.Length != 0)
        {
            AudioSource.PlayClipAtPoint(InteractionSFX[Random.Range(0, InteractionSFX.Length - 1)], transform.position);
        }
        OnItemInteracted(Hovered);

    }

    void IInteractableInterface.OnHoveredEnd(GameObject Hovered)
    {
    }

}
