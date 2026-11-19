using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public struct StarterStatInfo
{
    [SerializeField] private StatInfo Stat;
    public float MaxValue;
    public float MinValue;
    public float CurrentValue;

    public bool bShouldGenerateOvertime;
    public float GenerationTick;
    public bool MakeGenerateAmountAPercentOfMax;
    public float GenerationAmount;

    //private Descriptor<string, EGeneratedStats> StringToStatMap;

    
    public readonly StatInfo GetInfo()
    {
        return Stat;
    }

    public void SetInfo(StatInfo info)
    {
        Stat = info;
    }

}

[CreateAssetMenu(fileName = "StatInfo", menuName = "Status/StatInfo")]
public class StatInfo : ScriptableObject
{
    public EStatusAttribute Status;

    public StarterStatInfo GenericInfo;
    //maybe someone wants to leave the stat 0 at the start.
    public bool bStopAutoFillingCurrentValue = false;


    public bool ReachedMinimum()
    {
        return GenericInfo.CurrentValue <= GenericInfo.MinValue;
    }

    private void OnValidate()
    {
        if (GenericInfo.GetInfo() == null)
        {
            GenericInfo.SetInfo(this);
        }
        if (!bStopAutoFillingCurrentValue && GenericInfo.CurrentValue == 0)
        {
            GenericInfo.CurrentValue = GenericInfo.MaxValue;
        }
    }
}



