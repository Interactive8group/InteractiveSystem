using UnityEngine;

public class FaceBulletShooter : MonoBehaviour
{
    [Header("弾")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawnPoint;

    [Header("首振り感度（下振りのみ）")]
    [SerializeField] float verticalMoveThreshold = 0.03f;
    [SerializeField] float verticalSpeedThreshold = 0.8f;
    [SerializeField] float fireCooldown = 0.4f;

    [Header("弾速度")]
    [SerializeField] float minBulletSpeed = 6f;
    [SerializeField] float maxBulletSpeed = 14f;

    private Vector3 lastFaceCenter;
    private float cooldownTimer;

    void Start()
    {
        if (FacePointCollect.instance != null)
        {
            lastFaceCenter = FacePointCollect.instance.GetFaceCenter();
        }
    }

    void Update()
    {
        if (FacePointCollect.instance == null) return;
        if (!FacePointCollect.instance.collectFinish) return;

        cooldownTimer += Time.deltaTime;

        Vector3 currentCenter = FacePointCollect.instance.GetFaceCenter();
        Vector3 delta = currentCenter - lastFaceCenter;

        float verticalMove = delta.y;
        float verticalSpeed = delta.y / Time.deltaTime;

        // ★ 下振りだけ反応（マイナス方向）
        if (verticalMove < -verticalMoveThreshold &&
            verticalSpeed < -verticalSpeedThreshold &&
            cooldownTimer >= fireCooldown)
        {
            Fire(-verticalSpeed); // マイナスを正のパワーに
            cooldownTimer = 0f;
        }

        lastFaceCenter = currentCenter;
    }

    void Fire(float power)
    {
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(Vector3.forward);
            bulletScript.speed = Mathf.Clamp(
                power * 1.5f,
                minBulletSpeed,
                maxBulletSpeed
            );
        }
    }
}
