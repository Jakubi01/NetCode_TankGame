using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BulletNet : NetworkBehaviour
{
    public ulong ClientId { get; set; }
    
    private Rigidbody _rb;
    private const int Score = 100;
    private int _bulletDamage;
    private AudioSource _bulletHitSound;
    
    [HideInInspector]
    public GameObject launchPoint;

    public GameObject explosionParticle;

    private void Awake()
    {
        _bulletHitSound = GetComponent<AudioSource>();
    }
    
    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        
        _rb = GetComponent<Rigidbody>();
        _bulletDamage = InGameManager.Instance.BulletDamage;
        
        ShotBulletRpc();
        Invoke(nameof(KillBulletServerRpc), 3.0f);
    }

    private IEnumerator WaitForBulletHitSoundEnd()
    {
        var component = GetComponent<Collider>();
        if (component)
        {
            component.enabled = false;
        }

        if (_bulletHitSound)
        {
            _bulletHitSound.volume = SoundManager.Instance.soundVolume;
            _bulletHitSound.Play();
            yield return new WaitForSeconds(_bulletHitSound.clip.length);
        }
        
        gameObject.SetActive(false);
        KillBulletServerRpc();
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

    [ServerRpc]
    private void KillBulletServerRpc()
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
        if (other.gameObject.CompareTag("Player"))
        {
            if (IsServer)
            {
                other.gameObject.GetComponent<PlayerHealthNet>().DecHealthRpc(_bulletDamage);
                NetworkManager.Singleton.ConnectedClients[ClientId].PlayerObject.GetComponent<PlayerScoreManager>()
                    .AddScoreServerRpc(ClientId, Score);
            }
        }

        SpawnParticleRpc(transform.position, transform.rotation);
        StartCoroutine(nameof(WaitForBulletHitSoundEnd));
    }
}
