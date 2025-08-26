using UnityEngine;

public class PlayerDeadState: PlayerActionState  
{
  private readonly float _deadTimeInterval;
  private float _deadTimeCnt;

  public PlayerDeadState(                         PlayerActionContext context,
                          PlayerActionStateMachine.EPlayerActionState stateKey,
                                                                float deadTime
                          ) 
      : base(context, stateKey)
  {
    _deadTimeInterval = deadTime;
    _deadTimeCnt = 0f;
  }

  public override void EnterState()
  {
    _deadTimeCnt = 0f;
    _context.PlayerRigidbody.velocity = Vector2.zero;
    AudioManager.Instance.PlaySE("Zombie_Dead");
    _context.PlayerAnimator.Play("Dead");
    _context.PlayerCtrl.OnDead();
  }

  public override void UpdateState()
  {
    _deadTimeCnt += Time.deltaTime;
    if(_deadTimeCnt >= _deadTimeInterval)
    {
      // プレイヤーの復活メッセージを送る
      SpawnEvent spawnEvent = new SpawnEvent
      {
        SpawnTarget = _context.PlayerGameObject
      };
      TypeBasedEventManager.Instance.Send(spawnEvent);
    }
  }
}
