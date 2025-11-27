using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    // アイテムの種類定義
    public enum ItemType
    {
        Wood,     // 木
        Stone,    // 石
        ThrowAxe  // 投げオノ（未定と付け替え）
    }

    [SerializeField] private ItemType type;

    // 初期化処理
    public void Initialize()
    {
        // アニメーションが終わるまでColliderを無効化する
        var colliderCache = GetComponent<Collider>();
        colliderCache.enabled = false;

        // 簡単なアニメーション
        var transformCache = transform;
        var dropPosition = transform.localPosition +
            new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        transformCache.DOLocalMove(dropPosition, 0.5f);
        var defaultScale = transformCache.localScale;
        transformCache.localScale = Vector3.zero;

        transformCache.DOScale(defaultScale, 0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                // アニメーションが終わったらColliderを有効化する
                colliderCache.enabled = true;
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // TODO: プレイヤーの所持数などに加算する

        // オブジェクトを破壊する
        Destroy(gameObject);
    }
}
