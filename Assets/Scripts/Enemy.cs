using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 moveDirection;

    // 弾の向きをセット
    public void SetDirection(Vector3 targetPos)
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    void Update()
    {
        // 移動
        transform.position += moveDirection * speed * Time.deltaTime;

        // 画面外チェック
        if (IsOutOfScreen())
        {
            Destroy(gameObject);
        }
    }

    // 弾が画面外か判定
    bool IsOutOfScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < -0.1f || screenPoint.x > 1.1f || screenPoint.y < -0.1f || screenPoint.y > 1.1f;
    }
}
