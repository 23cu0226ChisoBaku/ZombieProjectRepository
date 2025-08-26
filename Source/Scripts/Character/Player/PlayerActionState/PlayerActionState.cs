using MStateMachine;

public abstract class PlayerActionState : State<PlayerActionStateMachine.EPlayerActionState>
{
    protected PlayerActionContext _context;  

    public PlayerActionState(                            PlayerActionContext context,
                                PlayerActionStateMachine.EPlayerActionState stateKey
                            )                                         :base(stateKey)
    {
      _context = context;
    }
}
