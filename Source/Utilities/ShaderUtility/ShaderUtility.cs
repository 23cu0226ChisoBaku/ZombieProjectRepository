using UnityEngine;

namespace MEffect
{
    public static partial class ShaderUtility
    {
        // TODO 名前がよくない
        public static Vector2 GetScreenFadeTargetPos(Vector3 worldPos)
        {
            Vector2 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

            viewportPos *= 2f;
            viewportPos -= Vector2.one;

            return viewportPos;
        }
    }
}