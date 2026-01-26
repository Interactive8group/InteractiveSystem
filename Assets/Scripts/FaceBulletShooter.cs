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

    private Vector2 lastFace01;
    private float cooldownTimer;

    void Start()
    {
        if (FacePointCollect.instance != null)
        {
            lastFace01 = FacePointCollect.instance.GetFaceCenter01();
        }
    }

    void Update()
    {
        if (FacePointCollect.instance == null) return;
        if (!FacePointCollect.instance.collectFinish) return;

        cooldownTimer += Time.deltaTime;

        Vector2 current01 = FacePointCollect.instance.GetFaceCenter01();
        Vector2 delta = current01 - lastFace01;

        float verticalMove = delta.y;
        float verticalSpeed = delta.y / Time.deltaTime;

        // ★ 下振りのみ
        if (verticalMove < -verticalMoveThreshold &&
            verticalSpeed < -verticalSpeedThreshold &&
            cooldownTimer >= fireCooldown)
        {
            Fire(-verticalSpeed);
            cooldownTimer = 0f;
        }

        lastFace01 = current01;
    }

    void Fire(float power)
    {
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            spawnPoint.position,
            Quaternion.Euler(0, 90, 0) // Y軸90°回転で見た目だけ回転
        );

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(Vector3.forward); // 弾の進む方向は変えない
            bulletScript.speed = Mathf.Clamp(
                power * 1.5f,
                minBulletSpeed,
                maxBulletSpeed
            );
        }

        SoundManager.Instance.PlaySE(0);
    }

}
