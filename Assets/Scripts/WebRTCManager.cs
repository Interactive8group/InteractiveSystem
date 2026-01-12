using UnityEngine;
using Unity.WebRTC;
using System.Collections;

public class WebRTCManager : MonoBehaviour
{
    public static WebRTCManager instance;

    private RTCPeerConnection pc;

    void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        Debug.Log("WebRTC Initialize");
        //WebRTC.Initialize();

        yield return null; // ★ 1フレーム待つ

        pc = new RTCPeerConnection();

        pc.OnIceCandidate = candidate =>
        {
            if (candidate == null) return;
            if (SignalingClient.instance == null) return;

            SignalingClient.instance.Send(
                JsonUtility.ToJson(new SignalingClient.SignalingMessage
                {
                    type = "ice",
                    candidate = candidate.Candidate,
                    sdpMid = candidate.SdpMid,
                    sdpMLineIndex = candidate.SdpMLineIndex.Value
                })
            );
        };

        pc.OnTrack = e =>
        {
            Debug.Log("OnTrack called");

            if (e.Track is VideoStreamTrack videoTrack)
            {
                EnemyFaceReceiver.instance.SetRemoteVideo(videoTrack);
            }
        };
    }

    public void SetLocalVideo(VideoStreamTrack track)
    {
        Debug.Log("SetLocalVideo");
        pc.AddTrack(track);
    }

    public IEnumerator CreateOfferCoroutine(System.Action<RTCSessionDescription> callback)
    {
        var op = pc.CreateOffer();
        yield return op;

        var desc = op.Desc;
        yield return pc.SetLocalDescription(ref desc);

        callback?.Invoke(desc);
    }

    public IEnumerator CreateAnswerCoroutine(System.Action<RTCSessionDescription> callback)
    {
        var op = pc.CreateAnswer();
        yield return op;

        var desc = op.Desc;
        yield return pc.SetLocalDescription(ref desc);

        callback?.Invoke(desc);
    }

    public IEnumerator SetRemoteDescriptionCoroutine(RTCSessionDescription desc)
    {
        var op = pc.SetRemoteDescription(ref desc);
        yield return op;

        Debug.Log("SetRemoteDescription done");
    }

    public void AddIceCandidate(RTCIceCandidate candidate)
    {
        // RemoteDescription がまだ無い場合は弾く
        if (string.IsNullOrEmpty(pc.RemoteDescription.sdp))
        {
            Debug.LogWarning("ICE before RemoteDescription → ignore");
            return;
        }

        pc.AddIceCandidate(candidate);
    }

    void OnDestroy()
    {
        pc?.Close();
        pc?.Dispose();
        //WebRTC.Dispose();
    }
}
