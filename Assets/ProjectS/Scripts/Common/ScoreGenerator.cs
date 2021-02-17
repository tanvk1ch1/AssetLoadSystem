using System;
using UnityEngine;

namespace ProjectS
{
    public class ScoreGenerator : MonoBehaviour
    {
        #region Member
        
        [SerializeField]
        private ScoreAnimationView plusPrefab;
        [SerializeField]
        private ScoreAnimationView minusPrefab;
        [SerializeField]
        private GameObject canvas;
        
        #endregion
        
        #region Method
        
        public void SetTargetCanvas(GameObject target)
        {
            canvas = target;
        }
        
        public void Instantiate(int score, Vector3 offset, Action<GameObject> onEnd)
        {
            ScoreAnimationView obj;
            if (score >= 0)
            {
                obj = Instantiate(plusPrefab);
                // 音を再生したい
            }
            else
            {
                obj = Instantiate(minusPrefab);
                // 音を再生したい
            }
            
            Transform trf;
            (trf = obj.transform).SetParent(canvas.transform);
            trf.localPosition = offset;
            obj.SetScore(Math.Abs(score));
            obj.OnEndAnimation = onEnd;
            obj.StartAnimation();
        }
        
        #endregion
    }
}