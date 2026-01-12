using UnityEngine;
using WebSocketSharp;
using Unity.WebRTC;
using System.Collections;

public class SignalingClient : MonoBehaviour
{
    public static SignalingClient instance;
    private WebSocket ws;

    void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        ws = new WebSocket("ws://192.168.11.18:3000"); // ← サーバPCのIP
        ws.OnMessage += (s, e) => StartCoroutine(ProcessMessage(e.Data));
        ws.Connect();

        // WebRTC / MediaPipe 初期化待ち
        yield return new WaitForSeconds(1.0f);

        // ★ これが無いと一生映らない
        Debug.Log("Create Offer");
        yield return WebRTCManager.instance.CreateOfferCoroutine(offer =>
        {
            Debug.Log("Send Offer");
            Send(JsonUtility.ToJson(new SignalingMessage
            {
                type = "offer",
                sdp = offer.sdp
            }));
        });
    }

    public void Send(string message)
    {
        Debug.Log("Send signaling: " + message);
        ws.Send(message);
    }

    IEnumerator ProcessMessage(string msg)
    {
        Debug.Log("Receive signaling: " + msg);
        var obj = JsonUtility.FromJson<SignalingMessage>(msg);

        if (obj.type == "offer")
        {
            Debug.Log("Receive Offer");
            RTCSessionDescription offer = new RTCSessionDescription
            {
                type = RTCSdpType.Offer,
                sdp = obj.sdp
            };

            yield return WebRTCManager.instance.SetRemoteDescriptionCoroutine(offer);
            yield return WebRTCManager.instance.CreateAnswerCoroutine(answer =>
            {
                Debug.Log("Send Answer");
                Send(JsonUtility.ToJson(new SignalingMessage
                {
                    type = "answer",
                    sdp = answer.sdp
                }));
            });
        }
        else if (obj.type == "answer")
        {
            Debug.Log("Receive Answer");
            RTCSessionDescription answer = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = obj.sdp
            };
            yield return WebRTCManager.instance.SetRemoteDescriptionCoroutine(answer);
        }
        else if (obj.type == "ice")
        {
            Debug.Log("Receive ICE");
            RTCIceCandidate candidate = new RTCIceCandidate(
                new RTCIceCandidateInit
                {
                    candidate = obj.candidate,
                    sdpMid = obj.sdpMid,
                    sdpMLineIndex = obj.sdpMLineIndex
                }
            );
            WebRTCManager.instance.AddIceCandidate(candidate);
        }
    }

    [System.Serializable]
    public class SignalingMessage
    {
        public string type;
        public string sdp;
        public string candidate;
        public string sdpMid;
        public int sdpMLineIndex;
    }
}
