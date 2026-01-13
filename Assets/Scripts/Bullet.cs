using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;           // 弾の速度
    private Vector3 moveDirection;     // 弾の移動方向

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        // 画面外に出たら消す
        if (IsOutOfScreen())
        {
            Destroy(gameObject);
        }
    }

    bool IsOutOfScreen()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        return vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f;
    }
}
