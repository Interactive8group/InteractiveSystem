using UnityEngine;
using WebSocketSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.WebRTC;

public class SignalingClient : MonoBehaviour
{
    [SerializeField] private bool isHost;
    [SerializeField] private bool isOfferer = false;

    public static SignalingClient instance;

    private WebSocket ws;
    private bool isOpen = false;

    // ★ WebSocket スレッド → Unity メインスレッド受け渡し用
    private readonly Queue<string> messageQueue = new Queue<string>();

    void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        ws = new WebSocket("ws://192.168.11.18:3000");

        ws.OnOpen += (s, e) =>
        {
            Debug.Log("WebSocket Open");
            isOpen = true;
        };

        ws.OnMessage += (s, e) =>
        {
            lock (messageQueue)
            {
                messageQueue.Enqueue(e.Data);
            }
        };

        ws.Connect();

        yield return new WaitUntil(() => isOpen);

        // ★ Offer を作るのは片方だけ
        if (!isOfferer)
        {
            Debug.Log("This peer waits for offer");
            yield break;
        }

        Debug.Log("Create Offer");

        yield return WebRTCManager.instance.CreateOfferCoroutine(offer =>
        {
            Send(JsonUtility.ToJson(new SignalingMessage
            {
                type = "offer",
                sdp = offer.sdp
            }));
        });
    }


    void Update()
    {
        // ★ Unity メインスレッドで安全に処理
        if (messageQueue.Count > 0)
        {
            string msg;
            lock (messageQueue)
            {
                msg = messageQueue.Dequeue();
            }

            StartCoroutine(ProcessMessage(msg));
        }
    }

    public void Send(string message)
    {
        if (!isOpen)
        {
            Debug.LogWarning("WebSocket not open yet. Skip Send.");
            return;
        }

        Debug.Log("Send signaling: " + message);
        ws.Send(message);
    }

    IEnumerator ProcessMessage(string msg)
    {
        Debug.Log("ProcessMessage: " + msg);

        SignalingMessage obj;
        try
        {
            obj = JsonUtility.FromJson<SignalingMessage>(msg);
        }
        catch
        {
            Debug.LogError("Invalid JSON");
            yield break;
        }

        if (obj == null || string.IsNullOrEmpty(obj.type))
        {
            Debug.LogWarning("Invalid signaling message");
            yield break;
        }

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
