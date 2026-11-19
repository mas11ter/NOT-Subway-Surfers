using UnityEngine;
using UnityEngine.UI;

public class SliderStat : MonoBehaviour
{
    public EStatusAttribute MyStatus;
    public Slider MySlider;

    private StatusComponent StatusComp;

    private void Start()
    {
        if (!MySlider)
        {
            MySlider = GetComponent<Slider>();
        }

        StatusComp = transform.root.GetComponent<StatusComponent>();
        if (StatusComp)
        {
            StatusComp.OnStatChanged.AddListener(OnStatChanged);
            StatInfo S = StatusComp.GetStatInfo(MyStatus);
            if (S)
            {
                UpdateSlider(S);
            }
        }
    }

    private void OnDisable()
    {
        if (StatusComp)
        {
            StatusComp.OnStatChanged.RemoveListener(OnStatChanged);
        }
    }

    private void UpdateSlider(StatInfo Info)
    {
        MySlider.value = Info.GenericInfo.CurrentValue / Info.GenericInfo.MaxValue;
    }

    private void OnStatChanged(StatInfo Info, float OldValue, float NewValue)
    {
        if (Info.Status == MyStatus && MySlider)
        {
            UpdateSlider(Info);
        }
    }
}
