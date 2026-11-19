using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveGame : MonoBehaviour
{

    public SO_PersistenceData PD;

    public void SaveData()
    {
        var SC = GetComponent<StatusComponent>();
        SetStats(SC.GetAllStats());

    }

    public bool HasPersistenceData()
    {
        return PD.HasPersistenceData();
    }

    public List<StatInfo> GetStats()
    {
        return PD.GetSavedStats();
    }

    public void SetStats(List<StatInfo> InStats)
    {
        PD.SetStats(InStats);
    }


    //public void SetReachedNewSavePoint(SavePoint Point)
    //{
    //    if (Point && (!LastSavePoint || Point.Priority > LastSavePoint.Priority))
    //    {
    //        LastSavePoint = Point;
    //    }
    //}

    private void OnApplicationQuit()
    {
        PD.OnApplicationQuit();
    }
}
