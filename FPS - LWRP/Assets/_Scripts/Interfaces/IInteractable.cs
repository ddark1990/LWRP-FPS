using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void OnPickedUp();
    void OnDroped();
    void OnEat();
    void OnInteract();
}
