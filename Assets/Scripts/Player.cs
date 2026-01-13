using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 0.01f;
    [Header("オブジェクトの位置の微調整"), SerializeField] Vector3 pos_config;
    [SerializeField] float moveLimit_up = 0, moveLimit_bottom = 0, moveLimit_left = 0, moveLimit_right = 0;
    [SerializeField] GameObject fukidasi;

    [Header("無敵設定")]
    [SerializeField] float invincibleTime = 2f;   // 無敵時間（秒）
    [SerializeField] float blinkInterval = 0.2f;  // 点滅間隔

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            transform.position = FacePointCollect.instance.GetFaceCenter();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!isInvincible)
            {
                GameManager.instance.hart--;
                Debug.Log("体力：" + GameManager.instance.hart);

                // 無敵時間＋点滅開始
                StartCoroutine(BecomeInvincible());
            }
        }
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // 点滅
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true; // 元に戻す
        isInvincible = false;
    }

    // 吹き出しは従来のまま
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            GameManager.instance.TextChange("いた～");
            ViewFukidashi();
        }
    }

    void ViewFukidashi()
    {
        fukidasi.SetActive(true);
        StartCoroutine(HideAfter3Seconds(fukidasi));
    }

    IEnumerator HideAfter3Seconds(GameObject obj)
    {
        yield return new WaitForSeconds(3f);
        obj.SetActive(false);
    }
}
