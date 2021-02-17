using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectS
{
    public class FadeManager : MonoBehaviour
    {
        #region Member
        
        private Action _onFinish;
        private static Canvas _fadeCanvas;
        private static Image _fadeImage;
        private static float _alpha;
        private bool _isFadeIn;
        private bool _isFadeOut;
        
        public bool IsFade
        {
            get { return (_isFadeIn || _isFadeOut); }
        }
        private static float fadeTime = 0.5f;
        private static FadeManager _instance;
        
        public static FadeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("FadeManager");
                    _fadeCanvas = obj.AddComponent<Canvas>();
                    obj.AddComponent<GraphicRaycaster>();
                    _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    _instance = obj.AddComponent<FadeManager>();
                    _fadeCanvas.sortingOrder = 100;
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region MonoBehavior
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        
        public void Init()
        {
            if (_fadeImage != null)
            {
                _fadeImage.color = Color.black;
                return;
            }
            _fadeImage = new GameObject("ImageFade").AddComponent<Image>();
            _fadeImage.transform.SetParent(_fadeCanvas.transform, false);
            _fadeImage.rectTransform.anchoredPosition = Vector3.zero;
            _fadeImage.rectTransform.sizeDelta = new Vector2(2560, 1080); // 仮の値
            _fadeImage.color = Color.black;
        }

        private void Update()
        {
            if (_isFadeIn)
            {
                _alpha -= Time.deltaTime / fadeTime;
                
                if (_alpha <= 0.0f)
                {
                    _isFadeIn = false;
                    _alpha = 0.0f;
                    _fadeCanvas.enabled = false;
                    Finish();
                }
                
                _fadeImage.color = new Color(0.0f, 0.0f, 0.0f, _alpha);
            }
            else if (_isFadeOut)
            {
                _alpha += Time.deltaTime / fadeTime;
                
                if (_alpha >= 1.0f)
                {
                    _isFadeOut = false;
                    _alpha = 1.0f;
                    Finish();
                }
                
                _fadeImage.color = new Color(0.0f, 0.0f, 0.0f, _alpha);
            }
        }

        #endregion
        
        #region Method
        
        private void Finish()
        {
            _onFinish?.Invoke();
        }
        
        public void FadeIn(Action callback)
        {
            if (_fadeImage == null)
            {
                Init();
            }
            _onFinish = callback;
            _fadeImage.color = Color.black;
            _isFadeIn = true;
            _alpha = 1.0f;
        }
        
        public void FadeOut(Action callback)
        {
            if (_fadeImage == null)
            {
                Init();
            }
            _onFinish = callback;
            _fadeImage.color = Color.clear;
            _fadeCanvas.enabled = true;
            _isFadeOut = true;
            _alpha = 0.0f;
        }
        
        #endregion
    }
}