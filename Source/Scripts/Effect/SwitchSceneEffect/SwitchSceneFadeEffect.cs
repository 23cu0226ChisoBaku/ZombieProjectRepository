using System;
using System.Collections;
using MEffect;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class SwitchSceneFadeEffect : MonoBehaviour
{
  private string _nextSceneName;
  private FadeInOutEffect _effect;
  private FadeInOutModel _fadeOutModel;
  private FadeInOutModel _fadeInModel;
  private Vector3 _fadeInPos;
  private Vector3 _fadeOutPos;
  private bool _isFadeAllOver = false;
  public bool IsFadeAllOver => _isFadeAllOver;

  public event Action onFadeAllOver;
  private void Awake()
  {
    _effect = gameObject.AddComponent<FadeInOutEffect>();
    SceneManager.sceneLoaded += AfterSceneLoad;
    DontDestroyOnLoad(gameObject);
  }

  private void Start()
  {
    StartCoroutine(FadeOut());
  }

  private void Update()
  {
    if(_isFadeAllOver)
    {
      onFadeAllOver?.Invoke();
      Destroy(gameObject);
    }
  }
  public void SetFadeModels(string nextScene, FadeInOutModel outModel, FadeInOutModel inModel, Vector2 fadeOutPos = default, Vector2 fadeInPos = default)
  {
    _nextSceneName = nextScene;
    _fadeOutModel = outModel;
    _fadeInModel = inModel;
    _fadeOutPos = fadeOutPos;
    _fadeInPos = fadeInPos;
  }

  private IEnumerator FadeOut()
  {
    if(_effect == null)
    {
      _isFadeAllOver = true;
      SceneManager.sceneLoaded -= AfterSceneLoad;
      yield break;
    }

    _effect.SetFadeProperties(_fadeOutModel.FadeEffect,_fadeOutModel.FadeTime, _fadeOutPos, _fadeOutModel.FadeColor);

    _effect.transform.position = Camera.main.transform.position - new Vector3(0f, 0f, Camera.main.transform.position.z);
    float height = Camera.main.orthographicSize * 2f;
    _effect.transform.localScale = new Vector3(height * Camera.main.aspect, height, 1f);

    while (!_effect.IsFadeOver)
    {
      yield return null;
    }
    yield return SceneManager.LoadSceneAsync(_nextSceneName);

    yield break;
  }

  private IEnumerator FadeIn()
  {

    _effect.SetFadeProperties(_fadeInModel.FadeEffect,_fadeInModel.FadeTime, _fadeInPos, _fadeInModel.FadeColor);
    gameObject.transform.position = Camera.main.transform.position - new Vector3(0f, 0f, Camera.main.transform.position.z);
    float height = Camera.main.orthographicSize * 2f;
    gameObject.transform.localScale = new Vector3(height * Camera.main.aspect, height, 1f);

    while (!_effect.IsFadeOver)
    {
      yield return null;
    }

    _isFadeAllOver = true;
    SceneManager.sceneLoaded -= AfterSceneLoad;

    yield break;
  }

  private void AfterSceneLoad(Scene scene, LoadSceneMode mode)
  {
    if(scene.name.Equals(_nextSceneName))
    {
      var player = FindAnyObjectByType<PlayerController>();
      if(player != null)
      {
          _fadeInPos = player.transform.position;
      }
      
      StartCoroutine(FadeIn());
    }
  }

  private void OnDestroy()
  {
    onFadeAllOver = null;
  }
}