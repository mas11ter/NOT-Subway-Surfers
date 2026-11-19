using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct DamagedEnemiesToTime
{
    StatusComponent StatusComponent;
    float LastTick;
}

public class DamageOnCollision : MonoBehaviour
{
    public DamageInfo DamageInfo;
    public bool bShouldTickDamage = false;
    public float DamageTick = 0.75f;
    public GameObject HitEffect;

    private float LastTick;

    private Dictionary<StatusComponent, float> ObjectToLastDamaged = new();

    bool bOnCooldown = false;

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (!other.gameObject || bOnCooldown) return;
        var StatusComponent = other.gameObject.GetComponent<StatusComponent>();
        if (StatusComponent)
        {
            StatusComponent.TakeDamage(DamageInfo);
            DamageCollided(StatusComponent, StatusComponent.transform.position);
            ObjectToLastDamaged.Add(StatusComponent, Time.time);
            bOnCooldown = true;
            StartCoroutine(Fire());
        }
    }

    

IEnumerator Fire()
{
    yield return new WaitForSeconds(0.5f);
    bOnCooldown = false;
}

private void Update()
    {
        if (bShouldTickDamage && ObjectToLastDamaged.Count != 0)
        {
            for (int i = ObjectToLastDamaged.Count - 1; i >= 0; i--)
            {
                KeyValuePair<StatusComponent, float> I = ObjectToLastDamaged.ElementAt(i);
                if (Time.time - I.Value >= DamageTick)
                {
                    ObjectToLastDamaged[I.Key] = Time.time;

                    if (I.Key)
                    {
                        I.Key.TakeDamage(DamageInfo);
                        DamageCollided(I.Key, I.Key.transform.position);
                    }
                    else
                    {
                        ObjectToLastDamaged.Remove(I.Key);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        var SC = other.gameObject.GetComponent<StatusComponent>();
        if (SC && ObjectToLastDamaged.ContainsKey(SC))
        {
            ObjectToLastDamaged.Remove(SC);
        }
    }

    private void DamageCollided(StatusComponent StatusComp, Vector3 Location)
    {
        StatusComp.TakeDamage(DamageInfo);

        if (HitEffect)
        {
            PoolSystem.GetInstance().SpawnFromPool(HitEffect, Location, Quaternion.identity);
        }
        
    }
}
