using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource shootingSound1911;
    public AudioSource reloadSound1911;
    public AudioSource emptySound1911;


    private void Awake()
    {
        if (Instance != null && Instance != this) // Corrected `id` to `if`
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}

