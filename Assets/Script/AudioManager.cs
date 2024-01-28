using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class AudioManager : MonoBehaviour
{
    /* How to use

        Step 1: Put sounds under Asset/Resource/Audio
        Step 2: Play sounds by calling AudioManager._instance.PlayOneShot("NAME OF BGM")
        PS: If your sound resides in a folder under Audio, prepend the folder name too.
        Eg. AudioManager._instance.PlayOneShot("BGM/my_awesome_bgm")

        Control volume by calling UpdateVolume_BGM(float vol) and UpdateVolume_SFX(float vol)
        respectively.
    */

    public static AudioManager _instance;
    AudioSource audioSource;
    AudioSource bgSource;

    public bool bgmPlayOnStart = true;
    public string startingBGM;

    [SerializeField]
    float _sfxVol = 1.0f;
    [SerializeField]
    float _bgmVol = 1.0f;

    void Awake()
    {
        if (_instance == null){

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
    
        } else {
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        GameObject bgSourceObj = new GameObject("Background Audio Manager");
        bgSourceObj.transform.parent = transform;
        bgSource = bgSourceObj.AddComponent<AudioSource>();

    }

    void Start()
    {
        if (bgmPlayOnStart)
        {
            PlayBGM(startingBGM);
        }
    }

    [Button]
    public static void UpdateVolumeFromInspector()
    {
        Debug.Log($"Updating Volumes via Inspector: BGM {_instance._bgmVol} , SFX {_instance._sfxVol}");
        UpdateVolume_BGM(_instance._bgmVol);
        UpdateVolume_SFX(_instance._sfxVol);
    }

    public static AudioClip GetAudioClip(string audioString)
    {
        var clip = Resources.Load<AudioClip>("Sounds/" + audioString);
        if (clip == null)
        {
            Debug.LogWarning($"Cannot find {audioString}, make sure its under Audio folder");
        }
        return Resources.Load<AudioClip>("Sounds/" + audioString);

    }

    public static void PlayBGM(string audioString, float volume = 1, float pitch = 1)
    {
        _instance.bgSource.clip = GetAudioClip(audioString);
        _instance.bgSource.volume = volume * _instance._bgmVol;
        _instance.bgSource.loop = true;
        _instance.bgSource.Play();
    }

    public static void PlayOneShot(string audioString, float volume = 1, float pitch = 1)
    {
        _instance.audioSource.pitch = pitch;
        _instance.audioSource.PlayOneShot(GetAudioClip(audioString), volume * _instance._sfxVol);
    }

    public static void UpdateVolume_BGM(float newVol)
    {
        _instance._bgmVol = newVol;
        _instance.bgSource.volume = newVol;
    }

    public static void UpdateVolume_SFX(float newVol)
    {
        _instance._sfxVol = newVol;
        _instance.audioSource.volume = newVol;
    }

    void Update()
    {

    }
}