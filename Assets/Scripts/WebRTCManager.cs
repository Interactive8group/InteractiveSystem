using UnityEngine;
using Unity.WebRTC;
using System.Collections;

public class WebRTCManager : MonoBehaviour
{
    public static WebRTCManager instance;
    private RTCPeerConnection pc;
    private VideoStreamTrack localTrack;

    void Awake()
    {
        instance = this;
        //WebRTC.Initialize();
        pc = new RTCPeerConnection();

        pc.OnIceCandidate = c =>
        {
            if (c != null)
            {
                SignalingClient.instance.Send(
                    JsonUtility.ToJson(new SignalingClient.SignalingMessage
                    { type = "ice", candidate = c.Candidate, sdpMid = c.SdpMid, sdpMLineIndex = c.SdpMLineIndex.Value })
                );
            }
        };

        pc.OnTrack = e =>
        {
            if (e.Track is VideoStreamTrack videoTrack)
                EnemyFaceReceiver.instance.SetRemoteVideo(videoTrack);
        };
    }

    public void SetLocalVideo(VideoStreamTrack track)
    {
        localTrack = track;
        pc.AddTrack(track);
    }

    public IEnumerator CreateOfferCoroutine(System.Action<RTCSessionDescription> callback)
    {
        var op = pc.CreateOffer();
        yield return op;

        if (!op.IsError)
        {
            var desc = op.Desc;                 // ここを修正
            yield return pc.SetLocalDescription(ref desc);
            callback?.Invoke(desc);
        }
    }

    public IEnumerator CreateAnswerCoroutine(System.Action<RTCSessionDescription> callback)
    {
        var op = pc.CreateAnswer();
        yield return op;

        if (!op.IsError)
        {
            var desc = op.Desc;                 // ここを修正
            yield return pc.SetLocalDescription(ref desc);
            callback?.Invoke(desc);
        }
    }


    public IEnumerator SetRemoteDescriptionCoroutine(RTCSessionDescription desc)
    {
        var op = pc.SetRemoteDescription(ref desc);
        yield return op;
    }

    public void AddIceCandidate(RTCIceCandidate candidate) => pc.AddIceCandidate(candidate);
}
