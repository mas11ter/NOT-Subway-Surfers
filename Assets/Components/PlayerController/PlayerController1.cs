using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class PlayerComponent : MonoBehaviour
{
    [SerializeField] private InputContext InputContext;
    [SerializeField] private GameObject DeathCanvas;
    [SerializeField] private PlayerSaveGame SaveGame;
    public SO_PersistenceData Data;
    public Gamemode Gamemode;
    public AudioClip[] DamageSFX;

    // private AbilityManager AbilityManager;
    private MovementComponent MovementComponent;
    private InteracteeComponent interacteeComponent;
    private StatusComponent StatusComponent;


    private Vector2 MovementDirection = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputContext.OnMoveEvent += OnMoved;

        //AbilityManager = GetComponent<AbilityManager>();
        MovementComponent = GetComponent<MovementComponent>();
        interacteeComponent = GetComponent<InteracteeComponent>();
        StatusComponent = GetComponent<StatusComponent>();

        if (StatusComponent)
        {
            StatusComponent.OnStatChanged.AddListener(OnStatChanged);
        }

#if !UNITY_EDITOR
        
        Cursor.visible = false;

#endif
    }




    private void OnDisable()
    {
        DisableInput();
    }

    private void FixedUpdate()
    {
        MovementComponent.Move(new Vector3(MovementDirection.x, MovementDirection.y, 0));
    }

    private void OnMoved(Vector2 InMovement)
    {
        MovementDirection = InMovement;
    }

    private void DisableInput()
    {
        InputContext.OnMoveEvent -= OnMoved;

        if (StatusComponent)
        {
            StatusComponent.OnStatChanged.RemoveListener(OnStatChanged);
        }
    }

    private void OnStatChanged(StatInfo Info, float OldValue, float NewValue)
    {
        if (Info.Status == EStatusAttribute.Coin)
        {
            Data.CurrentScore = (int)NewValue;
            MovementComponent.MovementSpeedY += 0.15f;
            MovementComponent.MovementSpeedY += 0.1f;
        }

        if (Info.Status == EStatusAttribute.Health)
        {
            //Debug.Log("Died");
            if (!Info.ReachedMinimum() && DamageSFX.Length != 0)
            {
                AudioSource.PlayClipAtPoint(DamageSFX[Random.Range(0, DamageSFX.Length - 1)], transform.position);
            }
            else if (Info.ReachedMinimum())
            {
                Data.AddPlayer(Data.CurrentPlayer, Data.CurrentScore);
                DisableInput();
                DeathCanvas.SetActive(true);
                Gamemode.Pause(true);
            }

        }
    }

    //public void ResetPC()
    //{
    //    if (StatusComponent)
    //    {
    //        StatusComponent.OnStatChanged.AddListener(OnStatChanged);
    //    }
    //    InputContext.OnMoveEvent += OnMoved;
    //    DeathCanvas.SetActive(false);
    //    Gamemode.TogglePause();
    //}
}
