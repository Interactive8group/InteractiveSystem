using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 0.01f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        if (IntersectionManager.instance.hasIntersection)
        {
            // Vector3 p = IntersectionManager.instance.intersectionPoint;
            transform.position = IntersectionManager.instance.intersectionPoint * speed;
            // Debug.Log("交点 → " + p);
        }
    }
}
