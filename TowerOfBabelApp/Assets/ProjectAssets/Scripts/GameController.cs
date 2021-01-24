using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _objectToConcentrate;
    [SerializeField] private Image _platformProgress;
    [SerializeField] private GameObject[] _birds;

    private List<GameObject> _platforms;
    
    private Vector3 _spawnPosition;
    private Vector3 _cameraPosition;
    private float _platformHeight;

    private float _maxPlatformScale;
    private float _minPlatformScale;

    private int _countOfFloors;

    public float MaxPlatformScale 
    {
        get
        {
            return _maxPlatformScale;
        }
        private set
        {
            _maxPlatformScale = value;
        }
    }

    public float MinPlatformScale 
    {
        get
        {
            return _minPlatformScale;
        }
        private set
        {
            _minPlatformScale = value;
        }
    }

    private void Start()
    {
        _platforms = new List<GameObject>();
        _countOfFloors = 0;
        _objectToConcentrate.transform.position = new Vector3(250, 0.2f, 263);
        _spawnPosition = new Vector3(250, 0.2f, 263);
        var position = _cinemachineVirtualCamera
            .GetCinemachineComponent<CinemachineTransposer>()
            .m_FollowOffset;
        _cameraPosition = new Vector3(position.x, position.y, position.z);
        SetDefaultPlatformSizes();
        SetDefaultSoundLevels();
        _platformHeight = 0.4f;
        var size = _floor.transform.localScale;
        size = new Vector3(_minPlatformScale, 0.2f, _minPlatformScale);
        _floor.transform.localScale = size;
        EnableBirds();
    }

    private void SetDefaultSoundLevels()
    {
        if (!PlayerPrefs.HasKey(ApplicationVariables.MUSIC_LEVEL))
        {
            PlayerPrefs.SetFloat(ApplicationVariables.MUSIC_LEVEL, 1);
        }

        if (!PlayerPrefs.HasKey(ApplicationVariables.SOUND_LEVEL))
        {
            PlayerPrefs.SetFloat(ApplicationVariables.SOUND_LEVEL, 1);
        }
        
        foreach (var source in GameObject.FindGameObjectsWithTag("Music"))
        {
            source.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(ApplicationVariables.MUSIC_LEVEL);
        }

        foreach (var source in GameObject.FindGameObjectsWithTag("Sound"))
        {
            source.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(ApplicationVariables.SOUND_LEVEL);
        }
    }

    private void SetDefaultPlatformSizes()
    {
        if (!PlayerPrefs.HasKey(ApplicationVariables.MINIMAL_PLATFORM_SCALE))
        {
            PlayerPrefs.SetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE, 1);
        }

        if (!PlayerPrefs.HasKey(ApplicationVariables.MAXIMUM_PLATFORM_SCALE))
        {
            PlayerPrefs.SetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE, 10);
        }

        _minPlatformScale = PlayerPrefs.GetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE);
        _maxPlatformScale = PlayerPrefs.GetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE);
    }

    private void EnableBirds()
    {
        var delay = 1f;
        
        foreach (var bird in _birds)
        {
            StartCoroutine(WaitForBird(delay, bird));
            delay += 1f;
        }
    }

    private IEnumerator WaitForBird(float seconds, GameObject bird)
    {
        yield return new WaitForSeconds(seconds);
        bird.SetActive(true);
    }

    private void LiftTheCamera()
    {
        var position = _objectToConcentrate.transform.position;
        position = new Vector3(position.x, position.y + _platformHeight, position.z);
        _objectToConcentrate.transform.position = position;
    }

    public void ZoomCamera(float zoom)
    {
        _cinemachineVirtualCamera
            .GetCinemachineComponent<CinemachineTransposer>()
            .m_FollowOffset = new Vector3(_cameraPosition.x + (zoom / 2), _cameraPosition.y,_cameraPosition.z - (zoom / 2));
    }

    public void SetPlatformProgress(float progress)
    {
        _platformProgress.fillAmount = progress;

        if (_platformProgress.fillAmount >= 1)
        {
            EndGame();
        }
    }

    public void SpawnNewPlatform(float length)
    {
        _cinemachineVirtualCamera
            .GetCinemachineComponent<CinemachineTransposer>()
            .m_FollowOffset = _cameraPosition;
        
        _countOfFloors++;
        FindObjectOfType<UiController>().UpdateScore(_countOfFloors);
        _maxPlatformScale = length;
        LiftTheCamera();
        _platforms.Add(Instantiate(_floor, _spawnPosition, Quaternion.identity));
        _spawnPosition = new Vector3(_spawnPosition.x, _spawnPosition.y + _platformHeight, _spawnPosition.z);
    }

    public void StartTheSession()
    {
        _minPlatformScale = PlayerPrefs.GetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE);
        _maxPlatformScale = PlayerPrefs.GetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE);
        var size = _floor.transform.localScale;
        _floor.transform.localScale = new Vector3(_minPlatformScale, size.y, _minPlatformScale);
        
        SpawnNewPlatform(_maxPlatformScale);
    }

    public void ClearTheTower()
    {
        foreach (var platform in _platforms)
        {
            Destroy(platform);
        }
        
        ResetProgress();
    }

    private void ResetProgress()
    {
        _countOfFloors = 0;
        _platformProgress.fillAmount = 0;
        _spawnPosition = new Vector3(250, 0.2f, 263);
        _objectToConcentrate.transform.position = _spawnPosition;
    }

    private void EndGame()
    {
        var bestScore = PlayerPrefs.GetInt(ApplicationVariables.BEST_SCORE);

        if (_countOfFloors > bestScore)
        {
            PlayerPrefs.SetInt(ApplicationVariables.BEST_SCORE, _countOfFloors);
        }

        FindObjectOfType<UiController>().EndGame(_countOfFloors);
        ResetProgress();

        _minPlatformScale = PlayerPrefs.GetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE);
        _maxPlatformScale = PlayerPrefs.GetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE);
    }
}
