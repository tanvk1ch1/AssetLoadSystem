using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectS
{
    public abstract class BaseGameMaster : MonoBehaviour
    {
        #region Member
        
        private enum State
        {
            Running,
            Pausing,
            WaitCountDown
        }
        private GameObject StartCountDownPrefab { get => ResourceStore.Instance.Get<GameObject>("StartCount"); }
        private GameObject PauseDialogPrefab { get => ResourceStore.Instance.Get<GameObject>("Pause"); }
        private Animator countDownAnimator = null;
        private bool isLoading = false;
        private State state = State.Running;
        protected bool IsPausing
        {
            get => state == State.Pausing;
            private set => state = value ? State.Pausing : State.Running;
        }
        private PauseDialog _pauseDialog;
        private bool _enablePause = true;
        public bool EnablePause { get => _enablePause; set => _enablePause = value; }
        protected bool NeedLoadSystem { get => true; }
        protected bool NeedRestartCountDown = false;
        
        #endregion
        
        #region MonoBehavior
        
        private void Awake()
        {
            if (!AssetLoader.Instance.IsLoadedSystemAsset && NeedLoadSystem) StartCoroutine(LoadSystem());
        }
        
        private void Start()
        {
            StartMain();
        }
        
        private void Update()
        {
            if (isLoading) return;
            
            if (state == State.WaitCountDown) countDownAnimator.Update(TimeUtility.UnscaledDeltaTime);
            
            if (IsPausing) return;
            
            if (Mathf.Approximately(Time.timeScale, 0f)) return;
            UpdateMain();
        }
        
        protected void OnDestroy()
        {
            iTween.Stop();
            iTween.tweens.Clear();
            InputObserver.Instance.ClearEvents();
        }
        
        #endregion

        #region Method

        abstract protected void StartMain();
        abstract protected void UpdateMain();

        private IEnumerator LoadSystem()
        {
            isLoading = true;
            var task = AssetLoader.Instance.LoadSystem();
            while (!task.IsCompleted) { yield return null; }
            isLoading = false;
        }
        
        // ポーズ処理をはじめとする、何らかの共通処理を書きたい
        // が、ポーズ処理不要説やゲームの規模を大きくしすぎないために、処理を大幅カットするかも
        
        #endregion
    }
}