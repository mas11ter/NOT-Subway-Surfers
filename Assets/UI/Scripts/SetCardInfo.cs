using TMPro;
using UnityEngine;

public class SetCardInfo : MonoBehaviour
{

    public TMP_Text Name;
    public TMP_Text Score;

    public void SetInfo(string InName, int InScore)
    {
        Name.text = InName;
        Score.text = $"{InScore:0}";

    }
}
