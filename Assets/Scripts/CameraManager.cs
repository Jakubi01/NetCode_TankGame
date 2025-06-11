using Unity.Netcode;
using UnityEngine;

public class CameraManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject player;
    
    public float sensitivity = 5f;
    private bool _bIsDragging;
    private float _currentAngle;
    private const float Distance = 15f;
    private const float Height = 5f;
    private const float FixedXAngle = 20f;

    private void Update()
    {
        if (!player)
        {
            TryAssignPlayer();
            return;
        }

        HandleMouseInput();
        UpdateCameraPosition();
    }
    
    private void TryAssignPlayer()
    {
        if (!NetworkManager.Singleton || NetworkManager.Singleton.SpawnManager == null)
        {
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > 0)
            {
                player = NetworkManager.Singleton.ConnectedClients[0].PlayerObject?.gameObject;
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()?.gameObject;
        }

        transform.rotation = Quaternion.Euler(80, 0, 0);
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _bIsDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _bIsDragging = false;
        }

        if (_bIsDragging)
        {
            float mouseX = Input.GetAxis("Mouse X");
            _currentAngle += mouseX * sensitivity;
        }
    }

    private void UpdateCameraPosition()
    {
        if (!player)
        {
            return;
        }
        
        Quaternion rotation = Quaternion.Euler(FixedXAngle, _currentAngle, 0);

        Vector3 offset = rotation * new Vector3(0, 0, -Distance);

        transform.position = player.transform.position + offset + Vector3.up * Height;
        transform.rotation = rotation;
    }
}
