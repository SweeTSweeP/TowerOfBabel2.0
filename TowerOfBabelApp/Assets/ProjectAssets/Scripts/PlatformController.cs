using System;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    private GameController _gameController;

    private bool _isMouseButtonHold;
    private bool _isDisable;
    private float _currentTime;
    private float _limitScaleValue;

    private Vector3 _startScale;
    private Vector3 _finalScale;

    private void Start()
    {
        _isMouseButtonHold = false;
        _isDisable = false;
        _currentTime = 0f;
        _gameController = FindObjectOfType<GameController>();
        _limitScaleValue = _gameController.MaxPlatformScale;
    }

    private void Update()
    {
        if (!_isMouseButtonHold || 
            transform.localScale.x > _limitScaleValue || 
            transform.localScale.z > _limitScaleValue || 
            _isDisable) return;
        
        _currentTime += Time.deltaTime;
        transform.localScale = Vector3.Lerp(_startScale, _finalScale, _currentTime / (_limitScaleValue -_gameController.MinPlatformScale));

        if (transform.localScale.x / _limitScaleValue == 1)
        {
            _isDisable = true;
            _audioSource.Stop();
        }

        if (transform.localScale.x > 10)
        {
            _gameController.ZoomCamera(transform.localScale.x - 10);
        }

        //_gameController.SetPlatformProgress(transform.localScale.x / _limitScaleValue);
        var progress = 1 - (_limitScaleValue - transform.localScale.x) / (_limitScaleValue - _gameController.MinPlatformScale);
        _gameController.SetPlatformProgress(progress);
    }

    private void OnMouseDown()
    {
        if (_isDisable) return;

        _audioSource.Play();
        _isMouseButtonHold = true;
        _limitScaleValue = _gameController.MaxPlatformScale;
        _startScale = transform.localScale;
        _finalScale = new Vector3(_limitScaleValue, _startScale.y, _limitScaleValue);
    }

    private void OnMouseUp()
    {
        if (_isDisable) return;
        
        _audioSource.Stop();
        _isMouseButtonHold = false;
        _isDisable = true;
        if (transform.localScale.x - _gameController.MinPlatformScale < 0.05)
        {
            _gameController.SetPlatformProgress(1);
        }
        else
        {
            _gameController.SetPlatformProgress(0);   
        }
        _gameController.SpawnNewPlatform(transform.localScale.x);
        gameObject.GetComponent<PlatformController>().enabled = false;
    }
}
