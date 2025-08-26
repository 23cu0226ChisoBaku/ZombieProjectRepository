using UnityEngine;

public interface IMediatable<Sender,Message> where Sender: class
                                             where Message: class
{
  void SetMediator(IMediator<Sender,Message> mediator) ;
}

public interface IRestartPoint : IMediatable<IRestartPoint, ISpawnable>
{
  bool IsActive {get;}
  Vector3 RestartPos {get;}
  void OnActive();
  void OnInactive();
}

[RequireComponent(typeof(BoxCollider2D))]
public class RestartPoint : MonoBehaviour, IRestartPoint
{
  Vector3 IRestartPoint.RestartPos
  {
    get => _restartPosition;
  }
  bool IRestartPoint.IsActive
  {
    get  => _isActive;
  }

  private bool _isActive;
  private Vector3 _restartPosition;
  private BoxCollider2D _collider2D;
  private IMediator<IRestartPoint, ISpawnable> _respawnMediator;   // 仲介者

  [SerializeField]
  private GameObject _onActiveEffectPrefabs;

  private void Awake()
  {
    _collider2D = GetComponent<BoxCollider2D>();
    _restartPosition = transform.position;
  }

  public void SetMediator(IMediator<IRestartPoint,ISpawnable> mediator)
  {
    _respawnMediator = mediator;
  }

  private void OnTriggerEnter2D(Collider2D other) 
  {
    // プレイヤーだけ取るようにする
    if (other.gameObject.TryGetComponent(out PlayerController dummy) && other.gameObject.TryGetComponent(out ISpawnable spawnable))
    {
      // オブジェクトの中心点からスプライトに合わせるようにずらす（最初の一回だけ）
      if (Mathf.Approximately((_restartPosition - transform.position).sqrMagnitude, 0f))
      {
        _restartPosition = new Vector3(transform.position.x - _collider2D.size.x * 0.1666666f,
                                  transform.position.y + (other.bounds.size.y - _collider2D.size.y * transform.localScale.y) * 0.5f,
                                  0f);
      }
      // 仲介者を通してリスポーンマネージャーとやり取りする。
      _respawnMediator?.Notify(this, spawnable);
    }
  }

  private void OnDestroy()
  {
    _isActive = false;
    if(_collider2D != null)
    {
      _collider2D.enabled = false;
    }
    _respawnMediator = null;
  }

  void IRestartPoint.OnActive()
  {
    _collider2D.enabled = false;
    _isActive = true;

    if (_onActiveEffectPrefabs != null)
    {
      Instantiate(_onActiveEffectPrefabs, _restartPosition, Quaternion.identity);
    }
  }

  void IRestartPoint.OnInactive()
  {
    _collider2D.enabled = true;
    _isActive = false;
  }
}
