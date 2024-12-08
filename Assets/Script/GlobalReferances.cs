using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferances : MonoBehaviour
{
    public static GlobalReferances Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;

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
