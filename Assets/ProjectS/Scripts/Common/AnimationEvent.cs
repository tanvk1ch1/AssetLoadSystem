using System;
using UnityEngine;

namespace ProjectS
{
    public class AnimationEvent : MonoBehaviour
    {
        #region Member
        
        public Action<GameObject> OnEnd;
        public Action<string> OnFinishAnimation;
        
        #endregion
        
        #region Method
        
        public void OnEndAnimation()
        {
            Debug.Log("Animationが終了した");
            OnEnd?.Invoke(gameObject);
        }
        
        #endregion
        
    }
}

