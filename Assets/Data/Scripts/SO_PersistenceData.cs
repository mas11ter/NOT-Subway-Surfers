using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    public string Name;
    public int Score;
}

[System.Serializable]
public class Leaderboard
{
    public List<PlayerScore> Players = new();
}

[CreateAssetMenu(fileName = "SO_PersistenceData", menuName = "Scriptable Objects/SO_PersistenceData")]
public class SO_PersistenceData : ScriptableObject
{
    
    private List<StatInfo> StatInfos = new();

    private bool bHasPersistenceData = false;

    public Dictionary<string, int> PlayerToScore = new();

    public string CurrentPlayer;
    public int CurrentScore;
    public string LastPlayer = "";

    string LeaderboardPath;

    private void OnEnable()
    {
        LeaderboardPath = Application.persistentDataPath + "/Leaderboard.json";
    }

    public void SaveGame(Leaderboard leaderboard)
    {
        string json = JsonUtility.ToJson(leaderboard);
        File.WriteAllText(LeaderboardPath,json);
    }

    public void AddPlayer(string InName, int InScore)
    {
        Leaderboard LB = GetLeaderboard();
        bool bFound = false;
        for (int i = LB.Players.Count - 1; i >= 0; i--)
        {
            if (LB.Players[i].Name == InName)
            {
                bFound = true;
                if (LB.Players[i].Score < InScore)
                {
                    LB.Players[i].Score = InScore;
                }
                break;
            }
        }
        if (!bFound)
        {
            PlayerScore PS = new();
            PS.Name = InName;
            PS.Score = InScore;
            LB.Players.Add(PS);
        }
        SaveGame(LB);
    }

    public int GetRank(string InName)
    {
        Leaderboard LB = GetLeaderboard();

        var LeaderboardList = LB.Players
            .OrderByDescending(x => x.Score)
            .ToList();

        int Index = LeaderboardList.FindIndex(x => x.Name == InName);

        return Index == -1 ? 0 : Index + 1;

    }

    public int GetHighestScore(string InName)
    {
        Leaderboard LB = GetLeaderboard();
        var Player = LB.Players.FirstOrDefault(x => x.Name == InName);
        return Player?.Score ?? 0;

    }

    public List<PlayerScore> GetSortedLeaderboard()
    {
        return GetLeaderboard().Players.OrderByDescending(x => x.Score).ToList();
    }

    public Leaderboard GetLeaderboard()
    {
        if (File.Exists(LeaderboardPath))
        {
            string json = File.ReadAllText(LeaderboardPath);

            if (!string.IsNullOrEmpty(json))
            {
                return JsonUtility.FromJson<Leaderboard>(json)
                       ?? new Leaderboard();
            }
        }

        return new Leaderboard();
    }

    public bool HasPersistenceData() {  return bHasPersistenceData; }

    public void SetStats(List<StatInfo> InStatInfos)
    {
        StatInfos = InStatInfos;
    }

    public List<StatInfo> GetSavedStats()
    {
        return StatInfos;
    }


    public void OnApplicationQuit()
    {
        bHasPersistenceData = false;
    }
}
