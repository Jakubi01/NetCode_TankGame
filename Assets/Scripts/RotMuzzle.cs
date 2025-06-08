using System.Linq.Expressions;
using UnityEngine;
using Unity.Netcode;

public class RotMuzzle : NetworkBehaviour
{
    private const float AngleMax = 0.0f;
    private const float AngleMin = -15.0f;
    
    void Update()
    {
        if (!gameObject || !BeginGameManager.Instance)
        {
            return;
        }
        
        int keyType = 0;
        
        if (Input.GetKey(KeyCode.I))
        {
            keyType = 1;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            keyType = -1;
        }
        
        Vector3 rot = transform.localEulerAngles;
        float angleOff = -keyType * BeginGameManager.Instance.RotSpeedTank * Time.deltaTime;
        double xAngle = rot.x;
        xAngle += angleOff;

        if (xAngle > 180)
        {
            xAngle -= 360;
        }
        if (xAngle >= AngleMin && xAngle <= AngleMax)
        {
            transform.Rotate(angleOff, 0.0f, 0.0f);
        }
    }
}
