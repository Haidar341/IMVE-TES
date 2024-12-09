using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

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

    private void Start() 
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        // Perbarui status slot senjata
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
                UpdateWeaponAnimator(weaponSlot, true); // Aktifkan animator pada senjata di slot aktif
            }
            else
            {
                weaponSlot.SetActive(false);
                UpdateWeaponAnimator(weaponSlot, false); // Nonaktifkan animator pada senjata di slot tidak aktif
            }
        }

        // Ganti slot aktif berdasarkan input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActivationSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActivationSlot(1);
        }
    }

    public void PickupWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);

        // Perbarui UI durability untuk senjata baru
        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();
        if (weapon != null)
        {
            DurabilityManager.Instance.ResetDurabilityDisplay(weapon.durability);
        }
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon();

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;

        // Aktifkan animator pada senjata yang baru diambil
        if (weapon.animator != null)
        {
            weapon.animator.enabled = true;
        }
    }

    internal void PickupAmmo(AmmoBox ammo)
    { 
        switch (ammo. ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break ;
        }
    }

    public void DropCurrentWeapon()
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            Weapon weapon = weaponToDrop.GetComponent<Weapon>();

            if (weapon != null)
            {
                // Nonaktifkan UI durability saat senjata dijatuhkan
                DurabilityManager.Instance.SetDurabilityUIActive(false);

                // Nonaktifkan animator pada senjata yang dijatuhkan
                if (weapon.animator != null)
                {
                    weapon.animator.enabled = false;
                }

                weapon.isActiveWeapon = false;
            }

            // Hancurkan senjata yang sedang dipegang
            Destroy(weaponToDrop);
        }
    }

    public void SwitchActivationSlot(int slotNumber)
    {
        // Nonaktifkan senjata di slot aktif sebelumnya
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;

            // Nonaktifkan animator senjata sebelumnya
            if (currentWeapon.animator != null)
            {
                currentWeapon.animator.enabled = false;
            }
        }

        // Ganti slot aktif
        activeWeaponSlot = weaponSlots[slotNumber];

        // Aktifkan senjata di slot baru jika ada
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;

            // Aktifkan animator senjata baru
            if (newWeapon.animator != null)
            {
                newWeapon.animator.enabled = true;
            }

            // Perbarui UI durability dengan durabilitas senjata baru
            DurabilityManager.Instance.ResetDurabilityDisplay(newWeapon.durability);
        }
        else
        {
            // Nonaktifkan UI durability jika slot kosong
            DurabilityManager.Instance.SetDurabilityUIActive(false);
        }
    }

    private void UpdateWeaponAnimator(GameObject weaponSlot, bool isActive)
    {
        if (weaponSlot.transform.childCount > 0)
        {
            Weapon weapon = weaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if (weapon != null && weapon.animator != null)
            {
                weapon.animator.enabled = isActive;
            }
        }
    }

internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
{
    switch (thisWeaponModel)
    {
        case Weapon.WeaponModel.M4:
            totalRifleAmmo -= bulletsToDecrease;
            break;
        case Weapon.WeaponModel.M1911:
            totalPistolAmmo -= bulletsToDecrease;
            break;
    }
}

public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
{
    switch (thisWeaponModel)
    {
        case Weapon.WeaponModel.M4:
            return totalRifleAmmo;

        case Weapon.WeaponModel.M1911:
            return totalPistolAmmo;

        default:
            return 0;
    }
}
}
