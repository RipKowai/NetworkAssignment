using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour

{
    [SerializeField] private float speed = 3f;
    private Camera m_Camera;
    private Vector3 _mouseInput = Vector3.zero;
    public GameObject _arrowPrefab;

    private Shooting shooting;
    
    private void Initialize ()
    {
        m_Camera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        shooting = GetComponent<Shooting>();
     
        Initialize();
    }
    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;
        //Movement
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = m_Camera.nearClipPlane;
        Vector2 mousePositionScreen = Input.mousePosition;
        Vector3 mouseWorldCoordinates = m_Camera.ScreenToWorldPoint(mousePositionScreen);
        mouseWorldCoordinates.z = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = Vector3.MoveTowards(transform.position,
                mouseWorldCoordinates, Time.deltaTime * speed);
        }

        //Rotate
        if(mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - transform.position;
            targetDirection.z = 0f;
            transform.up = targetDirection;
        }

        if(Input.GetMouseButtonDown(0))
        {
            shooting.ShootServerRpc();
        }
    }
}
