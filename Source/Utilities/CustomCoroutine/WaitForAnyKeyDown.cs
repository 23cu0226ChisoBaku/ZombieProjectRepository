/*

Filename    : WaitForAnyKeyDown.cs
Description : Coroutine about any input down

Version     : 1.0
Author      : MAI ZHICONG
Update      : 2024/09/07 Create

*/

using UnityEngine;

namespace Unity_MTool.Coroutine
{
    public sealed class WaitForAnyKeyDown : WaitForAnyKey
    {
        public override bool MoveNext()
        {
            return !Input.anyKeyDown;
        }
    }
}