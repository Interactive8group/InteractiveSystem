using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("無敵設定")]
    [SerializeField] float invincibleTime = 2f;
    [SerializeField] float blinkInterval = 0.2f;

    [Header("移動制御")]
    [SerializeField] float followSpeed = 8f;
    [SerializeField] Vector2 minMove = new Vector2(-8f, -4.5f);
    [SerializeField] Vector2 maxMove = new Vector2(8f, 4.5f);

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
        if (FacePointCollect.instance == null) return;
        if (!FacePointCollect.instance.collectFinish) return;

        // ★ 0〜1 の顔座標
        Vector2 face01 = FacePointCollect.instance.GetFaceCenter01();

        float x = Mathf.Lerp(minMove.x, maxMove.x, face01.x);
        float y = Mathf.Lerp(minMove.y, maxMove.y, face01.y);

        Vector3 targetPos = new Vector3(x, y, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInvincible) return;

        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
        {
            GameManager.instance.hart--;
            StartCoroutine(BecomeInvincible());

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
