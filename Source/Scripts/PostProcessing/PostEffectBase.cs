using Unity.VisualScripting;
using UnityEngine;

namespace ZPostEffect
{
    public abstract class PostEffectBase: IEffect
    {
        protected Camera _effectAttachCamera;
        protected PostEffectBase()
        {
            if(_effectAttachCamera == null)
            {
                _effectAttachCamera = Camera.main;
            }
        }

        public abstract void SetState(bool bValue);

        public abstract void Dispose();
    }
}