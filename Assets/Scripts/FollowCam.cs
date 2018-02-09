using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Vector3 CamRotation = new Vector3(45, -45, 0);
    public Vector3 CamPosition = new Vector3(6.5f, 10f, -6.5f);

    public Transform targetTr;


    // 추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기 위해 LateUpdate 사용
    void LateUpdate()
    {
        transform.position = targetTr.position + CamPosition;
        transform.localRotation = Quaternion.Euler(CamRotation);
    }
}
