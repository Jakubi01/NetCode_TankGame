using UnityEngine;
using Unity.Netcode;

public class RotTurret : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        int keyType = 0;
        
        if (Input.GetKey(KeyCode.J))
        {
            keyType = -1;
        }
        else if (Input.GetKey(KeyCode.L))
        {
            keyType = 1;
        }

        float angleOff = keyType * BeginSceneGameManager.Instance.RotSpeedTank * Time.deltaTime;
        transform.Rotate(0.0f, angleOff, 0.0f);
    }
}
