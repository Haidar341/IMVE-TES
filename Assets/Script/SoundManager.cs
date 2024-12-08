using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // AudioSources for shooting sounds
    public AudioSource ShootingChannel1911;
    public AudioSource ShootingChannelM4;

    // AudioClips for shooting
    public AudioClip M1911Shot;
    public AudioClip M4Shot;

    // AudioSources for reload sounds
    public AudioSource reloadSoundM4;
    public AudioSource reloadSound1911;
    public AudioSource emptySound;

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

    public void PlayEmptySound()
    {
        emptySound.Play();
    }

    public void PlayShootingSound(Weapon.WeaponModel weapon)
    {
        switch (weapon)
        {
            case Weapon.WeaponModel.M1911:
                ShootingChannel1911.PlayOneShot(M1911Shot);
                break;
            case Weapon.WeaponModel.M4:
                ShootingChannelM4.PlayOneShot(M4Shot);
                break;
        }
    }

    public void PlayReloadSound(Weapon.WeaponModel weapon)
    {
        switch (weapon)
        {
            case Weapon.WeaponModel.M1911:
                reloadSound1911.Play();
                break;
            case Weapon.WeaponModel.M4:
                reloadSoundM4.Play();
                break;
        }
    }
}
