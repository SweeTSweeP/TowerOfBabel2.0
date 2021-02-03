using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;

    // Update is called once per frame
    //private void Update()
    //{
    //    if(_audioSource.isPlaying) return;

    //    var randomAudioIndex = UnityEngine.Random.Range(0, _audioClips.Length - 1);
    //    _audioSource.clip = _audioClips[randomAudioIndex];
    //    _audioSource.Play();
    //}

    private void Start()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        while(true)
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            var delay = Random.Range(.5f, 5f);
            yield return new WaitForSeconds(delay);

            var randomAudioIndex = UnityEngine.Random.Range(0, _audioClips.Length - 1);
            _audioSource.clip = _audioClips[randomAudioIndex];
            _audioSource.Play();
        }
    }
}
