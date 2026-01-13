using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("無敵設定")]
    [SerializeField] float invincibleTime = 2f;
    [SerializeField] float blinkInterval = 0.2f;

    private bool isInvincible = false;
    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (GameManager.instance.hart > 0)
        {
            PlayerMove();
        }
    }

    void PlayerMove()
    {
        if (FacePointCollect.instance != null && FacePointCollect.instance.collectFinish)
        {
            Vector3 pos = FacePointCollect.instance.GetFaceCenter();
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInvincible) return;

        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
        {
            GameManager.instance.hart--;
            StartCoroutine(BecomeInvincible());

            // 弾だけ消す
            if (other.CompareTag("EnemyBullet"))
            {
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            playerRenderer.enabled = !playerRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        playerRenderer.enabled = true;
        isInvincible = false;
    }
}
