using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using MLibrary;


public class GameManager : SingletonMono<GameManager>
{
  private class Private
  {
    public static readonly string MAIN_GAME_SCENE_NAME = "MainGame";
  }
  private EnergyUIController _energyUIController;

  [SerializeField]
  private IEffect _postProcessEffect;

  private GameObject _energyTransferPrefab;

  private RespawnManager _respawnManager;

  private void Start()
  {
    Application.targetFrameRate = 60;
    Physics2D.queriesStartInColliders = false;
    _energyTransferPrefab = Resources.Load("TestResource/TestEnergyTransformEffect") as GameObject;

    RegisterEnergyEvent();

    EnergyProcessorSettingRepository.Instance.Initialize();
    EnergyReactionAllocator.Instance.Initialize();

    Application.quitting += TermGameManager;
  }

  private void Update()
  {
    _respawnManager?.Update(Time.deltaTime);
  }

  private void SendSpawnMessage(GameObject obj)
  {
    if(obj.TryGetComponent(out ISpawnable spawnable))
    {
      _respawnManager?.SpawnObj(spawnable);
    }
  }

  private void RegisterSpawnableObj(GameObject obj)
  {
    if(obj.TryGetComponent(out ISpawnable spawnable))
    {
      _respawnManager?.RegisterSpawnableObj(spawnable,obj.transform.position);
    }
    else
    {
      #if UNITY_EDITOR
      Debug.LogWarning($"Can't register {obj.name} because it is not implemented ISpawnable");
      #endif
    }
  }

  private void OnEnable() 
  {   
    SceneManager.sceneLoaded += OnGameSceneLoaded;
    SceneManager.activeSceneChanged += OnActiveGameSceneChange;
  }

  private void OnDisable()
  {
    SceneManager.sceneLoaded -= OnGameSceneLoaded;
    SceneManager.activeSceneChanged -= OnActiveGameSceneChange;
  }

  private void TermGameManager()
  {     
    EnergyReactionAllocator.Instance.Terminate();
    EnergyProcessorSettingRepository.Instance.Deinitialize();

    _postProcessEffect?.Dispose();
    _respawnManager?.Dispose();
  }

  private void RegisterEnergyEvent()
  {
    TypeBasedEventManager.Instance.RegisterEvent<EnergyStockUpdateEvent>
        (entity =>
        {
          _energyUIController.UpdateIcon(entity.Energies);
        }
        ).UnregisterEventOnDestroy(gameObject);

    TypeBasedEventManager.Instance.RegisterEvent<EnergySelectingEvent>
        (entity =>
        {
          switch (entity.selection)
          {
            case ESelection.Next:
              _energyUIController.SelectNext();
              break;
            case ESelection.Previous:
              _energyUIController.SelectPrevious();
              break;
          }
        }
        ).UnregisterEventOnDestroy(gameObject);

    
    TypeBasedEventManager.Instance.RegisterEvent<EnergyTransferEvent>
    (entity =>
    {
      if (entity.Source != entity.Target)
      {
        if (!entity.Source.EnergyContainer.Processor.IsEnergyStockEmpty())
        {
          GameObject energyTransferObj = Instantiate(_energyTransferPrefab, entity.Source.EnergyContainer.EnergyReactionCTX.UserObject.transform.position, Quaternion.identity);
          energyTransferObj.transform.SetParent(entity.Source.EnergyContainer.EnergyReactionCTX.UserObject.transform);

          EnergyTransformEffect energyTransformEffect = energyTransferObj.GetComponent<EnergyTransformEffect>();
          energyTransformEffect.SetupTarget(entity.Target.EnergyContainer.EnergyReactionCTX.UserObject.transform);
        }
      }
    }
    ).UnregisterEventOnDestroy(gameObject);

    TypeBasedEventManager.Instance.RegisterEvent<EnergyTransferEvent>
        (entity =>
        {
          Energy releasedEnergy;
          IEnergyInteractable source = entity.Source;
          IEnergyInteractable target = entity.Target;

          if ((source == null) || (target == null))
          {
            return;
          }

          if (source.EnergyContainer.EnergyReactionCTX.UserObject.TryGetComponent(out PlayerController dummy))
          {
            int selecting = _energyUIController.CurrentSelectingIndex;
            releasedEnergy = source.EnergyContainer.Processor.ReleaseEnergyByIndex(selecting, source.EnergyContainer.EnergyReactionCTX, target.EnergyContainer.EnergyReactionCTX.UserObject.transform.position);
          }
          else
          {
            releasedEnergy = source.EnergyContainer.Processor.ReleaseEnergyByIndex(0, source.EnergyContainer.EnergyReactionCTX, target.EnergyContainer.EnergyReactionCTX.UserObject.transform.position);
          }

          bool bIsSourceSelf = true;
          target.EnergyContainer.EnergyReactionCTX.EnergySourceObject = source.EnergyContainer.EnergyReactionCTX.UserObject;
          if (source == target)
          {
            bIsSourceSelf = false;
          }
          target.EnergyContainer.Processor.AbsorbEnergy(releasedEnergy, target.EnergyContainer.EnergyReactionCTX, bIsSourceSelf);

          if (dummy != null)
          {
            EnergyStockUpdateEvent energyUpdateEvent = new EnergyStockUpdateEvent()
            {
              Energies = source.EnergyContainer.Processor.GetEnergyTypes()
            };
            TypeBasedEventManager.Instance.Send(energyUpdateEvent);
          }
          else if (target.EnergyContainer.EnergyReactionCTX.UserObject.TryGetComponent(out dummy))
          {
            EnergyStockUpdateEvent energyUpdateEvent = new EnergyStockUpdateEvent()
            {
              Energies = target.EnergyContainer.Processor.GetEnergyTypes()
            };
            TypeBasedEventManager.Instance.Send(energyUpdateEvent);
          }
        }
        ).UnregisterEventOnDestroy(gameObject);

    TypeBasedEventManager.Instance.RegisterEvent<StartSelectTargetEvent>
        (entity =>
        {
          _postProcessEffect.SetState(true);
        }
        ).UnregisterEventOnDestroy(gameObject);

    TypeBasedEventManager.Instance.RegisterEvent<StopSelectTargetEvent>
        (entity =>
        {
          _postProcessEffect.SetState(false);
        }
        ).UnregisterEventOnDestroy(gameObject);

    TypeBasedEventManager.Instance.RegisterEvent<SpawnEvent>
        (entity =>
        {
          SendSpawnMessage(entity.SpawnTarget);
        }
        ).UnregisterEventOnDestroy(gameObject);
  }

  private void InitializeRespawnSystem()
  {
    if (_respawnManager == null)
    {
      _respawnManager = new RespawnManager();
    }

    IMediator<IRestartPoint, ISpawnable> mediator = new RestartPointMediator(_respawnManager);
    var restartPoints = FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<IRestartPoint>();
    foreach (var restartPoint in restartPoints)
    {
      restartPoint.SetMediator(mediator);
    }
  }

  private void OnGameSceneLoaded(Scene scene, LoadSceneMode sceneMode)
  {
    // TODO template Game Scene Name
    if (scene.name.Equals(Private.MAIN_GAME_SCENE_NAME))
    {
      _energyUIController = FindAnyObjectByType<EnergyUIController>();
      _postProcessEffect = new ZPostEffect.HideEffect();

      // 中間ポイントシステムを初期化
      InitializeRespawnSystem();

      // プレイヤーを中間ポイントシステムに登録
      PlayerController playerController = FindAnyObjectByType<PlayerController>();
      if (playerController != null)
      {
        RegisterSpawnableObj(playerController.gameObject);  
      } 
    }
  }

  private void OnActiveGameSceneChange(Scene current, Scene next)
  {
    if(string.IsNullOrEmpty(current.name))
    {
      return;
    }

    if(current.name.Equals(Private.MAIN_GAME_SCENE_NAME) && !next.name.Equals(Private.MAIN_GAME_SCENE_NAME))
    {
      _respawnManager?.Dispose();
      _respawnManager = null;

      _postProcessEffect?.Dispose();
      _postProcessEffect = null;
    }
  }
}
