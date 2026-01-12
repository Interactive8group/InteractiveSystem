using System.Collections.Generic;
using UnityEngine;

public class FacePointCollect : MonoBehaviour
{
    public static FacePointCollect instance;

    public List<GameObject> childList = new List<GameObject>();
    public bool collectFinish = false;

    void Awake()
    {
        // ★ シングルトン保証
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        childList.Clear();

        foreach (Transform child in transform)
        {
            // 非アクティブ除外（重要）
            if (child.gameObject.activeInHierarchy)
            {
                childList.Add(child.gameObject);
            }
        }

        collectFinish = childList.Count > 0;
    }

    public Vector3 GetFaceCenter()
    {
        if (!collectFinish || childList.Count == 0)
            return transform.position;

        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (GameObject point in childList)
        {
            if (point == null) continue;

            sum += point.transform.position;
            count++;
        }

        if (count == 0) return transform.position;

        return sum / count;
    }
}
