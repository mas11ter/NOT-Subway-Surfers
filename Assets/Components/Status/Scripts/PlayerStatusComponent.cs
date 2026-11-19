using UnityEngine;

public class PlayerStatusComponent : StatusComponent
{
    public PlayerSaveGame SaveGame;

    protected override void InitComponent()
    {
        if (!SaveGame.HasPersistenceData() || SaveGame.GetStats().Count != MyStatusInfos.Length)
        {
            base.InitComponent();
        }
        else
        {
            GenerateOvertimeList.Clear();
            StatInfosList.Clear();
            for (int i = 0; i < SaveGame.GetStats().Count; i++)
            {
                var Info = SaveGame.GetStats()[i];

                if (!Info)
                {
                    StatInfosList.Clear();
                    GenerateOvertimeList.Clear();
                    base.InitComponent();
                    break;
                }

                //Debug.Log(Info);
                StatInfosList.Add(Info);

                if (Info.GenericInfo.bShouldGenerateOvertime)
                {
                    GenerateOvertimeList.Add(new StatGenerateOvertime(Info, Time.time));
                }

            }
        }
    }
    
}
