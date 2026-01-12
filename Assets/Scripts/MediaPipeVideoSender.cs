using UnityEngine;
using Unity.WebRTC;
using System.Collections;

public class MediaPipeVideoSender : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.RawImage annotatableRawImage;

    private RenderTexture renderTexture;
    private VideoStreamTrack videoTrack;

    private int rtWidth = 1280;
    private int rtHeight = 720;

    IEnumerator Start()
    {
        while (annotatableRawImage.texture == null)
            yield return null;

        // RenderTexture を自動生成
        renderTexture = new RenderTexture(rtWidth, rtHeight, 0);
        renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.B8G8R8A8_UNorm;
        renderTexture.Create();

        videoTrack = new VideoStreamTrack(renderTexture);
        WebRTCManager.instance.SetLocalVideo(videoTrack);
    }

    void Update()
    {
        if (annotatableRawImage.texture != null && renderTexture != null)
            Graphics.Blit(annotatableRawImage.texture, renderTexture);
    }
}
