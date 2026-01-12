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

        // WebRTC.Initialize(); ← 既に別で呼んでいるなら不要

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
        Debug.Log("SetLocalVideo called");
        localTrack = track;
        pc.AddTrack(track);
    }

    public IEnumerator CreateOfferCoroutine(System.Action<RTCSessionDescription> callback)
    {
        var op = pc.CreateOffer();
        yield return op;

        if (op.IsError)
        {
            Debug.LogError("CreateOffer Error");
            yield break;
        }

        var desc = op.Desc;
        yield return pc.SetLocalDescription(ref desc);

        callback?.Invoke(desc);
    }

    public IEnumerator CreateAnswerCoroutine(System.Action<RTCSessionDescription> callback)
    {
        var op = pc.CreateAnswer();
        yield return op;

        if (op.IsError)
        {
            Debug.LogError("CreateAnswer Error");
            yield break;
        }

        var desc = op.Desc;
        yield return pc.SetLocalDescription(ref desc);

        callback?.Invoke(desc);
    }

    public IEnumerator SetRemoteDescriptionCoroutine(RTCSessionDescription desc)
    {
        var op = pc.SetRemoteDescription(ref desc);
        yield return op;

        if (op.IsError)
        {
            Debug.LogError("SetRemoteDescription Error");
        }
        else
        {
            Debug.Log("SetRemoteDescription done");
        }
    }

    public void AddIceCandidate(RTCIceCandidate candidate)
    {
        pc.AddIceCandidate(candidate);
    }
}
