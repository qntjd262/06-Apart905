using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerStat player);
    string GetInteractText();
    Constants.InteractType GetInteractType();
}
 