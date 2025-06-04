using Unity.Netcode;
using UnityEngine;

public class CameraManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject player;
    
    public float sensitivity = 5f;
    private bool _bIsDragging = false;
    private float _currentAngle = 0f;
    private const float Distance = 15f;
    private const float Height = 5f;

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
        if (!NetworkManager.Singleton|| NetworkManager.Singleton.SpawnManager == null)
            return;

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
        if (!player) return;

        float fixedXAngle = 20f;
        Quaternion rotation = Quaternion.Euler(fixedXAngle, _currentAngle, 0);

        // 플레이어로부터 떨어진 위치 계산 (Y축 회전에 따라 회전된 벡터)
        Vector3 offset = rotation * new Vector3(0, 0, -Distance);

        // 카메라 위치와 회전 적용
        transform.position = player.transform.position + offset + Vector3.up * Height;
        transform.rotation = rotation;
    }
}
