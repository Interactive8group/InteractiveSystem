using UnityEngine;
using UnityEngine.UI;

public class TextureCopy : MonoBehaviour
{
    [SerializeField] RawImage sourceRawImage;
    private RawImage myRawImage;
    private bool isSet = false;

    void Awake()
    {
        myRawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (isSet) return;

        if (myRawImage.texture == null &&
            sourceRawImage != null &&
            sourceRawImage.texture != null)
        {
            myRawImage.texture = sourceRawImage.texture;
            isSet = true; // ★ 一度だけ
        }
    }
}
