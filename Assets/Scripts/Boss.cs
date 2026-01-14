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

    [Header("Death Settings")]
    [SerializeField] private GameObject deathEffectPrefab; // 倒れたときのエフェクト
    [SerializeField] private float deathFallDuration = 1f; // 倒れる時間

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
    // ダメージ判定
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
            SoundManager.Instance.PlaySE(1); // ダメージSE
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

    // --------------------
    // 点滅処理
    // --------------------
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

    // --------------------
    // 死亡処理
    // --------------------
    void Die()
    {
        // 移動や攻撃を止める
        StopAllCoroutines();
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        // 1. ゲームクリア通知
        GameManager.instance.GameClear();

        // 2. 倒れる演出
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * 2f; // 下に2ユニット倒れる
        float elapsed = 0f;

        while (elapsed < deathFallDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / deathFallDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // 3. エフェクト生成
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 4. 倒れる音
        SoundManager.Instance.PlaySE(2); // 倒れるSE

        // 5. ボス削除
        Destroy(gameObject);
    }
}
