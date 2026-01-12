using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    public GameObject bulletPrefab;
    public float spawnInterval = 1f;
    private float timer = 0f;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBullet();
            timer = 0f;
        }
    }

    void SpawnBullet()
    {
        Camera mainCam = Camera.main;
        Vector3 bottomLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 80f));

        // ランダムに画面外座標を決定
        Vector3 spawnPos = Vector3.zero;
        float side = Random.value;

        if (side < 0.25f)
            spawnPos = new Vector3(bottomLeft.x - 1f, Random.Range(bottomLeft.y, topRight.y), 80f);
        else if (side < 0.5f)
            spawnPos = new Vector3(topRight.x + 1f, Random.Range(bottomLeft.y, topRight.y), 80f);
        else if (side < 0.75f)
            spawnPos = new Vector3(Random.Range(bottomLeft.x, topRight.x), topRight.y + 1f, 80f);
        else
            spawnPos = new Vector3(Random.Range(bottomLeft.x, topRight.x), bottomLeft.y - 1f, 80f);

        // // 弾を生成
        // GameObject newBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // // 画面中央をターゲットに設定
        // Vector3 targetPos = (bottomLeft + topRight) / 2f;
        // newBullet.GetComponent<Bullet>().SetTarget(targetPos);
    }

    public void TextChange(string newText)
    {
        text.text = newText;
    }
}
