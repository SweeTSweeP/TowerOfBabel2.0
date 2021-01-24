using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;

    private int _currentAudioIndex;

    private void Start()
    {
        _currentAudioIndex = 0;
        _audioSource.clip = _audioClips[_currentAudioIndex];
        _audioSource.Play();
    }

    private void Update()
    {
        if (_audioSource.isPlaying)  return;

        _audioSource.Stop();
        NextTrack();
    }

    private void NextTrack()
    {
        if (_currentAudioIndex < _audioClips.Length - 1)
        {
            _currentAudioIndex++;
        }
        else
        {
            _currentAudioIndex = 0;
        }

        _audioSource.clip = _audioClips[_currentAudioIndex];
        _audioSource.Play();
    }
}
