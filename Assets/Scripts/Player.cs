using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 0.01f;
    [Header("オブジェクトの位置の微調整"), SerializeField] Vector3 pos_config;
    [SerializeField] float moveLimit_up = 0, moveLimit_bottom = 0, moveLimit_left = 0, moveLimit_right = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        // if (IntersectionManager.instance.hasIntersection)
        // {
        //     transform.position = (IntersectionManager.instance.intersectionPoint + pos_config) * speed;
        // }

        if (FacePointCollect.instance != null && FacePointCollect.instance.collectFinish)
        {
            if (FacePointCollect.instance.childList.Count > 4)
            {
                transform.position = FacePointCollect.instance.childList[4].transform.position;
                Debug.Log("transform.position←" + FacePointCollect.instance.childList[4].transform.position);
            }
        }

        // // 現在の位置を取得
        // Vector3 pos = transform.position;

        // // Y方向を制限
        // pos.y = Mathf.Clamp(pos.y, moveLimit_bottom, moveLimit_up);

        // // X方向を制限
        // pos.x = Mathf.Clamp(pos.x, moveLimit_left, moveLimit_right);

        // // 修正した位置を再代入
        // transform.position = pos;
    }
}
