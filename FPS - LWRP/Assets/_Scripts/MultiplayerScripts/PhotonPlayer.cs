using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviourPunCallbacks
{
    public string PlayerName;
    public int PlayerID;

    [Header("Cache")]
    public GameObject playerPrefab;

    private void Start()
    {
        if (!photonView.IsMine) return;

        photonView.RPC("RPC_SendPlayerData", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void RPC_SendPlayerData()
    {
        ////var
        var spawnPosition = new Vector3(0, 2, 0);

        ////Player Data
        PlayerName = photonView.Owner.NickName;
        PlayerID = photonView.Owner.ActorNumber;

        ////GameObject Data
        gameObject.name = PlayerName;

        ////PlayerPrefab Logic
        var _playerPrefab = CreatePlayerPrefab(playerPrefab, spawnPosition, Quaternion.identity);
        _playerPrefab.transform.SetParent(transform);
        //Transfer Ownership/ID To PlayerPrefab
        _playerPrefab.GetPhotonView().TransferOwnership(photonView.Owner);
        _playerPrefab.GetPhotonView().ViewID = photonView.ViewID + 1;
    }

    private GameObject CreatePlayerPrefab(GameObject _playerPrefab, Vector3 _spawnPos, Quaternion _spawnRot)
    {
        return Instantiate(_playerPrefab, _spawnPos, _spawnRot);
    }
}