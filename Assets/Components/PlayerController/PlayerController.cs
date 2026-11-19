using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputContext InputContext;
    [SerializeField] private GameObject DeathCanvas;
    [SerializeField] private PlayerSaveGame SaveGame;


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



#if !UNITY_EDITOR
    private void Awake()
    {
        transform.position = new Vector3(0f, 5f, 0f);
        
    }

#endif

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
        if (Info.Status == EStatusAttribute.Health && Info.ReachedMinimum())
        {
            //Debug.Log("Died");
            //DisableInput();
            //DeathCanvas.SetActive(true);
            ReturnToSavepoint();
        }
    }

    public void ReturnToSavepoint()
    {
        StatusComponent.ResetStats();
    }
}
