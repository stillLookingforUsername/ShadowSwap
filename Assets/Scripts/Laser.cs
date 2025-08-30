using UnityEngine;

public class Laser : MonoBehaviour
{

    [Header("Laser Settings")]
    public float activeTime = 2f;    // How long laser stays ON
    public float inactiveTime = 2f;  // How long laser stays OFF
    public bool startActive = true;  // Should laser start ON?

    private bool isActive;
    private float timer;

    private SpriteRenderer spriteRenderer;
    private Collider2D hitbox;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();
    }

    private void Start()
    {
        isActive = startActive;
        timer = isActive ? activeTime : inactiveTime;
        UpdateLaserState();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = !isActive;
            timer = isActive ? activeTime : inactiveTime;
            UpdateLaserState();
        }
    }

    private void UpdateLaserState()
    {
        spriteRenderer.enabled = isActive;
        hitbox.enabled = isActive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && other.gameObject.TryGetComponent(out PlayerHealth player))
        {
            Debug.Log("Player fried by laser!");
            player.Die();

            // other.GetComponent<PlayerHealth>().TakeDamage(999);
        }
    }
}