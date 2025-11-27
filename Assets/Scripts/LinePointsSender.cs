using UnityEngine;

public class LinePointsSender : MonoBehaviour
{
    public LineRenderer lr;

    void Update()
    {
        IntersectionManager.instance.A = lr.GetPosition(0);
        IntersectionManager.instance.B = lr.GetPosition(2);
        IntersectionManager.instance.C = lr.GetPosition(1);
        IntersectionManager.instance.D = lr.GetPosition(3);
    }
}