using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public bool isInit = false;
    public int maxAudios = 10;
    int playIdx;

    AudioSource[] audios;
    AudioSource bgmAudio;
    AudioSource chatAudio;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("이미 사운드 매니저가 존재합니다.");
            Destroy(this.gameObject);
        }
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        if (!instance.isInit)
        {
            audios = new AudioSource[maxAudios];

            for (int i = 0; i < maxAudios; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.parent = this.transform;
                audios[i] = obj.AddComponent<AudioSource>();
                audios[i].playOnAwake = false;
            }

            //BGM Player
            GameObject bgmobj = new GameObject();
            bgmobj.name = "BGMPlayer";
            bgmobj.transform.parent = this.transform;
            bgmAudio = bgmobj.AddComponent<AudioSource>();
            bgmAudio.playOnAwake = false;
            bgmAudio.loop = true;

            //ChatPlayer
            GameObject chatobj = new GameObject();
            chatobj.name = "ChatPlayer";
            chatobj.transform.parent = this.transform;
            chatAudio = chatobj.AddComponent<AudioSource>();
            chatAudio.loop = true;

            instance.isInit = true;

        }
        else
            Debug.Log("이미 사운드 매니저 초기화가 완료 되었습니다.");
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {

    }
    
    public void PlayBgm(AudioClip clip, float volume = 1.0f)
    {
        bgmAudio.clip = clip;
        bgmAudio.volume = volume;
        bgmAudio.Play();
    }

    public void PlayEffSound(string SoundName, float volume = 1.0f)
    {
        audios[playIdx].clip = Resources.Load(SoundName) as AudioClip;
        audios[playIdx].volume = volume;
        audios[playIdx].Play();
        playIdx++;
        if (playIdx >= maxAudios)
            playIdx = 0;
    }

    public void PlayEffSound(string[] SoundName, float volume = 1.0f)
    {
        int rand = Random.Range(0, SoundName.Length);
        audios[playIdx].clip = Resources.Load(SoundName[rand]) as AudioClip;
        audios[playIdx].volume = volume;
        audios[playIdx].Play();
        playIdx++;
        if (playIdx >= maxAudios)
            playIdx = 0;
    }

    public void PlayEffSound(AudioClip clip, float volume = 1.0f)
    {
        audios[playIdx].clip = clip;
        audios[playIdx].volume = volume;
        audios[playIdx].Play();
        playIdx++;
        if (playIdx >= maxAudios)
            playIdx = 0;
    }

    public void StopBeforeSound()
    {
        if (playIdx == 0)
            audios[audios.Length - 1].Stop();

        audios[playIdx - 1].Stop();
    }
}