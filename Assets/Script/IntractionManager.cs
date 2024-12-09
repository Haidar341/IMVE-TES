using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntractionManager : MonoBehaviour
{
    public static IntractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null;
    public float maxRayDistance = 5f; // Jarak maksimum raycast

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

    public void Update()
    {
        // Buat ray ke arah depan kamera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Lakukan raycast
        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;

            // Jika objek memiliki komponen Weapon
            if (objectHitByRaycast.GetComponent<Weapon>() && objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                if (hoveredWeapon != null && hoveredWeapon != objectHitByRaycast.GetComponent<Weapon>())
                {
                    // Nonaktifkan outline pada weapon sebelumnya
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }

                // Tetapkan weapon yang baru dan aktifkan outline
                hoveredWeapon = objectHitByRaycast.GetComponent<Weapon>();
                hoveredWeapon.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                WeaponManager.Instance.PickupWeapon(objectHitByRaycast.gameObject);
                }
            }
            else
            {
                // Jika objek bukan weapon, nonaktifkan outline dari weapon yang dihover sebelumnya
                if (hoveredWeapon != null)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                    hoveredWeapon = null;
                }
            }
        }
        else
        {
            // Jika tidak ada hit, pastikan outline weapon terakhir dimatikan
            if (hoveredWeapon != null)
            {
                hoveredWeapon.GetComponent<Outline>().enabled = false;
                hoveredWeapon = null;
            }
        }
    }
}
