using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DurabilityManager : MonoBehaviour
{
    public static DurabilityManager Instance { get; set; }

    public TextMeshProUGUI durabilityDisplay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Memperbarui tampilan durabilitas
    public void UpdateDurabilityDisplay(int durability)
    {
        if (durabilityDisplay != null)
        {
            // Tampilkan "0" jika durability <= 0
            durabilityDisplay.text = durability <= 0 ? "0" : $"{durability}";

            // Nonaktifkan UI jika durabilitas habis
            if (durability <= 0)
            {
                SetDurabilityUIActive(false);
            }
        }
    }

    // Mengaktifkan atau menonaktifkan UI durabilitas
    public void SetDurabilityUIActive(bool isActive)
    {
        if (durabilityDisplay != null)
        {
            durabilityDisplay.gameObject.SetActive(isActive);
        }
    }

    // Reset UI untuk senjata baru
    public void ResetDurabilityDisplay(int newDurability)
    {
        if (durabilityDisplay != null)
        {
            SetDurabilityUIActive(true); // Aktifkan kembali UI
            UpdateDurabilityDisplay(newDurability); // Perbarui nilai durabilitas
        }
    }
}
