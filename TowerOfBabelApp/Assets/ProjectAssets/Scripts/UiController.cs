using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private GameObject _resultDialog;
    [SerializeField] private GameObject _playScreen;
    [SerializeField] private GameObject _subMenuPlayScreen;

    [SerializeField] private TMP_Text _minPlatfromScaleValue;
    [SerializeField] private TMP_Text _maxPlatfromScaleValue;
    [SerializeField] private TMP_Text _highestTower;
    [SerializeField] private TMP_Text _gamesCount;

    [SerializeField] private Slider _musicLevelSlider;
    [SerializeField] private Slider _soundLevelSlider;
    [SerializeField] private Slider _minPlatformScaleSlider;
    [SerializeField] private Slider _maxPlatformScaleSlider;

    [SerializeField] private CinemachineVirtualCamera _camera;

    [SerializeField] private Sprite _activeMusicIcon;
    [SerializeField] private Sprite _activeSoundIcon;
    [SerializeField] private Sprite _disableMusicIcon;
    [SerializeField] private Sprite _disableSoundIcon;

    private float _musicLevel;
    private float _soundLevel;
    private float _minPlatformSize;
    private float _maxPlatformSize;

    private void Start()
    {
        SetStatistic();
    }

    private void SetStatistic()
    {
        var highestTower = PlayerPrefs.GetInt(ApplicationVariables.BEST_SCORE);
        _highestTower.text = $"Highest tower: {highestTower}";

        if (!PlayerPrefs.HasKey(ApplicationVariables.GAMES_COUNT))
        {
            PlayerPrefs.SetInt(ApplicationVariables.GAMES_COUNT, 0);
        }

        var gamesCount = PlayerPrefs.GetInt(ApplicationVariables.GAMES_COUNT);
        _gamesCount.text = $"Games Count: {gamesCount}";
    }

    public void PressPlayButton(GameObject menu)
    {
        _camera.gameObject.SetActive(true);
        _audioSource.Play();
        menu.SetActive(false);
        _playScreen.SetActive(true);
        _gameController.StartTheSession();
    }

    public void PressExitButton()
    {
        _audioSource.Play();
        Application.Quit();
    }

    public void EndGame(int sessionScore)
    {
        _resultDialog.SetActive(true);

        _resultDialog
            .transform
            .Find("Dialog")
            .Find("FloorsText")
            .gameObject
            .GetComponent<TMP_Text>()
            .text = $"Floors count: {sessionScore}";

        _resultDialog
            .transform
            .Find("Dialog")
            .Find("BestResultText")
            .gameObject
            .GetComponent<TMP_Text>()
            .text = $"Best result: {PlayerPrefs.GetInt("bestScore")}";
        
        var gamesCount = PlayerPrefs.GetInt(ApplicationVariables.GAMES_COUNT);
        gamesCount++;
        PlayerPrefs.SetInt(ApplicationVariables.GAMES_COUNT, gamesCount);
        SetStatistic();
    }

    public void PressRestartButton()
    {
        _audioSource.Play();
        _camera.gameObject.SetActive(true);
        _resultDialog.SetActive(false);
    }

    public void ChangeMusicLevel(Slider slider)
    {
        var musicSources = GameObject.FindGameObjectsWithTag("Music");

        foreach (var source in musicSources)
        {
            source.GetComponent<AudioSource>().volume = slider.value;
        }

        _musicLevel = slider.value;
    }

    public void ChangeSoundLevel(Slider slider)
    {
        var soundSources = GameObject.FindGameObjectsWithTag("Sound");

        foreach (var source in soundSources)
        {
            source.GetComponent<AudioSource>().volume = slider.value;
        }

        _soundLevel = slider.value;
    }

    public void ChangeMinSizeOfPlatform(Slider slider)
    {
        _minPlatfromScaleValue.text = slider.value.ToString();

        _minPlatformSize = slider.value;
    }

    public void ChangeMaxSizeOfPlatform(Slider slider)
    {
        _maxPlatfromScaleValue.text = slider.value.ToString();

        _maxPlatformSize = slider.value;
    }

    public void PressBackButton(GameObject _settingsDialog)
    {
        _audioSource.Play();
        PlayerPrefs.SetFloat(ApplicationVariables.SOUND_LEVEL, _soundLevel);
        PlayerPrefs.SetFloat(ApplicationVariables.MUSIC_LEVEL, _musicLevel);
        PlayerPrefs.SetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE, _minPlatformSize);
        PlayerPrefs.SetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE, _maxPlatformSize);
        
        _settingsDialog.SetActive(false);
    }

    public void PressSettingsButton(GameObject _settingsDialog)
    {
        _audioSource.Play();
        _settingsDialog.SetActive(true);
        _soundLevelSlider.value = PlayerPrefs.GetFloat(ApplicationVariables.SOUND_LEVEL);
        _musicLevelSlider.value = PlayerPrefs.GetFloat(ApplicationVariables.MUSIC_LEVEL);
        _minPlatformScaleSlider.value = PlayerPrefs.GetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE);
        _maxPlatformScaleSlider.value = PlayerPrefs.GetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE);
        
        _soundLevel = PlayerPrefs.GetFloat(ApplicationVariables.SOUND_LEVEL);
        _musicLevel = PlayerPrefs.GetFloat(ApplicationVariables.MUSIC_LEVEL);
        _minPlatformSize = PlayerPrefs.GetFloat(ApplicationVariables.MINIMAL_PLATFORM_SCALE);
        _maxPlatformSize = PlayerPrefs.GetFloat(ApplicationVariables.MAXIMUM_PLATFORM_SCALE);
    }

    public void UpdateScore(int floorsCount)
    {
        _playScreen
            .transform
            .Find("FloorsText")
            .gameObject
            .GetComponent<TMP_Text>()
            .text = $"Floors: {floorsCount}";
    }

    public void PressExitButton(GameObject menu)
    {
        _audioSource.Play();
        _camera.gameObject.SetActive(false);
        _gameController.ClearTheTower();
        _resultDialog.SetActive(false);
        _playScreen.SetActive(false);
        menu.SetActive(true);
    }

    public void SwitchMusic(Image musicImage)
    {
        _audioSource.Play();
        var musicLevel = PlayerPrefs.GetFloat(ApplicationVariables.MUSIC_LEVEL);

        if (musicLevel > 0)
        {
            musicImage.sprite = _disableMusicIcon;
            PlayerPrefs.SetFloat(ApplicationVariables.MUSIC_LEVEL, 0);
            _musicLevel = 0;
        }
        else
        {
            musicImage.sprite = _activeMusicIcon;
            PlayerPrefs.SetFloat(ApplicationVariables.MUSIC_LEVEL, 1);
            _musicLevel = 1;
        }
        
        var musicSources = GameObject.FindGameObjectsWithTag("Music");

        foreach (var source in musicSources)
        {
            source.GetComponent<AudioSource>().volume = _musicLevel;
        }
    }

    public void SwitchSound(Image soundImage)
    {
        var soundLevel = PlayerPrefs.GetFloat(ApplicationVariables.SOUND_LEVEL);

        if (soundLevel > 0)
        {
            soundImage.sprite = _disableSoundIcon;
            PlayerPrefs.SetFloat(ApplicationVariables.SOUND_LEVEL, 0);
            _soundLevel = 0;
        }
        else
        {
            soundImage.sprite = _activeSoundIcon;
            PlayerPrefs.SetFloat(ApplicationVariables.SOUND_LEVEL, 1);
            _soundLevel = 1;
        }
        
        var soundSources = GameObject.FindGameObjectsWithTag("Sound");

        foreach (var source in soundSources)
        {
            source.GetComponent<AudioSource>().volume = _soundLevel;
        }
        
        _audioSource.Play();
    }

    public void PressExitButtonFromSubMenu(GameObject menu)
    {
        UpdateScore(0);
        _audioSource.Play();
        _camera.gameObject.SetActive(false);
        _subMenuPlayScreen.SetActive(false);
        _playScreen.SetActive(false);
        menu.SetActive(true);
    }

    public void PressRestartButtonFromSubMenu()
    {
        UpdateScore(0);
        _audioSource.Play();
        _camera.gameObject.SetActive(true);
        _subMenuPlayScreen.SetActive(false);
    }

    public void PressResumeButtonFromSubMenu()
    {
        _audioSource.Play();
        _camera.gameObject.SetActive(true);
        _subMenuPlayScreen.SetActive(false);
    }

    public void PresSubMenuButton()
    {
        _audioSource.Play();
        _camera.gameObject.SetActive(false);
        _subMenuPlayScreen.SetActive(true);
    }
}
