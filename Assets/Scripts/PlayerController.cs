using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private GameObject _bulletPrefabFile;
    private const string PrefabName = "Bullet";
    private bool _bCanShoot = true;
    private const float ShootCooldown = 1f;
    
    [SerializeField]
    private GameObject spawnPoint;

    void Start()
    {
        _bulletPrefabFile = Resources.Load(PrefabName) as GameObject;
    }
    
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        Movement();
        
        if (Input.GetKeyDown(KeyCode.Space) && _bCanShoot)
        {
            MakeBulletRpc();
            StartCoroutine(nameof(ShootCooldownRoutine));
        }
    }

    private void Movement()
    {
        float zOff = Input.GetAxis("Vertical") * BeginSceneGameManager.Instance.SpeedTank * Time.deltaTime;
        transform.Translate(0.0f, 0.0f, zOff);
        
        float angleOff = Input.GetAxis("Horizontal") * BeginSceneGameManager.Instance.RotSpeedTank * Time.deltaTime;
        transform.Rotate(0.0f, angleOff, 0.0f);
    }

    [Rpc(SendTo.Server)]
    private void MakeBulletRpc()
    {
        GameObject prefab = Instantiate(_bulletPrefabFile);
        prefab.GetComponent<BulletNet>().launchPoint = spawnPoint;
        
        prefab.transform.position = spawnPoint.transform.position;
        prefab.transform.rotation = spawnPoint.transform.rotation;
        prefab.GetComponent<NetworkObject>().Spawn();
    }

    private IEnumerator  ShootCooldownRoutine()
    {
        _bCanShoot = false;
        yield return new WaitForSeconds(ShootCooldown);
        _bCanShoot = true;
    }
}
