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

public class PlayerSpawnManager : NetworkBehaviour
{
    public float randomPosition = 10f;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            var spawnPoints = InGameManager.Instance.spawnPoints;

            var newPoint = GetRandomSpawnPoint(spawnPoints);
            transform.position = newPoint.Position;
            transform.rotation = newPoint.Rotation;

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
        Vector3 pos = spawnPoints[index].position;

        pos += Vector3.up * 1.5f;
        pos += new Vector3(Random.Range(-randomPosition, randomPosition), 0f, Random.Range(-randomPosition, randomPosition));
        
        return new SpawnInfo(pos, spawnPoints[index].rotation);
    }
}