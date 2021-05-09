using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectS
{
    public class EnemyController : MonoBehaviour
    {
        #region Member
        
        public Action OnFinishedEnter;
        public Action OnFinishedEscape;
        public Action OnFinishedDefeat;
        
        public Vector3 anchorPoint; // カットする場所
        public Vector3 normalDirection; // カットする向き
        public Material capMaterial;

        [SerializeField]
        private Animator animator;
        
        #endregion
        
        #region Method
        
        public virtual void StartEnterAnimation()
        {
            StartCoroutine(EnterAnimation());
        }
        
        IEnumerator EnterAnimation()
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "z", EnemyConstants.EnemyStartBreakPosZ,
                "time", EnemyConstants.EnemyAppearAnimationTime,
                "easeType", "linear"
            ));
            
            // AnimationTime - Delay + ShockTime = Time
            iTween.ShakeRotation(gameObject, iTween.Hash(
                "x" , 10f,
                "delay", EnemyConstants.EnemyAppearAnimationTime - 0.1f,
                "time", 0.1f
            ));
            
            yield return new WaitForSeconds(EnemyConstants.EnemyAppearAnimationTime);
            
            var obj = gameObject;
            iTween.MoveTo(obj, iTween.Hash(
                "z", EnemyConstants.EnemyStartStopPosZ,
                "time", EnemyConstants.EnemyAppearAnimationBackTime,
                "onComplete", "AppearAnimationEnd",
                "onCompleteTarget", obj
            ));
        }
        
        public virtual void StartDefeatAnimation()
        {
            animator.SetBool("death", true);
            var obj = gameObject;
            iTween.MoveTo(obj, iTween.Hash(
                "z", EnemyConstants.EnemyDefeatPosZ,
                "x", transform.position.x,
                "y", EnemyConstants.EnemyDefeatPosY,
                "time", EnemyConstants.EnemyLeaveAnimationTime,
                "easeType", "easeOutCubic",
                "onComplete", "DefeatAnimationEnd",
                "onCompleteTarget", obj
            ));
            float rotateY = 0;
            float rotateZ = 0;
            iTween.RotateBy(gameObject, iTween.Hash(
                "x", EnemyConstants.EnemyDefeatRotateX,
                "y", rotateY,
                "z", rotateZ
            ));
        }

        public virtual void StartEscapeAnimation()
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "z", EnemyConstants.EnemyEscapePosZ,
                "time", EnemyConstants.EnemyEscapeAnimationTime,
                "onComplete", "EscapeAnimationEnd",
                "onCompleteTarget", gameObject,
                "easeType", "linear"
            ));
        }

        public virtual void Cut()
        {
            anchorPoint = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            normalDirection = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));

            capMaterial = GetComponent<Renderer>().material;
            GameObject[] cutObjs = MeshCut.Cut(gameObject, anchorPoint, normalDirection, capMaterial);
            cutObjs[0].AddComponent<Rigidbody>();
            Destroy(cutObjs[0].GetComponent<BoxCollider>());
            cutObjs[0].AddComponent<BoxCollider>();
            
            cutObjs[1].AddComponent<Rigidbody>();
            cutObjs[1].AddComponent<BoxCollider>();
            cutObjs[1].AddComponent<EnemyController>().capMaterial = capMaterial;
            Destroy(cutObjs[0], 1.0f);
            Destroy(cutObjs[1], 1.0f);
            OnFinishedDefeat?.Invoke();
        }
        
        private void AppearAnimationEnd()
        {
            iTween.Stop(this.gameObject);
            // animator.SetBool("wait", true);
            OnFinishedEnter?.Invoke();
        }
        
        private void EscapeAnimationEnd()
        {
            iTween.Stop(this.gameObject);
            OnFinishedEscape?.Invoke();
        }
        
        private void DefeatAnimationEnd()
        {
            iTween.Stop(this.gameObject);
            OnFinishedDefeat?.Invoke();
        }
        
        #endregion
        
        #region MonoBehavior

        private void Awake()
        {
            animator = transform.GetComponentInChildren<Animator>();
        }

        private void OnDestroy()
        {
            iTween.Stop(gameObject);
        }
        
        #endregion
    }
}