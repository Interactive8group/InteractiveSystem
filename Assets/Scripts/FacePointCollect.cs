using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacePointCollect : MonoBehaviour
{
    public static FacePointCollect instance;

    [Header("状態")]
    public bool collectFinish = false;

    private List<GameObject> childList = new List<GameObject>();
    private Camera uiCamera;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        AutoAssignCamera();
        CollectChildren();
        collectFinish = true;
    }

    void Update()
    {
        // MediaPipeなどで子が後から生成される場合の保険
        if (childList.Count == 0)
        {
            CollectChildren();
        }
    }

    /// <summary>
    /// Canvas / MainCamera から自動でカメラを取得
    /// </summary>
    void AutoAssignCamera()
    {
        // ① 親Canvasを探す
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            // Screen Space - Camera の場合
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera &&
                canvas.worldCamera != null)
            {
                uiCamera = canvas.worldCamera;
                return;
            }
        }

        // ② MainCamera
        if (Camera.main != null)
        {
            uiCamera = Camera.main;
            return;
        }

        // ③ シーン内の最初のカメラ
        Camera cam = FindObjectOfType<Camera>();
        if (cam != null)
        {
            uiCamera = cam;
            return;
        }

        Debug.LogError("FacePointCollect: カメラが見つかりません");
    }

    /// <summary>
    /// 子オブジェクトを収集
    /// </summary>
    void CollectChildren()
    {
        childList.Clear();
        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);
        }
    }

    /// <summary>
    /// 顔中心を「画面内相対座標（0〜1）」で返す
    /// Canvasの位置変更に影響されない
    /// </summary>
    public Vector2 GetFaceCenter01()
    {
        if (childList.Count == 0 || uiCamera == null)
        {
            return new Vector2(0.5f, 0.5f);
        }

        Vector3 sum = Vector3.zero;
        foreach (GameObject point in childList)
        {
            sum += point.transform.position;
        }

        Vector3 worldCenter = sum / childList.Count;

        Vector3 screenPos = uiCamera.WorldToScreenPoint(worldCenter);

        float x01 = Mathf.Clamp01(screenPos.x / Screen.width);
        float y01 = Mathf.Clamp01(screenPos.y / Screen.height);

        return new Vector2(x01, y01);
    }
}
