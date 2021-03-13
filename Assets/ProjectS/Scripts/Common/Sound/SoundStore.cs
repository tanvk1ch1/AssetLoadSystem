using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectS
{
    public sealed class SoundStore : MonoBehaviour
    {
        public static SoundStore Instance { get; } = new SoundStore();
        
        private SoundStore()
        {
        }
        
        public AudioClip GetSe(Se id)
        {
            var name = SeDict.Dict[id];
            return ResourceStore.Instance.Get<AudioClip>(name);
        }
        
        public AudioClip GetBgm(Bgm id)
        {
            var name = BgmDict.Dict[id];
            return ResourceStore.Instance.Get<AudioClip>(name);
        }
    }
}

