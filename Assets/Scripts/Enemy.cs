using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;
    public float destroyZ = -6f; // ★ これを超えたら消す

    private Vector3 direction;

    public void SetDirection(Vector3 targetPos)
    {
        direction = (targetPos - transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // ★ プレイヤーを通り過ぎたら消える
        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
