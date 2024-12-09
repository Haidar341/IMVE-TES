using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    public int bulletPerBurst = 3;
    public int burstBulletsLeft;
    public float spreadIntensity;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;
    public GameObject muzzleEffect;
    internal Animator animator;
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public int durability = 100; // Ketahanan senjata

    public enum WeaponModel
    {
        M1911,
        M4
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
    }

    void Update()
    {
        if (!isActiveWeapon)
        {
            return;
        }

        DurabilityManager.Instance.UpdateDurabilityDisplay(durability);

        // Menentukan apakah pemain sedang menembak
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // Reload jika tombol R ditekan
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
        {
            Reload();
        }

        // Menembak jika siap menembak dan pemain menekan tombol menembak
        if (readyToShoot && isShooting && bulletsLeft > 0)
        {
            burstBulletsLeft = bulletPerBurst;
            FireWeapon();
        }

    }

    private void FireWeapon()
    {
        bulletsLeft--;

        // Kurangi ketahanan senjata
        durability -= 10; // Misalnya, kurangi 10 setiap kali menembak

        // Hancurkan senjata jika durabilitas habis
        if (durability <= 0)
        {
            DestroyWeapon();
            return;
        }

        // Kunci posisi senjata sebelum menembak
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;

        if (muzzleEffect != null && muzzleEffect.TryGetComponent<ParticleSystem>(out var particleSystem))
        {
            particleSystem.Play();
            if (animator != null)
            {
                animator.SetTrigger("RECOIL");
            }
            SoundManager.Instance.PlayShootingSound(thisWeaponModel);
        }

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // Kembalikan posisi senjata setelah menembak
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        animator.SetTrigger("RELOAD");
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

   private void DestroyWeapon()
    {
    // Mainkan suara senjata hancur
    SoundManager.Instance.PlayWeaponDestroySound();

    // Hapus senjata dari slot aktif di WeaponManager
    if (isActiveWeapon && WeaponManager.Instance != null)
    {
        WeaponManager.Instance.DropCurrentWeapon();
    }

    // Nonaktifkan UI durability
    DurabilityManager.Instance.SetDurabilityUIActive(false);

    // Hancurkan game object senjata
    Destroy(gameObject);
    }


    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
