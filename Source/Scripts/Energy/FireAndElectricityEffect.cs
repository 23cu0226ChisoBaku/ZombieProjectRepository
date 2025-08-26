using UnityEngine;

public class FireAndElectricityEffect : MonoBehaviour
{
  private Animator _animator;

  private void Awake()
  {
    _animator = GetComponent<Animator>();
  }

  void Update()
  {
    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
    {
      Destroy(gameObject);
    }
  }
}
