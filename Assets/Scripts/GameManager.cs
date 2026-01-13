using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject _gameoverPanel;
    public GameObject enemyPrefab;

    public int hart = 3;
    public float spawnInterval = 1f;
    private float timer = 0f;

    public static GameManager instance;

    private bool isGameStarted = false; // ゲーム開始フラグ

    void Awake()
    {
        instance = this;
        _gameoverPanel.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (!isGameStarted) return; // ゲーム開始前は何もしない

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }

        if (hart <= 0)
        {
            _gameoverPanel.SetActive(true);
            Time.timeScale = 0f; // ゲームを止める
        }
    }

    // 3秒カウントダウン
    IEnumerator StartCountdown()
    {
        float countdown = 3f;
        while (countdown > 0f)
        {
            text.text = Mathf.Ceil(countdown).ToString(); // 3,2,1と表示
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        text.text = "GO!"; // GO!を表示
        yield return new WaitForSeconds(1f);
        text.text = ""; // テキストを消す
        isGameStarted = true; // ゲーム開始
    }

    void SpawnEnemy()
    {
        Camera mainCam = Camera.main;
        Vector3 bottomLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10f));

        Vector3 spawnPos = Vector3.zero;
        float side = Random.value;

        if (side < 0.25f) // 左
            spawnPos = new Vector3(bottomLeft.x - 1f, Random.Range(bottomLeft.y, topRight.y), 0f);
        else if (side < 0.5f) // 右
            spawnPos = new Vector3(topRight.x + 1f, Random.Range(bottomLeft.y, topRight.y), 0f);
        else if (side < 0.75f) // 上
            spawnPos = new Vector3(Random.Range(bottomLeft.x, topRight.x), topRight.y + 1f, 0f);
        else // 下
            spawnPos = new Vector3(Random.Range(bottomLeft.x, topRight.x), bottomLeft.y - 1f, 0f);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Vector3 targetPos = new Vector3(
            Random.Range(bottomLeft.x, topRight.x),
            Random.Range(bottomLeft.y, topRight.y),
            0f
        );

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetDirection(targetPos);
        }
    }

    public void TextChange(string newText)
    {
        text.text = newText;
    }
}
