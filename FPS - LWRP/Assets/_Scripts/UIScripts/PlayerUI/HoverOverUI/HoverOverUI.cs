using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HoverOverUI : MonoBehaviour
{
    public Text ObjectNameText;

    public Image activeHoverImg;

    public Image PickUpHandIconImg;
    public Image InteractableIconImg;
    public Image CenterDotImg;

    public bool HoveringOverObject;

    [Header("Cache")]
    public Animator animator;
    public PlayerSelection playerSelection;
    
    private static readonly int HoveringOver = Animator.StringToHash("HoveringOver");
    private PhotonView _photonView;

    private void Start()
    {
        _photonView = transform.root.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected && !_photonView.IsMine) return;

        UpdateHoverOver();
    }

    private void UpdateHoverOver()
    {
        //update animator
        animator.SetBool(HoveringOver, HoveringOverObject); 

        //default reset
        if (!PlayerSelection.Instance.objectInView)
        {
            HoveringOverObject = false;
            return;
        }

        //set hover info based on what object you are looking at
        if(PlayerSelection.Instance.itemInView)
        {
            activeHoverImg = PickUpHandIconImg;
            ObjectNameText.text = playerSelection.itemInView.ItemType.itemData.ItemName.ToUpper(); 
        }
        if (PlayerSelection.Instance.interactableInView)
        {
            activeHoverImg = InteractableIconImg;
            ObjectNameText.text = $"Interact With: {PlayerSelection.Instance.interactableInView.name}";
        }

        HoveringOverObject = true;
    }
}
