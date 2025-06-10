using System.Collections;
using Unity.Netcode;
using UnityEngine;

public struct SpawnInfo
{
    public Vector3 Position;
    public Quaternion Rotation;

    public SpawnInfo(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public class PlayerNetworkManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            var spawnPoints = InGameManager.Instance.spawnPoints;

            var newPoint = GetRandomSpawnPoint(spawnPoints);
            transform.position = newPoint.Position;
            transform.rotation = newPoint.Rotation;

            StartCoroutine(nameof(SubmitReadyServer));
            InGameManager.Instance.ConnectedUserNum++;
        }
        
        base.OnNetworkSpawn();
    }

    private SpawnInfo GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return new SpawnInfo();
        }

        int index = Random.Range(0, spawnPoints.Length);
        return new SpawnInfo(spawnPoints[index].position, spawnPoints[index].rotation);
    }
    
    private IEnumerator SubmitReadyServer()
    {
        // wait for PlayerReadyState init
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerReadyState>().SubmitReadyServerRpc();
    }
}