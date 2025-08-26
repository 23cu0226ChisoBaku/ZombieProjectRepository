using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zombie.Global;

public struct StartOpeningTextEvent
{

}

public class TestTextUIController : MonoBehaviour
{
  private readonly static string GAME_SCENE_NAME = "MainGame";
  [SerializeField]
  private TextDisplayer _displayer;

  [SerializeField]
  private int _textUIMaxLineCnt;

  [SerializeField]
  private float _charSpacing;

  [SerializeField]
  private RectTransform _textUIViewPort;

  [SerializeField]
  private float _textUIFontSize;

  [SerializeField]
  private Color _fontColor;

  [SerializeField]
  [Range(0.1f,100f)]
  private float _textBoxMoveSpeed;

  [SerializeField]
  private Sprite[] _badDesignBackgroundSprites;

  [SerializeField]
  private Image _badDesignBackgroundImage;
  private int _badDesignImageIndex = 0;


  private List<TMP_Text> _textUIGroups = new List<TMP_Text>();
  private int _currentTextUIIndex;
  private float _updateTextUIPosTimeCnt = 0f;
  private StringBuilder _displayTextBuffer;
  private Coroutine _delayDisplayCoroutine = null;
  private Coroutine _updateTextUIPosCoroutine = null;
  private Coroutine _switchNextBGImageCoroutine = null;
  private int _displayTextLineCnt = 0;
  private bool _isTextUIUpdating = false;
  private bool _canSkip = false;
  private bool _isTextUIMoving = false;
  private bool _isWaitFading = true;


  private void Awake()
  {
    CheckValidate();

    _displayer.InitializeDisplayer();
    _displayTextBuffer = new StringBuilder();
    _isTextUIUpdating = true;
    _badDesignBackgroundImage.sprite = _badDesignBackgroundSprites[_badDesignImageIndex++ % _badDesignBackgroundSprites.Length];

    TypeBasedEventManager.Instance.RegisterEvent<StartOpeningTextEvent>
    (entity =>
    {
        _delayDisplayCoroutine ??= StartCoroutine(DisplayNextText());
        _isWaitFading = false;
    }
    ).UnregisterEventOnDestroy(gameObject);
  }

  // Start is called before the first frame update
  void Start()
  {
    var childs = GetComponentsInChildren<TMP_Text>();
    foreach (var child in childs)
    {
      if (_textUIMaxLineCnt > 1)
      {
        child.characterSpacing = _charSpacing;
        child.fontSize = _textUIFontSize;
        child.lineSpacing = _textUIViewPort.rect.height - _textUIMaxLineCnt * child.fontSize;
        child.color = _fontColor;
        child.alignment = TextAlignmentOptions.TopLeft;

        float linespacing = (_textUIViewPort.rect.height - _textUIMaxLineCnt * _textUIFontSize) / (float)(_textUIMaxLineCnt - 1) / _textUIFontSize * 100f;
        child.lineSpacing = linespacing;
      }

      _textUIGroups.Add(child);
    }

    if (_textUIGroups.Count > 0)
    {
      _currentTextUIIndex = (int)Mathf.Ceil((float)_textUIGroups.Count * 0.5f) - 1;
    }

    if(FindAnyObjectByType<SwitchSceneFadeEffect>() == null)
    {
        TypeBasedEventManager.Instance.Send<StartOpeningTextEvent>();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if(_isWaitFading)
    {
      return;
    }
    
    if (_displayer._reader.IsEnd && _displayer._loader.IsAllLoaded)
    {
      if(Input.anyKeyDown)
      {
        GameObject switchScenePrefab = Resources.Load<GameObject>(GlobalParam.SWITCH_SCENE_OBJ_PATH);
        GameObject switchSceneObj = Instantiate(switchScenePrefab,Vector3.zero,Quaternion.identity);

        SwitchSceneFadeEffect fadeCtrl = switchSceneObj.GetComponent<SwitchSceneFadeEffect>();
        FadeInOutModel Out = ModelManager.Instance.GetModel<FadeInOutModel>("SwitchSceneFadeOut");
        FadeInOutModel In = ModelManager.Instance.GetModel<FadeInOutModel>("SwitchSceneFadeIn");

        fadeCtrl.SetFadeModels(GAME_SCENE_NAME,Out,In,Vector3.zero);

        _isWaitFading = true;
      }
      return;
    }

    if (Input.anyKeyDown)
    {
      if (_displayer._reader.IsEnd)
      {
        if (!_displayer._loader.IsAllLoaded)
        {
          _displayer._reader.AssignNextText(_displayer._loader.GetNextText());

          _updateTextUIPosCoroutine ??= StartCoroutine(UpdateTextUIPos());
          _delayDisplayCoroutine ??= StartCoroutine(DisplayNextText());

          _switchNextBGImageCoroutine ??= StartCoroutine(SwitchNextBackgroundImage());
        }
      }
      else
      {
        if (_isTextUIUpdating)
        {
          _canSkip = true;
        }
        else
        {                        
          _updateTextUIPosCoroutine ??= StartCoroutine(UpdateTextUIPos());
        }
      }
    }
  }

  private IEnumerator DisplayNextText()
  {
    yield return new WaitUntil(() => !_isTextUIMoving);

    // TODO �S�~(GC Alloc)�̔�����h������
    var waitForNextRead = new WaitForSeconds(_displayer.DisplayNextInterval);

    _displayTextBuffer.Clear();

    while (!_displayer._reader.IsEnd)
    {
      string next = _displayer._reader.ReadNext();
      _displayTextBuffer.Append(next);

      if(next.Equals("\n"))
      {
        ++_displayTextLineCnt;
      }

      _textUIGroups[_currentTextUIIndex].SetText(_displayTextBuffer.ToString());

      if ((_displayTextLineCnt > 0) && (_displayTextLineCnt % _textUIMaxLineCnt == 0))
      {
          _isTextUIUpdating = false;
        _canSkip = false;
        yield return new WaitUntil(() => _isTextUIUpdating);

        _displayTextLineCnt = 0;
        _displayTextBuffer.Clear();
      }
          
      if (_canSkip)
      {
        continue;
      }

      yield return waitForNextRead;
    }

    _displayTextLineCnt = 0;
    _isTextUIUpdating = false;
    _canSkip = false;
    _delayDisplayCoroutine = null;

    yield break;
  }

  private IEnumerator UpdateTextUIPos()
  {
    _isTextUIMoving = true;
    float textBoxMoveTime = 1f / _textBoxMoveSpeed; 
    float textUIMoveDistance = _textUIViewPort.rect.height;
    List<Vector3> TMPTargetPos = new List<Vector3>();

    foreach (var t in _textUIGroups)
    {
        TMPTargetPos.Add(t.rectTransform.localPosition + Vector3.up * textUIMoveDistance);
    }

    while (_updateTextUIPosTimeCnt < textBoxMoveTime)
    {
        _updateTextUIPosTimeCnt += Time.deltaTime;
        for(int i = 0; i < _textUIGroups.Count; ++i)
        {
            var tmp = _textUIGroups[i];
            tmp.rectTransform.localPosition = tmp.rectTransform.localPosition + Vector3.up * textUIMoveDistance * Time.deltaTime * _textBoxMoveSpeed;
            
        }
        yield return null;
    }

    for (int i=0; i<_textUIGroups.Count; ++i)
    {
        _textUIGroups[i].rectTransform.localPosition = TMPTargetPos[i];
    }

    _updateTextUIPosTimeCnt = 0f;
    _displayTextLineCnt = 0;
    _isTextUIUpdating = true;
    _isTextUIMoving = false;

    _textUIGroups[_currentTextUIIndex].text = string.Empty;
    _currentTextUIIndex = (_currentTextUIIndex + 1) % _textUIGroups.Count;
    _textUIGroups[(_currentTextUIIndex + 1) % _textUIGroups.Count].rectTransform.localPosition += Vector3.down * textUIMoveDistance * _textUIGroups.Count;

    _updateTextUIPosCoroutine = null;

    yield break;

  }

  private void CheckValidate()
  {
    Assert.IsFalse(_textUIMaxLineCnt < 1);
    Assert.IsFalse(_badDesignBackgroundSprites.Length < 1);
  }
  
  private IEnumerator SwitchNextBackgroundImage()
  {
    yield return new WaitUntil(() => !_isTextUIMoving);

    if(_badDesignBackgroundSprites.Length > 0)
    {
      _badDesignBackgroundImage.sprite = _badDesignBackgroundSprites[(_badDesignImageIndex++) % _badDesignBackgroundSprites.Length];
    }

    _switchNextBGImageCoroutine = null;

    yield break;
  }

  private void OnDestroy() 
  {
    _displayer.Dispose();
  }
    
}
