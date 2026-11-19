using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkSliderWithStat : MonoBehaviour
{
    public EStatusAttribute MyStatus;
    public TMP_Text TextComponent;
    private StatusComponent StatusComp;

    private void Start()
    {
       // TextComponent = GetComponent<TMP_Text>();
        StatusComp = transform.root.GetComponent<StatusComponent>();
        if (StatusComp)
        {
            StatusComp.OnStatChanged.AddListener(OnStatChanged);
            bListening = true;
            StatInfo S = StatusComp.GetStatInfo(MyStatus);
            if (S)
            {
                UpdateInfo(S);
            }
        }
    }
    bool bListening = false;
    private void OnEnable()
    {
        if (bListening)
        {
            return;
        }
        StatusComp = transform.root.GetComponent<StatusComponent>();
        if (StatusComp)
        {
            StatusComp.OnStatChanged.AddListener(OnStatChanged);
            bListening = true;
            StatInfo S = StatusComp.GetStatInfo(MyStatus);
            if (S)
            {
                UpdateInfo(S);
            }
        }
    }

    private void OnDisable()
    {
        if (StatusComp)
        {
            StatusComp.OnStatChanged.RemoveListener(OnStatChanged);
            bListening = false;
        }
    }

    private void UpdateInfo(StatInfo Info)
    {
        //MySlider.value = Info.GenericInfo.CurrentValue / Info.GenericInfo.MaxValue;
        //TextComponent.text += Info.GenericInfo.CurrentValue;
        TextComponent.text = $"Coins: {Info.GenericInfo.CurrentValue:0}";
    }

    private void OnStatChanged(StatInfo Info, float OldValue, float NewValue)
    {
        if (Info.Status == MyStatus)
        {
            UpdateInfo(Info);
        }
    }
}
