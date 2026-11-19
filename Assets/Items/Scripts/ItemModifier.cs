using UnityEngine;

public class ItemModifier : ItemBase
{
    public SO_Modifier MyModifier;
    Vector3 StartingLocation;

    protected override void Start()
    {
    //    if (MyModifier)
    //    {
    //        SetItemInfo(MyModifier.ItemName, MyModifier.ItemDescription);
    //    }
        base.Start();
        StartingLocation = transform.position;
        FinishLine.GetInstance().OnReachedFinishLine.AddListener(RechedFinishLine);
    }

    private void OnDisable()
    {
        //FinishLine.GetInstance().OnReachedFinishLine.RemoveListener(RechedFinishLine);
    }
    public void RechedFinishLine()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }    
    }

    protected override void OnItemInteracted(GameObject Interacted)
    {
        if (gameObject.activeInHierarchy)
        {
            var StatusComp = Interacted.GetComponent<StatusComponent>();
            if (StatusComp)
            {
                StatusComp.ApplyModifier(MyModifier);
            }
            gameObject.SetActive(false);
        }

        //transform.position = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z);
    }
}
