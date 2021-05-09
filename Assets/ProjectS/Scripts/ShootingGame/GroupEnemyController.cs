using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectS
{
    public class GroupEnemyController : EnemyController
    {
        #region Member
        
        private const float INTERVAL = 0.2f;
        private const int MAX = 30;
        private const float DEFAULT_Z = -4.2f;
        private const float OFFSET_Z = 1.0f;
        public GameObject Prefab;
        private List<GameObject> children = new List<GameObject>();
        private float interval = INTERVAL;
        private bool _enableAddEnemy = true;
        
        #endregion
        
        #region MonoBehavior
        private void Update()
        {
            if (!_enableAddEnemy)
            {
                if (children.Count == 0)
                {
                    OnFinishedEscape?.Invoke();
                }
                return;
            }
            if (children.Count >= MAX) return;
            interval -= Time.deltaTime;
            if (interval >= 0) AddEnemy();
        }
        
        #endregion
        
        #region Method
        
        private void AddEnemy()
        {
            var child = Instantiate(Prefab, transform);
            child.transform.Rotate(new Vector3(10,90,0));
            var z = DEFAULT_Z + OFFSET_Z * UnityEngine.Random.Range(-1.0f, 1.0f);
            var startPos = new Vector3(-1.2f, 0.6f, z);
            child.transform.localPosition = startPos;
            var endPos = new System.Numerics.Vector3(1.2f, 0.6f, z);
            var tweenHash = new Hashtable();
            tweenHash.Add("x", child.transform.position.x + 2.4f);
            tweenHash.Add("time", 1.0);
            tweenHash.Add("OnComplete", "OnFinishedMove");
            tweenHash.Add("OnCompleteParams", child);
            tweenHash.Add("OnCompleteTarget", gameObject);
            tweenHash.Add("EaseType", iTween.EaseType.linear);
            iTween.MoveTo(child, tweenHash);
            children.Add(child);
            interval = INTERVAL;
        }
        
        public override void StartEnterAnimation()
        {
            AddEnemy();
            StartCoroutine(WaitEnter());
        }
        
        private IEnumerator WaitEnter()
        {
            yield return new WaitForSeconds(0.01f);
            OnFinishedEnter?.Invoke();
        }
        
        public override void StartDefeatAnimation()
        {
            var center = new Vector3(0, 0.6f, DEFAULT_Z);
            var list = new List<float>();
            foreach (var child in children)
            {
                list.Add(Vector3.Distance(center, child.transform.localPosition));
            }
            var min = list.Min();
            var index = list.IndexOf(min);
            var obj = children[index];
            iTween.Stop(obj);
            DestroyImmediate(obj.GetComponent<iTween>());

            iTween.MoveTo(obj, iTween.Hash(
                "z", EnemyConstants.EnemyDefeatPosZ,
                "x", transform.position.x,
                "y", EnemyConstants.EnemyDefeatPosY,
                "time", EnemyConstants.EnemyLeaveAnimationTime,
                "easeType", "easeOutQubic",
                "onComplete", "DefeatAnimationEnd",
                "onCompleteParams", obj,
                "onCompleteTarget", gameObject
            ));
            float rotateY = 0;
            float rotateZ = 0;
            iTween.RotateBy(obj, iTween.Hash(
                "x", EnemyConstants.EnemyDefeatRotateX, 
                "y", rotateY,
                "z", rotateZ
            ));
        }
        
        public override void StartEscapeAnimation()
        {
            _enableAddEnemy = false;
        }

        public override void Cut()
        {
            
        }
        
        #endregion
    }
}