using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    public Animator InventoryAnimator;
    public GoomerFPSController.FPSController FPSController;

    public AudioSource AudioSource;
    public AudioClip SlotHoverSound;

    private PhotonView PV;

    private void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected && !PV.IsMine) return;

        if(Input.GetKeyDown(InputManager.Instance.InputKeyManager.OpenInventoryKey))
        {
            InventoryManager.Instance.MenuOpen = !InventoryManager.Instance.MenuOpen;
            FPSController.cameraLocked = !FPSController.cameraLocked;
            FPSController.cursorIsLocked = !FPSController.cursorIsLocked;
            InventoryAnimator.SetBool("MenuOpen", InventoryManager.Instance.MenuOpen);
        }
    }

    public void PlayOnHoverSlotSound(float volumeScale)
    {
        AudioSource.PlayOneShot(SlotHoverSound, volumeScale);
    }

}
