using UnityEngine;
using UnityEngine.UI;

namespace ProjectS
{
    public class ScoreView : MonoBehaviour
    {
        #region Member
        
        [SerializeField]
        private Text score;
        
        #endregion
        
        #region Method
        
        public void SetScore(int score)
        {
            this.score.text = "" + score;
        }
        
        #endregion
    }
}