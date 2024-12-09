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
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
                UpdateWeaponAnimator(weaponSlot, true);
            }
            else
            {
                weaponSlot.SetActive(false);
                UpdateWeaponAnimator(weaponSlot, false);
            }
        }

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
    // Cari slot kosong terlebih dahulu
    GameObject emptySlot = weaponSlots.Find(slot => slot.transform.childCount == 0);

    if (emptySlot != null)
    {
        // Jika ada slot kosong, tambahkan senjata ke slot kosong
        AddWeaponToSlot(pickedupWeapon, emptySlot);

        // Langsung set slot kosong ini sebagai aktif
        SetSlotAsActive(emptySlot);
    }
    else
    {
        // Jika tidak ada slot kosong, tambahkan ke slot aktif
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    // Perbarui UI durability untuk senjata baru
    Weapon weapon = pickedupWeapon.GetComponent<Weapon>();
    if (weapon != null)
    {
        DurabilityManager.Instance.ResetDurabilityDisplay(weapon.durability);
    }
}

private void SetSlotAsActive(GameObject slot)
{
    if (slot != null)
    {
        activeWeaponSlot = slot;

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

        if (weapon.animator != null)
        {
            weapon.animator.enabled = true;
        }
    }

    private void AddWeaponToSlot(GameObject pickedupWeapon, GameObject targetSlot)
    {
        pickedupWeapon.transform.SetParent(targetSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = false;

        if (weapon.animator != null)
        {
            weapon.animator.enabled = false;
        }
    }

    internal void PickupAmmo(AmmoBox ammo)
    { 
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
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
                DurabilityManager.Instance.SetDurabilityUIActive(false);

                if (weapon.animator != null)
                {
                    weapon.animator.enabled = false;
                }

                weapon.isActiveWeapon = false;
            }

            Destroy(weaponToDrop);
        }
    }

    public void SwitchActivationSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;

            if (currentWeapon.animator != null)
            {
                currentWeapon.animator.enabled = false;
            }
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;

            if (newWeapon.animator != null)
            {
                newWeapon.animator.enabled = true;
            }

            DurabilityManager.Instance.ResetDurabilityDisplay(newWeapon.durability);
        }
        else
        {
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
