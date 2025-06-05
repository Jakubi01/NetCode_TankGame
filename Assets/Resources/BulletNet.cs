using Unity.Netcode;
using UnityEngine;

public class BulletNet : NetworkBehaviour
{
    private Rigidbody _rb;
    private const int Score = 100;
    
    [HideInInspector]
    public GameObject launchPoint;

    public GameObject explosionParticle;
    
    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        
        _rb = GetComponent<Rigidbody>();
        
        ShotBulletRpc();
        Invoke(nameof(KillBulletRpc), 3.0f);
    }
    
    [Rpc(SendTo.Everyone)]
    private void ShotBulletRpc()
    {
        if (!_rb)
        {
            _rb = GetComponent<Rigidbody>();
        }

        _rb.AddForce(launchPoint.transform.forward * BeginGameManager.Instance.BulletSpeed);
    }

    [Rpc(SendTo.Server)]
    private void KillBulletRpc()
    {
        NetworkObject.Despawn();
    }
    
    [Rpc(SendTo.Everyone)]
    private void SpawnParticleRpc(Vector3 position, Quaternion rotation)
    {
        if (!explosionParticle)
        {
            return;
        }
        
        GameObject particle = Instantiate(explosionParticle, position, rotation);
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        
        if (!ps)
        {
            return;
        }
        
        ps.Play();
        Destroy(particle, ps.main.duration + ps.main.startLifetime.constantMax);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealthNet>().DecHealthRpc();
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerScoreManager>().AddScoreServerRpc(Score);
        }

        SpawnParticleRpc(transform.position, transform.rotation);
        KillBulletRpc();
    }
}
