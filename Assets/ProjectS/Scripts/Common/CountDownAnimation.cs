using System;
using UnityEngine;

namespace ProjectS
{
    public class CountDownAnimation : MonoBehaviour
    {
        private Action _onEndCallBack;
        
        public void SetVisible(bool flag)
        {
            gameObject.SetActive(flag);
        }
        
        public void SetOnEndCallBack(Action callback)
        {
            _onEndCallBack = callback;
        }
    }
}