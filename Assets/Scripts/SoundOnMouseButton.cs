using UnityEngine;
using System.Collections;

public class SoundOnMouseButton : MonoBehaviour {

    public AudioClip valvesfx;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

    }

    public void PlayShooting()
    {
        
    }
}


