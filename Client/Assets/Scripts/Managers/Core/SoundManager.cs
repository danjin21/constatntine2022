using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    // 음원들 넣는 곳
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // MP3 Player   -> AudioSource
    // MP3 음원     -> AudioClip
    // 관객(귀)     -> AudioListener

    SoundSupport SoundSupport;

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            // 이름을 추출
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));


            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                SoundSupport = go.AddComponent<SoundSupport>();
                go.transform.parent = root.transform;

                
                if( i ==0) // bgm
                           //_audioSources[i].volume = 0.05f;
                    _audioSources[i].volume = 0.00f;
                else if( i==1) // effect
                    _audioSources[i].volume = 0.5f;

            }

            // BGM 인 경우 loop를 튼다.
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    // 계속 들고있으면 메모리 터지므로 가끔씩 지워줘야 한다.
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();

    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

	public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
	{
        if (audioClip == null)
            return;

		if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            // 이미 다른 곡을 틀고 있다면 멈춰준다.
            if (audioSource.isPlaying)
            {
                if (audioSource.clip != audioClip)
                    audioSource.Stop();
                else
                    return;
            }

			audioSource.pitch = pitch;
			audioSource.clip = audioClip;
			audioSource.Play();
		}
		else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];

            //audioSource.pitch = pitch;
			//audioSource.PlayOneShot(audioClip);

            SoundSupport.PlayOneShotSound(audioSource, audioClip, 0.5f);

        }
	}

    // 캐싱역할
    // 기존에 없는거면 넣어주고, 기존에 있는거면 기존꺼를 쓴다.
	AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
            // Path를 통해 딕셔너리에서 이 클립이 있는지 봐준다.
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }







}
