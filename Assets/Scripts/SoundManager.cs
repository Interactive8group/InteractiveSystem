using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource seSource;

    [Header("Audio Clips")]
    public List<AudioClip> bgmClips;
    public List<AudioClip> seClips;

    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionaries()
    {
        for (int i = 0; i < bgmClips.Count; i++)
        {
            bgmDict[bgmClips[i].name] = bgmClips[i];
        }
        for (int i = 0; i < seClips.Count; i++)
        {
            seDict[seClips[i].name] = seClips[i];
        }
    }

    // 名前でBGM再生
    public void PlayBGM(string name, bool loop = true)
    {
        if (bgmDict.ContainsKey(name))
        {
            bgmSource.clip = bgmDict[name];
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGMが見つかりません: " + name);
        }
    }

    // IDでBGM再生
    public void PlayBGM(int id, bool loop = true)
    {
        if (id >= 0 && id < bgmClips.Count)
        {
            bgmSource.clip = bgmClips[id];
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM IDが範囲外です: " + id);
        }
    }

    // 名前でSE再生
    public void PlaySE(string name)
    {
        if (seDict.ContainsKey(name))
        {
            seSource.PlayOneShot(seDict[name]);
        }
        else
        {
            Debug.LogWarning("SEが見つかりません: " + name);
        }
    }

    // IDでSE再生
    public void PlaySE(int id)
    {
        if (id >= 0 && id < seClips.Count)
        {
            seSource.PlayOneShot(seClips[id]);
        }
        else
        {
            Debug.LogWarning("SE IDが範囲外です: " + id);
        }
    }

    // BGM停止
    public void StopBGM() => bgmSource.Stop();

    // 音量調整
    public void SetBGMVolume(float volume) => bgmSource.volume = Mathf.Clamp01(volume);
    public void SetSEVolume(float volume) => seSource.volume = Mathf.Clamp01(volume);
}
