using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 target;

    /// <summary>
    /// 画面内のターゲット位置をセット（Zは弾と同じ）
    /// </summary>
    public void SetTarget(Vector3 targetPos)
    {
        // Zは現在の弾のZ座標を使う
        target = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }

    void Update()
    {
        // ターゲット方向に移動（XYだけ）
        Vector3 dir = (target - transform.position).normalized;
        dir.z = 0f; // 念のためZ方向は動かさない
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("被弾しました");
            Destroy(gameObject);
        }
    }
}
