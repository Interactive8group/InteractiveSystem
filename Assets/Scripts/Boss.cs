using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private int hp = 20;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveRangeX = 4f;
    [SerializeField] private float moveRangeY = 2f;

    [Header("Shot Settings")]
    [SerializeField] private GameObject bossBulletPrefab;
    [SerializeField] private float shotInterval = 1.2f;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Flash Settings")]
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private int flashCount = 6;

    private Renderer _renderer;
    private Color _defaultColor;
    private bool isFlashing = false;

    private Vector3 startPos;
    private float shotTimer;

    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _defaultColor = _renderer.material.color;
        startPos = transform.position;
    }

    void Update()
    {
        Move();
        Shoot();
    }

    // --------------------
    // 移動処理
    // --------------------
    void Move()
    {
        float x = Mathf.Sin(Time.time * moveSpeed) * moveRangeX;
        float y = Mathf.Cos(Time.time * moveSpeed * 0.7f) * moveRangeY;

        transform.position = new Vector3(
            startPos.x + x,
            startPos.y + y,
            startPos.z
        );
    }

    // --------------------
    // 弾発射
    // --------------------
    void Shoot()
    {
        shotTimer += Time.deltaTime;

        if (shotTimer >= shotInterval)
        {
            shotTimer = 0f;

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            Vector3 dir = (player.transform.position - transform.position).normalized;

            GameObject bullet = Instantiate(
                bossBulletPrefab,
                transform.position,
                Quaternion.LookRotation(dir)
            );

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
            }
        }
    }

    // --------------------
    // ダメージ
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
    }

    void TakeDamage(int damage)
    {
        hp -= damage;

        if (!isFlashing)
        {
            StartCoroutine(Flash());
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    IEnumerator Flash()
    {
        isFlashing = true;

        for (int i = 0; i < flashCount; i++)
        {
            _renderer.material.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
            _renderer.material.color = _defaultColor;
            yield return new WaitForSeconds(flashInterval);
        }

        isFlashing = false;
    }

    void Die()
    {
        GameManager.instance.GameClear();
        Destroy(gameObject);
    }
}
