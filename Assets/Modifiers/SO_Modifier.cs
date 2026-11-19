using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TemporaryItemModifierInfo
{
    public EStatusAttribute Attribute;
    public float Amount;
    public bool MakeAmountAPercentOfMax;

    public bool IsOvertime;
    public float OvertimeDuration;
    public float OvertimeTick;
    public bool bApplyModifierTickOnInteraction;

    public bool RevertAfterItsDone;

    [HideInInspector] public float TimeStarted;
    [HideInInspector] public float LastTick;
}

[System.Serializable]
public struct PersistentItemModifierInfo
{
    public EStatusAttribute Attribute;
    public float Amount;
    public bool MakeAmountAPercentOfMax;
}

[CreateAssetMenu(fileName = "SO_Modifier", menuName = "Scriptable Objects/SO_Modifier")]
public class SO_Modifier : ScriptableObject
{
    public TemporaryItemModifierInfo[] TemporaryItemModifiers;
    public PersistentItemModifierInfo[] MaxPersistentItemModifierInfos;
    public GameObject VFX;

    public string ItemName;
    public string ItemDescription;

    [HideInInspector] public bool bShouldUpdate;

    List<TemporaryItemModifierInfo> RemainingModifiers = new();
    List<TemporaryItemModifierInfo> RevertModifiers = new();
    [HideInInspector] public StatusComponent OwnerStatusComp;
    List<GameObject> VFX_Refs = new();

    private void OnDisable()
    {
        foreach (var V in VFX_Refs)
        {
            if (V)
            {
                V.SetActive(false);
            }
        }
        VFX_Refs.Clear();
    }

    public void Update()
    {
        if (bShouldUpdate)
        {
            for (int i = RemainingModifiers.Count - 1; i >= 0; i--)
            {
                var Mod = RemainingModifiers[i];
                if (Time.time - Mod.TimeStarted >= Mod.OvertimeDuration)
                {
                    RemainingModifiers.RemoveAt(i);
                    continue;
                }

                if (Mod.OvertimeTick != -1 && Time.time - Mod.LastTick >= Mod.OvertimeTick)
                {
                    Mod.LastTick = Time.time;
                    RemainingModifiers[i] = Mod;
                    ApplyTemporaryModifier(OwnerStatusComp, Mod);
                }
            }

            if (RemainingModifiers.Count == 0)
            {
                bShouldUpdate = false;

                foreach(var A in RevertModifiers)
                {
                    OwnerStatusComp.GiveStatValue(A.Attribute, -A.Amount);
                }



                OwnerStatusComp.RemoveModifier(this, true);
            }
        }
    }

    public void Init()
    {
        foreach (var M in MaxPersistentItemModifierInfos)
        {
            ApplyPersistentModifier(OwnerStatusComp, M);
        }

        for (int i = 0; i < TemporaryItemModifiers.Length; i++)
        {
            var I = TemporaryItemModifiers[i];
            if (I.IsOvertime)
            {
                I.TimeStarted = Time.time;
                I.LastTick = Time.time;
                RemainingModifiers.Add(I);
                bShouldUpdate = true;
                if (!I.bApplyModifierTickOnInteraction) continue;
            }

            if (I.RevertAfterItsDone)
            {
                RevertModifiers.Add(I);
            }

            ApplyTemporaryModifier(OwnerStatusComp, I);
        }


        if (!bShouldUpdate)
        {
            //Exec shouldn't come here, if its not updating it should only be default object and not a clone.
            OwnerStatusComp.RemoveModifier(this, true);
        }
    }

    private void ApplyTemporaryModifier(StatusComponent Comp, TemporaryItemModifierInfo Info)
    {
        if (VFX)
        {
            var V = PoolSystem.GetInstance().SpawnFromPool(VFX, Comp.gameObject.transform);
            V.transform.SetPositionAndRotation(Comp.gameObject.transform.position, Quaternion.identity);
            VFX_Refs.Add(V);
        }
        var Stat = Comp.GetStatInfo(Info.Attribute);
        if (!Stat)
        {
            Debug.LogError("Trying to add a modifier to " + Comp.gameObject + " that doesn't have " + Info.Attribute + " stat");
            return;
        }

        float AmountToGive = Info.MakeAmountAPercentOfMax ? Info.Amount * Stat.GenericInfo.MaxValue : Info.Amount;
        Comp.GiveStatValue(Info.Attribute, AmountToGive);
    }

    private void ApplyPersistentModifier(StatusComponent Comp, PersistentItemModifierInfo Info)
    {
        var I = Comp.GetStatInfo(Info.Attribute);
        if (I)
        {
            float MaxAddition = Info.MakeAmountAPercentOfMax ? Info.Amount * Comp.GetStatInfo(Info.Attribute).GenericInfo.MaxValue : Info.Amount;
            Comp.SetNewMaxValue(Info.Attribute, MaxAddition + Comp.GetStatInfo(Info.Attribute).GenericInfo.MaxValue);
        }
    }

    public bool bShouldClone()
    {
        foreach( var i in TemporaryItemModifiers)
        {
            if (i.IsOvertime)
            {
                return true;
            }
        }
        return false;
    }

    public void ApplyAllModifier(StatusComponent Comp)
    {
        foreach( var i in TemporaryItemModifiers)
        {
            ApplyTemporaryModifier(Comp, i);
        }
        foreach (var M in MaxPersistentItemModifierInfos)
        {
            ApplyPersistentModifier(Comp, M);
        }
    }
}
