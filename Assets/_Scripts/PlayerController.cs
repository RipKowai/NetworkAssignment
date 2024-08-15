using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;
    private Camera m_Camera;
    private Vector3 _mouseInput = Vector3.zero;
    private Shooting shooting;
    private int Heart = 4;
    private int Laugh = 5;
    private int MiddleFinger = 6;

    //public Transform spawn_point;
    public float deSpawnTime = 3;
    public GameObject _arrowPrefab;
    public GameObject a_SpawnPrefab;
    private bool emoji_spawned = false;
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

        if (IsOwner && Input.GetKey(KeyCode.E))
        {
            UseEmoji();
            InvokeRepeating("DeSpawnEmoji", 0f, deSpawnTime);
        }
    }
    public void UseEmoji()
    {
        // Call the ServerRpc to activate the emoji
        EmojiServerRpc();

    }

    [ServerRpc]
    private void EmojiServerRpc()
    {
        //if (!IsOwner)
        //{
        //    Debug.LogWarning("Attempted to activate emoji but this client is not the owner!");
        //    return;
        //}
        
        if (!emoji_spawned)
        {
            GameObject emoji = gameObject.transform.GetChild(Laugh).gameObject;
            emoji.SetActive(true);
            emoji_spawned = true;

            // Inform all clients to activate the emoji
            EmojiClientRpc(Laugh); // Pass the index of the emoji for clarity

            // Start the deactivation process
            Invoke(nameof(DeactivateEmoji), deSpawnTime);
        }
    }

    [ClientRpc]  
    private void EmojiClientRpc(int emojiIndex)
    {
        // This will be called on all clients
        GameObject emoji = transform.GetChild(emojiIndex).gameObject;
        emoji.SetActive(true);

    }
    private void DeactivateEmoji()
    {
        // Deactivate the emoji on the server
        GameObject emoji = gameObject.transform.GetChild(Laugh).gameObject;
        emoji.SetActive(false);
        emoji_spawned = false;

        EmojiDeactivationClientRpc(Laugh);
    }

    [ClientRpc]
    private void EmojiDeactivationClientRpc(int emojiIndex)
    {
        // This will be called on all clients
        GameObject emoji = transform.GetChild(emojiIndex).gameObject;
        emoji.SetActive(false);
    }

    [ServerRpc]
    private void CreateAsteroidSpawnerServerRpc()
    {
        if (!IsOwner) return;
        GameObject spawn = Instantiate(a_SpawnPrefab, Vector3.zero, Quaternion.identity);

        spawn.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Spawned Asteroid");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Arrow" || collision.gameObject.tag == "Asteroids")
        {
            Debug.Log("Ouch I got Hit");
        }
    }
}
