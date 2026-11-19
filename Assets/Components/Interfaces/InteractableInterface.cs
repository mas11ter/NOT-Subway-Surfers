using UnityEngine;

//add a trigger collidor and set the object's layer to be "Interactable"
public interface IInteractableInterface
{
    void OnHoveredStart(GameObject Hovered);
    void OnHoveredEnd(GameObject Hovered);



}
