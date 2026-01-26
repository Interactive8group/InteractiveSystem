using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;
    public float destroyZ = -6f;

    private Vector3 direction;
    private bool isDead = false; // ★ 二重死亡防止

    public void SetDirection(Vector3 targetPos)
    {
        direction = (targetPos - transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // ★ 画面奥へ抜けたら消える
        if (transform.position.z <= destroyZ)
        {
            Die();
        }
    }

    // ★ プレイヤー or 弾 に当たった
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(1);
            }
        }
        else if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject); // 弾を消す
            Die();
        }
    }

    // ★ 消える処理はここに集約
    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.OnEnemyDead();
        }
        SoundManager.Instance.PlaySE(1);
        Destroy(gameObject);
    }
}
