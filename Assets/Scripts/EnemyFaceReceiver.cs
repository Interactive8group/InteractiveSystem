using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;

public class EnemyFaceReceiver : MonoBehaviour
{
    public static EnemyFaceReceiver instance;

    [SerializeField] private RawImage enemyImage;

    private RenderTexture renderTexture;

    void Awake()
    {
        instance = this;
    }

    public void SetRemoteVideo(VideoStreamTrack track)
    {
        Debug.Log("SetRemoteVideo called");

        // ★ RenderTexture を作る（これが超重要）
        renderTexture = new RenderTexture(1280, 720, 0);
        renderTexture.Create();

        enemyImage.texture = renderTexture;

        // ★ 受信フレームを必ずコピー
        track.OnVideoReceived += tex =>
        {
            if (renderTexture == null) return;
            Graphics.Blit(tex, renderTexture);
        };
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }
    }
}
