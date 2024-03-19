using UnityEngine;
using System.Collections;

public class MovementSound : MonoBehaviour
{

    public AudioClip valvesfx;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

    }

    void Update()
    {

        if (Input.GetKeyDown("w"))
        {
            audioSource.enabled = true;
            if (!audioSource.isPlaying)
            {
                audioSource.clip = valvesfx;
                audioSource.Play();
            }
        }






    }

}
