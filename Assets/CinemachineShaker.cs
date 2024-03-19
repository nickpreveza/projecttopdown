using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShaker : MonoBehaviour
{
    public static CinemachineShaker Instance;
    private CinemachineVirtualCamera vCam;
    float internalTimer;
    bool effectActive;
    private void Awake()
    {
        Instance = this;
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        internalTimer = time;
        effectActive = true;

    }

    private void Update()
    {
        if (effectActive)
        {
            internalTimer -= Time.deltaTime;
            if (internalTimer < 0)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
              vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
                effectActive = false;
            }
        }
       
    }
}
