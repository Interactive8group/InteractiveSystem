using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject _gameoverPanel;

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;      // 雑魚
    [SerializeField] GameObject bossPrefab;       // ボス
    [SerializeField] GameObject enemyBulletPrefab;

    [Header("Spawn Position (Z)")]
    [SerializeField] private float enemySpawnZ = 25f;
    [SerializeField] private float bossSpawnZ = 35f;

    [Header("Enemy Spawn Count")]
    [SerializeField] private int spawnCountMin = 1; // 1回の出現で最低何体
    [SerializeField] private int spawnCountMax = 3; // 1回の出現で最大何体

    [Header("Game Settings")]
    public int hart = 3;
    public float spawnInterval = 1f;
    [SerializeField] private int maxEnemies = 10;

    public static GameManager instance;

    private float timer = 0f;
    private bool isGameStarted = false;
    private int enemiesSpawned = 0;
    private bool bossSpawned = false;

    void Awake()
    {
        instance = this;
        _gameoverPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (!isGameStarted) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            if (enemiesSpawned < maxEnemies)
            {
                int spawnCount = Random.Range(spawnCountMin, spawnCountMax + 1);

                for (int i = 0; i < spawnCount; i++)
                {
                    if (enemiesSpawned >= maxEnemies) break;

                    SpawnEnemy();
                    enemiesSpawned++;
                }
            }
            else if (!bossSpawned)
            {
                SpawnBoss();
                bossSpawned = true;
            }

            timer = 0f;
        }

        if (hart <= 0)
        {
            _gameoverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    IEnumerator StartCountdown()
    {
        float countdown = 3f;

        while (countdown > 0f)
        {
            text.text = Mathf.Ceil(countdown).ToString();
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        text.text = "GO!";
        yield return new WaitForSeconds(1f);
        text.text = "";
        isGameStarted = true;
    }

    void SpawnEnemy()
    {
        Camera cam = Camera.main;
        GameObject player = GameObject.FindWithTag("Player");

        float targetZ = player != null ? player.transform.position.z : 0f;

        Vector3 bl = cam.ScreenToWorldPoint(
            new Vector3(0, 0, cam.nearClipPlane)
        );
        Vector3 tr = cam.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, cam.nearClipPlane)
        );

        // ★ 出現位置（xyランダム）
        Vector3 spawnPos = new Vector3(
            Random.Range(bl.x, tr.x),
            Random.Range(bl.y, tr.y),
            enemySpawnZ
        );

        // ★ 移動先もランダム
        Vector3 targetPos = new Vector3(
            Random.Range(bl.x, tr.x),
            Random.Range(bl.y, tr.y),
            targetZ
        );

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.SetDirection(targetPos);
        }
    }

    void SpawnBoss()
    {
        Camera cam = Camera.main;

        Vector3 center = cam.ScreenToWorldPoint(
            new Vector3(Screen.width / 2, Screen.height / 2, bossSpawnZ)
        );

        Instantiate(bossPrefab, center, Quaternion.identity);
    }

    // ★ ボス撃破時に呼ぶ
    public void GameClear()
    {
        text.text = "CLEAR!";
        Time.timeScale = 0f;
    }

    public void TextChange(string newText)
    {
        if (text != null)
        {
            text.text = newText;
        }
    }
}
