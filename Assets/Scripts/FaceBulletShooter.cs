using UnityEngine;

public class FaceBulletShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float speedThreshold = 0.5f;  // 閾値
    public float fireCooldown = 0.3f;    // クールタイム

    private Vector3 lastFacePos;
    private float cooldownTimer = 0f;

    void Start()
    {
        lastFacePos = transform.position;
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        Vector3 facePos = transform.position;
        Vector3 delta = facePos - lastFacePos;

        // 縦方向の移動量
        float verticalMovement = delta.y / Time.deltaTime;

        if (Mathf.Abs(verticalMovement) > speedThreshold && cooldownTimer >= fireCooldown)
        {
            // 奥行き方向（カメラの前方向）に飛ばす
            Vector3 shootDirection = Vector3.forward;  // ワールド前方向に固定
            Shoot(shootDirection, Mathf.Abs(verticalMovement));
            cooldownTimer = 0f;
        }

        lastFacePos = facePos;
    }

    void Shoot(Vector3 direction, float velocity)
    {
        GameObject newBullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        Bullet bullet = newBullet.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetDirection(direction);
            bullet.speed = Mathf.Clamp(velocity * 2f, 5f, 15f);
        }
        Debug.Log("弾発射！速度:" + velocity);
    }
}
