using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DamageInfo
{
    public float Damage;
    public StatusComponent DamageCauser;
    [HideInInspector] public Vector3 HitLocation;
    public bool IgnoreDamageStat;
    public SO_Modifier Modifier;

    public DamageInfo(float Damage, GameObject DamageCauser, Vector3 HitLocation)
    {
        this.Damage = Damage;
        this.DamageCauser = DamageCauser.GetComponent<StatusComponent>();
        this.HitLocation = HitLocation;
        this.IgnoreDamageStat = false;
        this.Modifier = null;
    }

    public DamageInfo(float Damage, StatusComponent DamageCauser, Vector3 HitLocation)
    {
        this.Damage = Damage;
        this.DamageCauser = DamageCauser;
        this.HitLocation = HitLocation;
        this.IgnoreDamageStat = false;
        this.Modifier = null;
    }
}

public struct StatGenerateOvertime
{
    public StatInfo Stat;
    public float LastTimeGenerated;

    public StatGenerateOvertime(StatInfo InStat, float InTime)
    {
        Stat = InStat;
        LastTimeGenerated = InTime;
    }
}

public class StatusComponent : MonoBehaviour
{
    public StarterStatInfo[] MyStatusInfos;
    [HideInInspector] public UnityEvent<StatInfo, float /*Old Value*/, float /*New Value*/> OnStatChanged;
    public UnityEvent<DamageInfo> OnDamageTaken;
    public UnityEvent<StatusComponent, DamageInfo> OnDamageDealt;

    protected List<StatInfo> StatInfosList = new List<StatInfo>();
    protected List<StatGenerateOvertime> GenerateOvertimeList = new List<StatGenerateOvertime>();

    private List<SO_Modifier> ActiveModifiers = new();

    //#if With_UnityEditor
    private List<StatInfo> EditorCachedInfos = new();

    private void OnValidate()
    {
        for (int i = 0; i < MyStatusInfos.Length; i++)
        {
            StarterStatInfo Info = MyStatusInfos[i];
            if (!Info.GetInfo()) continue;

            if (i >= EditorCachedInfos.Count)
            {
                EditorCachedInfos.Insert(i, Info.GetInfo());
            }
            else if (EditorCachedInfos[i] != Info.GetInfo())
            {
                MyStatusInfos[i] = Info.GetInfo().GenericInfo;
                EditorCachedInfos[i] = MyStatusInfos[i].GetInfo();
            }
        }
    }
    //#endif

    private void Awake()
    {
        InitComponent();
    }

    protected virtual void InitComponent()
    {
        GenerateOvertimeList.Clear();
        StatInfosList.Clear();
        foreach (var Info in MyStatusInfos)
        {
            if (!Info.GetInfo()) continue;

            StatInfo NewInfo = Instantiate(Info.GetInfo());

            if (NewInfo)
            {
                NewInfo.GenericInfo = Info;
                StatInfosList.Add(NewInfo);
                if (NewInfo.GenericInfo.bShouldGenerateOvertime)
                {
                    GenerateOvertimeList.Add(new StatGenerateOvertime(NewInfo, Time.time));
                }
            }
        }
    }

    private void Update()
    {
        if (GenerateOvertimeList.Count > 0)
        {
            for (int i = 0; i < GenerateOvertimeList.Count; i++)
            {
                StatGenerateOvertime Local = GenerateOvertimeList[i];
                if (Local.Stat && (Time.time - Local.LastTimeGenerated > Local.Stat.GenericInfo.GenerationTick))
                {
                    Local.LastTimeGenerated = Time.time;
                    float AmountToGive = Local.Stat.GenericInfo.MakeGenerateAmountAPercentOfMax ? Local.Stat.GenericInfo.GenerationAmount * Local.Stat.GenericInfo.MaxValue : Local.Stat.GenericInfo.GenerationAmount;
                    TakeStatValue(Local.Stat.Status, -AmountToGive);

                    GenerateOvertimeList[i] = Local;
                }

            }
        }

        //for (int i = 0; i < ActiveModifiers.Count; i++)
        //{
        //    var Mod = ActiveModifiers[i];
        //    if (Mod && Mod.bShouldUpdate)
        //    {
        //        Mod.Update();
        //    }
        //}

    }

    public StatInfo GetStatInfo(EStatusAttribute Status)
    {
        for (int i = 0; i < StatInfosList.Count;i++)
        {
            var Info = StatInfosList[i];
            if (Info && Info.Status == Status)
            {
                return StatInfosList[i];
            }
        }
       // Debug.Assert(false, "trying to get a stat called " + Status + ", but it doesn't exist in " + gameObject);
        return null;
    }

    //returns true when stat value is adjusted
    public bool SetValue(ref StatInfo Stat, float Value)
    {
        if (Stat)
        {
            float OldValue = Stat.GenericInfo.CurrentValue;
            Stat.GenericInfo.CurrentValue = Value;
            Stat.GenericInfo.CurrentValue = Mathf.Clamp(Stat.GenericInfo.CurrentValue, Stat.GenericInfo.MinValue, Stat.GenericInfo.MaxValue);
            //Debug.Log("New " + Stat.GenericInfo.StatName + " value is " + Stat.GenericInfo.CurrentValue);

            OnStatChanged.Invoke(Stat, OldValue, Stat.GenericInfo.CurrentValue);

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TakeDamage(DamageInfo Damage)
    {


        if (Damage.Damage != 0)
        {

            bool bHasbeenDamaged = TakeStatValue(EStatusAttribute.Health, Damage.Damage);

            if (bHasbeenDamaged)
            {
                OnDamageTaken.Invoke(Damage);
                if (Damage.DamageCauser)
                {
                    Damage.DamageCauser.OnDamageDealt.Invoke(this, Damage);
                }
            }
               

            return bHasbeenDamaged;
        }
        return false;
    }

    //return true if the stat has enough to remove this amount.
    public bool HasEnoughStats(EStatusAttribute Status, float Amount)
    {
        StatInfo StatInfo = GetStatInfo(Status);
        if (StatInfo)
        {
            return (StatInfo.GenericInfo.CurrentValue - Amount) >= StatInfo.GenericInfo.MinValue;
        }
        return false;
    }

    public bool HasStat(EStatusAttribute Status)
    {
        foreach (var I in StatInfosList)
        {
            if (I && I.Status == Status)
            {
                return true;
            }
        }
        return false;
    }

    public bool TakeStatValue(EStatusAttribute Status, float Amount)
    {
        var I = GetStatInfo(Status);
        if (I)
        {
            return SetValue(ref I, I.GenericInfo.CurrentValue - Amount);
        }
        return false;
    }
    public bool GiveStatValue(EStatusAttribute Status, float Amount)
    {
        return TakeStatValue(Status, -Amount);
    }

    public bool SetNewMaxValue(EStatusAttribute Status, float NewMax)
    {
        var I = GetStatInfo(Status);
        if (I)
        {
            float Before = I.GenericInfo.MaxValue;
            I.GenericInfo.MaxValue = NewMax;
            OnStatChanged.Invoke(I, Before, NewMax);
        }
        return false;
    }    

    public void ResetStats()
    {
        for (int i = 0; i < StatInfosList.Count; i++)
        {
            var Info = StatInfosList[i];
            if (Info)
            {
                StatInfosList[i].GenericInfo.CurrentValue = Info.GenericInfo.MaxValue;
                OnStatChanged.Invoke(Info, Info.GenericInfo.MaxValue, Info.GenericInfo.MaxValue);
            }
        }
    }

    public void ApplyModifier(SO_Modifier Mod)
    {
        if (!Mod.bShouldClone())
        {
            Mod.ApplyAllModifier(this);
            return;
        }


        var M = Instantiate(Mod);
        if (M)
        {
            M.OwnerStatusComp = this;
            M.Init();
            ActiveModifiers.Add(M);
        }
    }


    public void RemoveModifier(SO_Modifier Mod, bool bShouldDestroy)
    {
        if (ActiveModifiers.Contains(Mod))
        {
            ActiveModifiers.Remove(Mod);
        }
        if (bShouldDestroy)
        {
            Destroy(Mod);
        }
    }

    public List<StatInfo> GetAllStats() { return StatInfosList; }

}


