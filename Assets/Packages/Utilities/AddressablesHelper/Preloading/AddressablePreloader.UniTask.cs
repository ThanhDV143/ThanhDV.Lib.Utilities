#if UNITASK_AVAILABLE
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ThanhDV.Utilities
{
    public sealed partial class AddressablePreloader
    {
        public UniTask PreloadKeyAsync<TObject>(object key, IProgress<float> progress = null)
        {
            return PreloadKey<TObject>(key, progress).ToUniTask();
        }

        public UniTask PreloadKeysAsync<TObject>(IEnumerable<object> keys, IProgress<float> progress = null)
        {
            return PreloadKeys<TObject>(keys, progress).ToUniTask();
        }
    }
}
#endif
