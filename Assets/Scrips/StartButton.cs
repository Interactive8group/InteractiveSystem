using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var button = GetComponent<Button>();

        //ボタンを押したときのリスナー設定
        button.onClick.AddListener(() =>
        {
            //シーン変移の際にSceneManagerを使用する
            SceneManager.LoadScene("MainScene");
        });
    }
}
