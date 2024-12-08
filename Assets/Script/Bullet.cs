using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectHit) 
    {
        if (objectHit.gameObject.CompareTag("Target"))
        {
            print("hit" + objectHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectHit);

            Destroy(gameObject);
        }

         if (objectHit.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");

            CreateBulletImpactEffect(objectHit);

            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferances.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)          
        );

        hole.transform.SetParent(objectHit.gameObject.transform);

    }
}