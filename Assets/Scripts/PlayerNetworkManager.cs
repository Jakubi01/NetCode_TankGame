using System.Collections;
using System.Collections.Generic;
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
                InGameManager.Instance.ScoreCache[OwnerClientId] = 
                    (gameObject.GetComponent<PlayerScoreManager>().userId.Value.Value
                        , gameObject.GetComponent<PlayerScoreManager>().score.Value);
            }
        }
        
        base.OnNetworkSpawn();
    }

    private IEnumerator SubmitReadyServer()
    {
        // wait for PlayerReadyState init
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerReadyState>().SubmitReadyServerRpc();
    }
}