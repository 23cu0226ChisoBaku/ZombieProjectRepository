using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Zombie.Global;

public interface IEffect : IDisposable
{
    void SetState(bool bValue);
}

namespace ZPostEffect
{
    public sealed class HideEffect : PostEffectBase
    {
        private Shader _shader;
        private Material _material;
        private Color _color = Color.black;
        private readonly int MAIN_COLOR = Shader.PropertyToID("_FogColor");
        private readonly int FOG_DENSE = Shader.PropertyToID("_FogDense");

        private CommandBuffer _buffer;
        private int _tempTextureIdentifier;

        private bool _currentState = false;

        public HideEffect()
        {
            _shader = Resources.Load("Shader/DimEffect") as Shader;
            // TODO GCの回収が遅かったからここに引っかかったことがある
            // 手動で回収していく(Dispose))
            _material = new Material(_shader);
            _material.SetColor(MAIN_COLOR, _color);
            _material.SetFloat(FOG_DENSE, GlobalParam.FOG_DENSE);

            if (_tempTextureIdentifier == 0)
            {
                _tempTextureIdentifier = Shader.PropertyToID("_PostEffect");
            }

            if (_buffer == null)
            {
                _buffer = new CommandBuffer { name = "DimEffect" };

                _buffer.GetTemporaryRT(_tempTextureIdentifier, -1, -1, 0);
                _buffer.Blit(BuiltinRenderTextureType.CameraTarget, _tempTextureIdentifier);
                _buffer.Blit(_tempTextureIdentifier, BuiltinRenderTextureType.CameraTarget, _material);
                _buffer.ReleaseTemporaryRT(_tempTextureIdentifier);
            }
        }

        ~HideEffect()
        {
            Dispose(false);
        }
        public override void SetState(bool bValue)
        {
            if (_currentState == bValue)
                return;
            
            if(_effectAttachCamera == null)
                return;

            _currentState = bValue;

            // trueだったらバッファをメインカメラに入れる
            // それ以外はバッファをメインカメラから外す
            if (bValue)
            {
                _effectAttachCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _buffer);
            }
            else
            {
                _effectAttachCamera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, _buffer);
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            // Unity Objectを手動解放
            if (disposing)
            {
                if (_shader != null)
                {
                    Resources.UnloadAsset(_shader);
                    _shader = null;
                }
                if (_material != null)
                {
                    UnityEngine.Object.Destroy(_material);
                    _material = null;
                }
                if (_buffer != null)
                {
                    _buffer.Clear();
                    _buffer = null;
                }

                GC.SuppressFinalize(this);
            }

        }
    }

}// namespace ZPostEffect
