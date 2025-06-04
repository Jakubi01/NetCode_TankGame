using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    // TODO : spawn point y 값 조정
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
            }
        }
        
        base.OnNetworkSpawn();
    }
}
