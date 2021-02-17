using UnityEngine;
using UnityEngine.UI;

namespace ProjectS
{
    public class TimeView : MonoBehaviour
    {
        #region Member
        
        [SerializeField]
        private Text time;
        
        #endregion
        
        #region Method
        
        public void SetTime(float time)
        {
            this.time.text = string.Format("{0:0}", time);
        }
        
        #endregion
    }
}