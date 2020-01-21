using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfo : MonoBehaviour
{
    public static PopupInfo Instance;

    public int PopupTimer;

    [Header("Cache")]
    [SerializeField] private Button PopUpInfoButton;
    [SerializeField] private Text infoText;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        DontDestroyOnLoad(this);
    }

    public void PopInfo()
    {
        animator.SetTrigger("PopInfo");
        animator.SetInteger("PopupTimer", PopupTimer);

        StartCoroutine(StartTimer(PopupTimer));
    }

    private IEnumerator StartTimer(int timer)
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
        }

        animator.SetInteger("PopupTimer", timer);
    }

    public void ClosePopup()
    {
        animator.SetInteger("PopupTimer", 0);
    }
}
