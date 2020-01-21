using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMenuController : MonoBehaviourPunCallbacks
{
    public int roomCount;

    [Header("Cache")]
    public Button JoinWorldButton;
    public Text ConnectionInfoText;
    public Text PingText;

    private const string WORLD_SCENE = "WorldScene";


    private void Start()
    {
        JoinWorldButton.interactable = false;

        var playerName = "Noob" + Random.Range(0, 10000); 

        PhotonNetwork.ConnectUsingSettings();
        InvokeRepeating("UpdateConnectionInfo", 1f, 0.25f);

        PhotonNetwork.NickName = playerName;
    }

    void UpdateConnectionInfo()
    {
        ConnectionInfoText.text = "Connection Status : " + PhotonNetwork.NetworkClientState.ToString();
        PingText.text = PhotonNetwork.GetPing().ToString();
        roomCount = PhotonNetwork.CountOfRooms;
    }

    public void OnEnterWorldPress()
    {
        var roomName = "Room" + Random.Range(1, 1000);
        RoomOptions options = new RoomOptions { MaxPlayers = 100 };

        if (PhotonNetwork.CountOfRooms > 0) PhotonNetwork.JoinRandomRoom();
        else PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");

        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        JoinWorldButton.interactable = true;
    }

    public override void OnCreatedRoom()
    {

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(WORLD_SCENE);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }
}
