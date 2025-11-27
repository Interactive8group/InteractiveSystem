using System.Collections.Generic;
using UnityEngine;

public class FacePointCollect : MonoBehaviour
{
    public List<GameObject> childList = new List<GameObject>();
    public static FacePointCollect instance;
    public bool collectFinish = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // このGameObjectの直下の子をすべて取得
        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);
        }

        collectFinish = true;
    }

    void Update()
    {
        // 必要に応じて子の位置を確認したり処理を追加
    }

    /// <summary>
    /// 全てのポイントの重心（平均位置）を返す
    /// </summary>
    public Vector3 GetFaceCenter()
    {
        if (childList.Count == 0) return transform.position;

        Vector3 sum = Vector3.zero;
        foreach (GameObject point in childList)
        {
            sum += point.transform.position;
        }
        return sum / childList.Count;
    }
}
