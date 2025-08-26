using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    #if SYSTEM_PRIVATE_CORELIB
        public
    #else
        internal
    #endif
    static class IsExternalInit
    {

    }
}