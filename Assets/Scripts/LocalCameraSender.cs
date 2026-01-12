using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

public class LocalCameraSender : MonoBehaviour
{
    [SerializeField] RawImage previewImage;

    WebCamTexture webcam;
    VideoStreamTrack videoTrack;

    void Start()
    {
        Debug.Log("LocalCameraSender Start");

        webcam = new WebCamTexture(640, 480);
        webcam.Play();

        Debug.Log("WebCamTexture Play: " + webcam.isPlaying);

        previewImage.texture = webcam;

        videoTrack = new VideoStreamTrack(webcam);
        Debug.Log("VideoStreamTrack created: " + (videoTrack != null));

        WebRTCManager.instance.SetLocalVideo(videoTrack);
    }

}
