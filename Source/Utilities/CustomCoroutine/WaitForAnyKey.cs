/*

Filename    : WaitForAnyKey.cs
Description : base Coroutine about any input

Version     : 1.0
Author      : MAI ZHICONG
Update      : 2024/09/07 Create

*/

using System.Collections;

namespace Unity_MTool.Coroutine
{
    public abstract class WaitForAnyKey : IEnumerator
    {
        public object Current => null;

        public abstract bool MoveNext(); 

        public void Reset() {}
    }
}
