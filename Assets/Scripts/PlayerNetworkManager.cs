using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            var spawnPoints = InGameManager.Instance.spawnPoints;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[randomIndex];
                
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;

                StartCoroutine(nameof(SubmitReadyServer));
                InGameManager.Instance.ConnectedUserNum++;
            }
        }
        
        base.OnNetworkSpawn();
    }

    public Vector3 GetRandomSpawnPoint()
    {
        var spawnPoints = InGameManager.Instance.spawnPoints;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return Vector3.zero;
        }

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index].position;
    }
    
    private IEnumerator SubmitReadyServer()
    {
        // wait for PlayerReadyState init
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerReadyState>().SubmitReadyServerRpc();
    }
}