using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

public class WebCamSender : MonoBehaviour
{
    public RawImage myFaceImage;

    void Start()
    {
        WebCamTexture cam = new WebCamTexture();
        cam.Play();

        myFaceImage.texture = cam;

        var track = new VideoStreamTrack(cam);
        WebRTCManager.instance.SetLocalVideo(track);
    }
}
