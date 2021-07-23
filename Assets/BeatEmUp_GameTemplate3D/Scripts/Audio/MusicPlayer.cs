using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] clips;
    
    private AudioSource _source;

    private float currentClipLength;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        if (!_source) _source = gameObject.AddComponent<AudioSource>();

        _source.loop = false;

        StopAllCoroutines();
        StartCoroutine(PlayRandomRepeatly());
    }

    IEnumerator PlayRandomRepeatly()
    {
        if (clips!=null && clips.Length>0)
        while (true)
        {
            _source.clip = clips[Random.Range(0, clips.Length)];
            _source.Play();
            
            yield return new WaitForSeconds(_source.clip.length);
        }
    }
}