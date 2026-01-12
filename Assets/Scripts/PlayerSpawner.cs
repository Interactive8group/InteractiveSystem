using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] Vector2 masterSpawnPos = new Vector2(-3f, 0f);
    [SerializeField] Vector2 clientSpawnPos = new Vector2(3f, 0f);

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos;

        if (PhotonNetwork.IsMasterClient)
            spawnPos = masterSpawnPos;   // ホスト側
        else
            spawnPos = clientSpawnPos;   // 相手側

        PhotonNetwork.Instantiate(
            "Player",
            spawnPos,
            Quaternion.identity
        );
    }
}
