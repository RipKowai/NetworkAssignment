using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour

{
    [SerializeField] private float speed = 3f;
    private Camera m_Camera;
    private Vector3 _mouseInput = Vector3.zero;
    private Shooting shooting;

    public Transform spawn_point;
    public GameObject _arrowPrefab;
    public GameObject a_SpawnPrefab;
    
    private void Initialize ()
    {
        m_Camera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
       // base.OnNetworkSpawn();
        shooting = GetComponent<Shooting>();

        Initialize();
        if (!IsOwner) return;
        CreateAsteroidSpawnerServerRpc();
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

    [ServerRpc]
    private void CreateAsteroidSpawnerServerRpc()
    {
        if (!IsOwner) return;
        GameObject spawn = Instantiate(a_SpawnPrefab, Vector3.zero, Quaternion.identity);

        spawn.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Spawned Asteroid");
    }
}
