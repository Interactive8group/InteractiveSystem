using UnityEngine;

public class IntersectionManager : MonoBehaviour
{
    public static IntersectionManager instance;

    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public Vector3 D;

    public Vector3 intersectionPoint;
    public bool hasIntersection = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

        hasIntersection = LineIntersection2D(A, B, C, D, out intersectionPoint);

        if (hasIntersection)
        {
            Debug.Log("交点 → " + intersectionPoint);
        }
    }

    bool LineIntersection2D(Vector3 A, Vector3 B, Vector3 C, Vector3 D, out Vector3 I)
    {
        I = Vector3.zero;

        Vector2 a = A;
        Vector2 b = B;
        Vector2 c = C;
        Vector2 d = D;

        Vector2 r = b - a;
        Vector2 s = d - c;

        float rxs = r.x * s.y - r.y * s.x;
        float qpxr = (c - a).x * r.y - (c - a).y * r.x;

        if (Mathf.Abs(rxs) < 0.00001f)
        {
            return false;  // 平行
        }

        float t = ((c - a).x * s.y - (c - a).y * s.x) / rxs;
        float u = qpxr / rxs;

        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            Vector2 p = a + t * r;
            I = new Vector3(p.x, p.y, 0f);
            return true;
        }

        return false;
    }
}