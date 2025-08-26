using System.Collections.Generic;
using UnityEngine;
using Zombie.Global;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class CharacterMovementController : MonoBehaviour,
                                                    IExternalForceAffectable2D
{
    [SerializeField]
    [Header("物理パラメータ")]
    [Space(10)]
    [Header("移動速度")]
    protected float _moveSpeed; 

    [SerializeField]
    [Header("ジャンプ力")]
    protected float _jumpPower;

    [SerializeField]
    [Space(20)]
    [Header("着地チェック")]
    [Space(10)]
    [Header("地面レイヤー")]
    protected LayerMask _groundLayer; 

    [SerializeField]
    [Header("着地チェックレイの長さ")]
    protected float _raycastLength;

    [SerializeField]
    [Space(20)]
    [Header("すり抜けチェック")]
    [Space(10)]
    [Header("すり抜ける天井レイヤー")]
    protected LayerMask _slipThroughCeilLayer;

    [SerializeField]
    [Header("天井チェックレイの長さ")]
    protected float _ceilingRaycastLength;

    [SerializeField]
    [Header("Raycast Box座標オフセット")]
    protected Vector2 _raycastAABBBoxOffset;

    [SerializeField]
    [Header("Raycast Boxサイズ")]
    protected Vector2 _raycastAABBBoxSize;

    protected Rigidbody2D _rigidbody;
    protected bool _isGroundOrCeilCheckActive = true;
    protected Vector2 _externalForce;

    private List<RaycastHit2D> _hitResultBuffer = new List<RaycastHit2D>(3);

    // 着地チェック
    public bool IsGrounded
    {  
      get
      {
        if (!_isGroundOrCeilCheckActive)
        {
          return false;
        }

        int cnt = ZRaycast.AxisAlignedRayCast((Vector2)transform.position + _raycastAABBBoxOffset,
                                                Vector2.down,         // 地面なので下方向
                                                _hitResultBuffer,
                                                _raycastAABBBoxSize.x,
                                                _raycastLength,
                                                _groundLayer
                                              );

        if (cnt < 1)
        {
          return false;
        }

        float groundCheckLength = 0f;
        float charaPosY = transform.position.y;
        
        // コライダーで着地チェックの長さを計算
        if (gameObject.TryGetComponent(out Collider2D collider))
        {
          groundCheckLength = collider.bounds.size.y * 0.5f;
        }
        // コライダーがなければスプライトにフォールバック
        else if (gameObject.TryGetComponent(out SpriteRenderer sr))
        {
          groundCheckLength = sr.size.y * 0.5f;
        }
        else
        {
          Debug.LogError($"Can't calculate half height of character {name}");
        }

        // ヒットチェックを行う
        for (int i = 0; i < cnt; ++i)
        {
          // 当たったところの点のY座標とキャラクターの中心座標のY座標の差分を計算
          float diff = Mathf.Abs(charaPosY - _hitResultBuffer[i].point.y);
          if(diff > groundCheckLength)
          {
            return true;
          }
        }

        return false;
      }
    }

    // 天井チェック
    public bool IsHitCeiling
    {
        get
        {
          int cnt = ZRaycast.AxisAlignedRayCast((Vector2)transform.position + _raycastAABBBoxOffset,
                                                  Vector2.up,       // 天井なので上方向
                                                  _hitResultBuffer,
                                                  _raycastAABBBoxSize.x,
                                                  _raycastLength,
                                                  _groundLayer
                                                );
          if(cnt < 1)
          {
              return false;
          }

          for(int i = 0; i < cnt; ++i)
          {
              var result = _hitResultBuffer[i];

              // 当たったオブジェクトがすり抜けれる天井じゃなかったtrueを返す
              if(((_slipThroughCeilLayer) & (1 << result.collider.gameObject.layer)) == 0)
              {
                  return true;
              }
          }

          return false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(transform.position 
                      + new Vector3(-_raycastAABBBoxSize.x * 0.5f + GlobalParam.RAYCAST_OFFSET * 0.5f
                                    , 0f
                                    , 0f
                                    )
                      ,
                      transform.position 
                      + new Vector3(-_raycastAABBBoxSize.x * 0.5f + GlobalParam.RAYCAST_OFFSET * 0.5f
                                    , 0f
                                    , 0f
                                    )
                      + Vector3.down *_raycastLength
                      );

      Gizmos.DrawLine(transform.position , transform.position + Vector3.down *_raycastLength);

      Gizmos.DrawLine(transform.position 
                      + new Vector3(_raycastAABBBoxSize.x * 0.5f - GlobalParam.RAYCAST_OFFSET * 0.5f
                                    , 0f
                                    , 0f
                                    )
                      ,
                      transform.position
                      + new Vector3(_raycastAABBBoxSize.x * 0.5f - GlobalParam.RAYCAST_OFFSET * 0.5f
                                    , 0f
                                    , 0f
                                    )
                      + Vector3.down *_raycastLength
                      );
    }
#endif
    private protected virtual void FlipSpriteRenderer() { }

    public virtual void SetForce(Vector2 force) {}

    public virtual void SetForceAtPosition(Vector2 force, Vector2 position) {}
}

public static class ZRaycast
{
    /// <summary>
    /// 軸平行レイキャスト、レイを三本飛ばす
    /// </summary>
    /// <param name="startPos">始点座標</param>
    /// <param name="direction">方向</param>
    /// <param name="hitResultBuffer">リザルトバッファ</param>
    /// <param name="rangeLength">�����͈͂̕�</param>
    /// <param name="length">レイの長さ</param>
    /// <param name="layerMask">チェックレイヤー</param>
    /// <returns>当たったレイの本数</returns>
    public static int AxisAlignedRayCast(Vector2 startPos, 
                                         Vector2 direction,
                                         List<RaycastHit2D> hitResultBuffer, 
                                         float rangeLength = 0f, 
                                         float length = Mathf.Infinity, 
                                         int layerMask = Physics2D.DefaultRaycastLayers
                                        )
    {
      hitResultBuffer.Clear();
      direction.Normalize();

      // レイ方向と垂直のベクトルで両辺のレイを中心レイからずらす量を計算
      Vector2 sideRayOffset = Vector2.Perpendicular(direction);
      float sideRayDistanceToMidRay = rangeLength * 0.5f;
      // 両辺からちょっと内側にずらす
      float sideRayOffsetLength = GlobalParam.RAYCAST_OFFSET * 0.5f;

      float sideRayDistanceResult = sideRayDistanceToMidRay - sideRayOffsetLength;

      // レイを三本飛ばす
      var rayLeft = Physics2D.Raycast(startPos + (sideRayOffset * sideRayDistanceResult), direction, length, layerMask);
      var rayMid = Physics2D.Raycast(startPos, direction, length, layerMask);
      var rayRight = Physics2D.Raycast(startPos - (sideRayOffset * sideRayDistanceResult), direction, length, layerMask);

      if(rayLeft.collider != null)
      {
        hitResultBuffer.Add(rayLeft);
      }
      if(rayMid.collider != null)
      {
        hitResultBuffer.Add(rayMid);
      }
      if(rayRight.collider != null)
      {
        hitResultBuffer.Add(rayRight);
      }

      return hitResultBuffer.Count;
    }
}
