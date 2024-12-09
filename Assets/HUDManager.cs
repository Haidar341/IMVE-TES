using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;
    public Sprite emptySlot;

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

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        GameObject unActiveWeaponSlot = GetUnActiveWeaponSlot();
        Weapon unActiveWeapon = unActiveWeaponSlot != null ? unActiveWeaponSlot.GetComponentInChildren<Weapon>() : null;

        if (activeWeapon != null)
        {
            // Update ammo UI
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletPerBurst}";
            totalAmmoUI.text = $"{activeWeapon.magazineSize / activeWeapon.bulletPerBurst}";

            // Update ammo type UI
            ammoTypeUI.sprite = GetAmmoSprite(activeWeapon.thisWeaponModel);

            // Update weapon UI
            activeWeaponUI.sprite = GetWeaponSprite(activeWeapon.thisWeaponModel);

            if (unActiveWeaponUI && unActiveWeapon != null)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
            else
            {
                unActiveWeaponUI.sprite = null; // Clear sprite if no unactive weapon
            }
        }
        else
        {
            // Clear ammo UI when no weapon is active
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;
    
            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M1911:
                return Resources.Load<GameObject>("M1911_Weapon").GetComponent<SpriteRenderer>().sprite;

            case Weapon.WeaponModel.M4:
                return Resources.Load<GameObject>("M4_Weapon").GetComponent<SpriteRenderer>().sprite;

            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M1911:
                return Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;

            case Weapon.WeaponModel.M4:
                return Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;

            default:
                return null;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null;
    }
}
