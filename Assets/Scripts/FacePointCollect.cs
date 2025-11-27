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
}
