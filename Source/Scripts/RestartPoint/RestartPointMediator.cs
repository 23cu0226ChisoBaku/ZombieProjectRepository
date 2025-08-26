public interface IMediator<Sender, Message> where Sender : class 
                                           where Message : class
{
  void Notify(Sender sender,Message message);
}

public class RestartPointMediator : IMediator<IRestartPoint, ISpawnable>
{
  private RespawnManager _respawnManager;
  private IRestartPoint _activeRestartPoint;              

  public RestartPointMediator(RespawnManager respawnManager)
  {
    _respawnManager = respawnManager;
  }

  public void Notify(IRestartPoint sender, ISpawnable message)
  {
    if (_activeRestartPoint.IsValid() && !_activeRestartPoint.Equals(sender))
    {
      _activeRestartPoint.OnInactive();
    }

    _activeRestartPoint = sender;
    _activeRestartPoint.OnActive();

    _respawnManager?.UpdateSpawnPos(message, _activeRestartPoint.RestartPos);  
  }
}
