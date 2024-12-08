using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
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
    private Animator animator;
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

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

        bulletsLeft = magazineSize; // Fix typo
    }

    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            // Holding Left Mouse
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || 
                 currentShootingMode == ShootingMode.Burst) 
        {
            // Click Once
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
        {
            Reload();
        }

        // Automatic Reload
        if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
        {
            //Reload();
        }

        if (readyToShoot && isShooting && bulletsLeft > 0)
        {
            burstBulletsLeft = bulletPerBurst;
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletPerBurst}/{magazineSize/bulletPerBurst}";
        }
    }

private void FireWeapon()
{
    bulletsLeft--;

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

    // Mainkan suara reload sesuai tipe senjata
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
