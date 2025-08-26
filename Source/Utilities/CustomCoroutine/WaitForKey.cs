/*

Filename    : WaitForKey.cs
Description : base Coroutine about input

*** currently is supporting only one key ***

Version     : 1.0
Author      : MAI ZHICONG
Update      : 2024/09/07 Create

*/

using System.Collections;
using UnityEngine;

namespace Unity_MTool.Coroutine
{
    public abstract class WaitForKey : IEnumerator
    {
        protected KeyCode _keycode;
        public object Current => null;

        public abstract bool MoveNext();

        public void Reset() {}

        public WaitForKey(KeyCode keyCode)
        {
            _keycode = keyCode;
        }
    }

}