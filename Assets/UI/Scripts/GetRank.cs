using TMPro;
using UnityEngine;

public class GetRank : MonoBehaviour
{
    public SO_PersistenceData Data;

    public TMP_Text Rank;
    public TMP_Text HighestScore;

    private void OnEnable()
    {
        HighestScore.text = $"Highest Score: {Data.GetHighestScore(Data.CurrentPlayer):0} as {Data.CurrentPlayer}";
        Rank.text = $"Rank: {Data.GetRank(Data.CurrentPlayer):0}";

    }


}
