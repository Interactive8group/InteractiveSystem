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
    [Header("Enemy Spawn Area (Viewport)")]
    [SerializeField, Range(0f, 1f)] private float spawnMinX = 0.1f;
    [SerializeField, Range(0f, 1f)] private float spawnMaxX = 0.9f;
    [SerializeField, Range(0f, 1f)] private float spawnMinY = 0.1f;
    [SerializeField, Range(0f, 1f)] private float spawnMaxY = 0.9f;


    public static GameManager instance;

    private float timer = 0f;
    private bool isGameStarted = false;
    private int enemiesSpawned = 0;
    private bool bossSpawned = false;
    private int aliveEnemies = 0;   // ★ 今生きている雑魚

    void Awake()
    {
        instance = this;
        _gameoverPanel.SetActive(false);
        StartCoroutine(StartCountdown());

        // ゲーム開始前にBGM停止
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopBGM();
    }

    void Update()
    {
        if (!isGameStarted) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            if (enemiesSpawned < maxEnemies)
            {
                // 雑魚敵出現時は雑魚BGM（ID 0）再生
                if (SoundManager.Instance != null && !SoundManager.Instance.bgmSource.isPlaying)
                {
                    SoundManager.Instance.PlayBGM(0, true);
                }

                int spawnCount = Random.Range(spawnCountMin, spawnCountMax + 1);

                for (int i = 0; i < spawnCount; i++)
                {
                    if (enemiesSpawned >= maxEnemies) break;

                    SpawnEnemy();
                    enemiesSpawned++;
                }
            }
            else if (!bossSpawned && aliveEnemies <= 0)
            {
                // ボス出現時はBGM ID 1
                SpawnBoss();
                bossSpawned = true;

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBGM(1, true);
                }
            }

            timer = 0f;
        }

        if (hart <= 0)
        {
            _gameoverPanel.SetActive(true);
            Time.timeScale = 0f;

            // ゲームオーバーでBGM停止
            if (SoundManager.Instance != null)
                SoundManager.Instance.StopBGM();
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

        float vx = Random.Range(spawnMinX, spawnMaxX);
        float vy = Random.Range(spawnMinY, spawnMaxY);

        Vector3 spawnPos = cam.ViewportToWorldPoint(
            new Vector3(vx, vy, enemySpawnZ)
        );

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        aliveEnemies++;

        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.SetDirection(new Vector3(spawnPos.x, spawnPos.y, targetZ));
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

        // ゲームクリアでBGM停止
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopBGM();
    }

    public void TextChange(string newText)
    {
        if (text != null)
        {
            text.text = newText;
        }
    }

    public void OnEnemyDead()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }

}
