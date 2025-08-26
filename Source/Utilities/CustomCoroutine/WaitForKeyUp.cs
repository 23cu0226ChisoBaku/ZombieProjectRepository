/*

Filename    : WaitForKeyUp.cs
Description : Coroutine about input up

*** currently is supporting only one key ***

Version     : 1.0
Author      : MAI ZHICONG
Update      : 2024/09/07 Create

*/
using UnityEngine;

namespace Unity_MTool.Coroutine
{
    public sealed class WaitForKeyUp : WaitForKey
    {

        public WaitForKeyUp(KeyCode keyCode) : base(keyCode)
        {

        }
        public override bool MoveNext()
        {
            return !Input.GetKeyUp(_keycode);
        }

    }
}