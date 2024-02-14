using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 프로젝트의 모든 영역에서 사용되는 사운드를 관리하는 매니저.
/// </summary>
public class SoundManager
{
    /// <summary>
    /// 모든 영역에서 사용되는 AudioSource 컴포넌트들을 담는 배열.
    /// 여기에 담기는 AudioSource는 각각 배경음악, 효과음을 담당한다.
    /// </summary>
    private AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    
    /// <summary>
    /// AudioClip Caching을 담당하는 Dictionary.
    /// Play함수를 통해 AudioClip을 재생할 때, 이 딕셔너리 안에 있는 AudioClip이라면 여기서 가져와 재생한다.
    /// </summary>
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    
    // [사운드를 들을 때 필요한 것]
    // 소리의 근원지 (음향 송신) : AudioSource
    // 소리                   : AudioClip
    // 귀 (음향 수신)          : AudioListener

    /// <summary>
    /// Sound 시스템 초기화.
    /// @Sound라는 이름의 GameObject를 찾거나 생성하고,
    /// 각 사운드 유형에 해당하는 AudioSource 컴포넌트를 이 GameObject의 자식으로 추가합니다.
    /// 배경음악을 담당하는 AudioSource 컴포넌트는 loop를 true로 설정합니다.
    /// </summary>
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)     // Define.Sound.MaxCount는 제외
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;       // BGM은 Loop : true
        }
    }
    
    /// <summary>
    /// string 버전의 오디오 플레이 함수. 
    /// </summary>
    /// <param name="path">음원 파일 경로</param>
    /// <param name="type">Clip의 종류</param>
    /// <param name="pitch">재생 속도</param>
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        var audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type,pitch);
    }

    /// <summary>
    /// AudioClip 버전의 오디오 플레이 함수. 
    /// </summary>
    /// <param name="audioClip">AudioClip</param>
    /// <param name="type">Clip의 종류</param>
    /// <param name="pitch">재생 속도</param>
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;
        
        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            
            if(audioSource.isPlaying)
                audioSource.Stop();
            
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    /// <summary>
    ///  AudioClip을 불러올 때 활용. 조건부 AudioClip Caching 함수.
    /// </summary>
    /// <param name="path">AudioClip 파일 경로 (/Resources부터)</param>
    /// <returns>AudioClip</returns>
    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";
        
        AudioClip audioClip = null;
        
        if (type == Define.Sound.Bgm)       // BGM일 때
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else                                // Effect일 때
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)      // 조건부  Caching
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path,audioClip);
            }
        }
        
        if (audioClip == null)
            Debug.Log($"AudioClip is Missing! {path}");

        return audioClip;
    }

    /// <summary>
    /// AudioClip Cache Clear 함수.
    /// 모든 AudioSource의 Clip을 제거하고 재생을 중지.
    /// </summary>
    public void Clear()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }
}
