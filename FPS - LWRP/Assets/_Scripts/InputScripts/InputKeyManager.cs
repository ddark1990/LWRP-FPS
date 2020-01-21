using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyInputs", menuName = "Create KeyInputs")]
public class InputKeyManager : ScriptableObject
{
    [Header("Player Control Input")]
    public KeyCode WalkForwardKey = KeyCode.W;
    public KeyCode WalkBackwardKey = KeyCode.S;
    public KeyCode WalkLeftKey = KeyCode.A;
    public KeyCode WalkRightKey = KeyCode.D;
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode CrouchKey = KeyCode.LeftControl;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public KeyCode InteractableKey = KeyCode.E;
    public KeyCode OpenInventoryKey = KeyCode.Tab;
    public KeyCode LeanRightKey = KeyCode.E;
    public KeyCode LeanLeftKey = KeyCode.Q;
    [Space]
    [Header("Weapon Controls")]
    public KeyCode AttackKey = KeyCode.Mouse0;
    public KeyCode ReloadKey = KeyCode.R;
    public KeyCode LookDownSightsKey = KeyCode.Mouse1;
    [Space]
    [Header("Placement Controls")]
    public KeyCode RotatePreviewKey = KeyCode.R;
    public KeyCode PlacementKey = KeyCode.Mouse0;
}
