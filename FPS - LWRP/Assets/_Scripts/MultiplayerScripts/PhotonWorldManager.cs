using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonWorldManager : MonoBehaviourPunCallbacks
{
    private const string NETWORK_PLAYER = "NetworkPlayer";


    private void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(NETWORK_PLAYER, new Vector3(0, 1, 0), Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PopupInfo.Instance.PopInfo();
    }
}
