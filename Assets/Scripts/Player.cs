using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviourPun
{
    [Header("Shoot 2D")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float bulletSpeed = 8f;
    [SerializeField] float shakeThreshold = 0.025f;
    [SerializeField] float shootCooldown = 0.3f;

    Vector3 prevFacePos;
    bool canShoot = true;

    [Header("Move")]
    [SerializeField] float speed = 0.01f;
    [SerializeField] Vector3 pos_config;

    [Header("UI")]
    [SerializeField] GameObject fukidasi;

    void Update()
    {
        // ★ 自分のPlayerだけ処理
        if (!photonView.IsMine) return;

        PlayerMove();
        FaceShakeShoot2D();
    }

    void PlayerMove()
    {
        if (FacePointCollect.instance != null &&
            FacePointCollect.instance.collectFinish)
        {
            transform.position = FacePointCollect.instance.GetFaceCenter();
        }
    }

    // ======================
    // 顔振り → 発射
    // ======================
    void FaceShakeShoot2D()
    {
        // ★ Photon所有チェック
        if (!photonView.IsMine) return;

        // ★ FacePointCollect 存在チェック
        if (FacePointCollect.instance == null) return;
        if (!FacePointCollect.instance.collectFinish) return;

        if (!canShoot) return;

        Vector3 currentFacePos = FacePointCollect.instance.GetFaceCenter();

        // 初回対策
        if (prevFacePos == Vector3.zero)
        {
            prevFacePos = currentFacePos;
            return;
        }

        Vector3 delta = currentFacePos - prevFacePos;

        // ミラー補正
        delta.x *= -1;

        Vector2 shootDir = Vector2.zero;

        if (Mathf.Abs(delta.x) > shakeThreshold)
        {
            shootDir = delta.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (Mathf.Abs(delta.y) > shakeThreshold)
        {
            shootDir = delta.y > 0 ? Vector2.up : Vector2.down;
        }

        if (shootDir != Vector2.zero)
        {
            Shoot2D(shootDir);
            canShoot = false;
            Invoke(nameof(ResetShoot), shootCooldown);
        }

        prevFacePos = currentFacePos;
    }

    void ResetShoot()
    {
        canShoot = true;
    }

    // ======================
    // ネットワーク発射
    // ======================
    void Shoot2D(Vector2 dir)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (!PhotonNetwork.InRoom) return;

        photonView.RPC(
            nameof(RPC_Shoot),
            RpcTarget.All,
            dir,
            bulletSpawnPoint.position
        );
    }


    [PunRPC]
    void RPC_Shoot(Vector2 dir, Vector3 pos)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * bulletSpeed;
    }

    // ======================
    // 被弾判定（2D）
    // ======================
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            GameManager.instance.TextChange("いた～");
            ViewFukidashi();
        }
    }

    void ViewFukidashi()
    {
        fukidasi.SetActive(true);
        StartCoroutine(HideAfter3Seconds());
    }

    IEnumerator HideAfter3Seconds()
    {
        yield return new WaitForSeconds(3f);
        fukidasi.SetActive(false);
    }
}
