using UnityEngine;

namespace MStateMachine
{
  /// <summary>
  /// ステートインスタンス
  /// </summary>
  /// <typeparam name="EState">ステートタイプ列挙</typeparam>
  public class State<EState> where EState : System.Enum
  {
    public State(EState key)
    {
      StateKey = key;
    }

    public EState StateKey { get; private set; }
    public virtual void EnterState() {}
    public virtual void ExitState() {}
    public virtual void UpdateState() {}
    public virtual void FixedUpdateState() {}
    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay2D(Collision2D collision) { }
    public virtual void OnCollisionExit2D(Collision2D collision) { }
    public virtual void OnTriggerEnter2D(Collider2D collision) { }
    public virtual void OnTriggerStay2D(Collider2D collision) { }
    public virtual void OnTriggerExit2D(Collider2D collision) { }
  }
} // namespace MStateMachine
