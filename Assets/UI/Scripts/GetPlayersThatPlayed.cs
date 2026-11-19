using System.Linq;
using UnityEngine;

public class GetPlayersThatPlayed : MonoBehaviour
{
    public GameObject Card;
    public SO_PersistenceData Data;
    public GameObject ParentContent;

    void UpdateInfo()
    {
        var LeaderboardList = Data.GetSortedLeaderboard();

        foreach (var D in LeaderboardList)
        {
            if (D.Name != "")
            {
                GameObject Card_Local = Instantiate(Card);
                Card_Local.transform.SetParent(ParentContent.transform, false);
                SetCardInfo Cardinfo = Card_Local.GetComponent<SetCardInfo>();
                if (Cardinfo)
                {
                    Cardinfo.SetInfo(D.Name, D.Score);
                }
            }

        }
    }

    private void OnEnable()
    {
        UpdateInfo();
    }

}
