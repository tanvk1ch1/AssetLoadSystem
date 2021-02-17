using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectS
{
    public class ScoreAnimationView : MonoBehaviour
    {
        #region Member
        
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private AnimationEvent animationEvent;
        
        public Action<GameObject> OnEndAnimation;
        
        #endregion
        
        #region MonoBehavior
        
        private void Awake()
        {
            animationEvent.OnEnd = EndAnimation;
        }
        
        #endregion
        
        #region Method
        
        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }
        
        public void StartAnimation()
        {
            gameObject.SetActive(true);
        }
        
        public void EndAnimation(GameObject notUse)
        {
            var obj = gameObject;
            obj.SetActive(false);
            OnEndAnimation?.Invoke(obj);
        }
        
        #endregion
    }
}