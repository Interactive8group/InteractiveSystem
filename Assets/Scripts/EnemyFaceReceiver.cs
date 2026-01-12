using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;

public class EnemyFaceReceiver : MonoBehaviour
{
    public static EnemyFaceReceiver instance;

    [SerializeField] private RawImage enemyImage;
    private VideoStreamTrack remoteTrack;

    void Awake() => instance = this;

    public void SetRemoteVideo(VideoStreamTrack track)
    {
        remoteTrack = track;
        track.OnVideoReceived += tex => enemyImage.texture = tex;
    }
}
