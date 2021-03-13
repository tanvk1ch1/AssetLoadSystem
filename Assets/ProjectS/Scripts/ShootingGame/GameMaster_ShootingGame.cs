using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectS
{
    public class GameMaster_ShootingGame : BaseGameMaster
    {
        #region Member
        
        private IPhase_ShootingGame _currentPhase;
        [SerializeField]
        private TimeView timeView;
        [SerializeField]
        private ScoreView scoreViewPlayer1;
        [SerializeField]
        private ScoreView scoreViewPlayer2;
        [SerializeField]
        private ScoreView scoreViewCpu;
        
        // いらないかも
        // [SerializeField]
        // private EnemyOrderListView enemyOrderListView;
        
        [SerializeField]
        private GameObject enemyOrder;
        private GameObject enemyPrefab;
        [SerializeField]
        private GameObject leftContainer;
        [SerializeField]
        private GameObject rightContainer;
        [SerializeField]
        private GameObject overlayCanvas;
        private ViewModel_ShootingGame _viewModel = new ViewModel_ShootingGame();
        private EnemyController _leftEnemy;
        private EnemyController _rightEnemy;
        private GameObject _hitEffect;
        private GameObject _missEffect;
        [SerializeField]
        private GameObject playerSelect;
        private ScoreGenerator _scoreGenerator;
        private GameObject _finishObject;
        
        #endregion
        
        #region MainMethod
        
        protected override void StartMain()
        {
            _viewModel = new ViewModel_ShootingGame(GameDataStore.Instance.PlayerNum ,GameDataStore.Instance.GameLevel);
            _viewModel.TimeDisplayState.OnChange += timeView.gameObject.SetActive;
            _viewModel.TimeValue.OnChange += timeView.SetTime;
            _viewModel.CountDownDisplayState.OnChange += display => { if (display) ShowCountDown(); };
            _viewModel.Score1ViewDisplayState.OnChange += scoreViewPlayer1.gameObject.SetActive;
            _viewModel.Score1.OnChange += scoreViewPlayer1.SetScore;
            
            if (GameDataStore.Instance.PlayerNum == 1)
            {
                // CPU
                _viewModel.Score2ViewDisplayState.OnChange += scoreViewCpu.gameObject.SetActive;
                _viewModel.Score2.OnChange += scoreViewCpu.SetScore;
            }
            else
            {
                // 二人対戦やりたいから作ってみる
                _viewModel.Score2ViewDisplayState.OnChange += scoreViewPlayer2.gameObject.SetActive;
                _viewModel.Score2.OnChange += scoreViewPlayer2.SetScore;
            }
            
            // _viewModel.EnemyOrderListDisplayState.OnChange += enemyOrder.SetActive;
            // _viewModel.EnemyManager.OnAddList += enemyOrderListView.AddList;
            _viewModel.EnemyManager.OnNext += AppearEnemy;
            // _viewModel.EnemyManager.OnNext += enemyOrderListView.Move;
            _viewModel.EnemyManager.Init();
            
            _viewModel.OnEscapeEnemy += EscapeAllEnemy;
            _viewModel.OnDefeatLeft += DefeatLeft;
            _viewModel.OnDefeatRight += DefeatRight;
            _viewModel.OnEscapeLeft += EscapeLeft;
            _viewModel.OnEscapeRight += EscapeRight;
            
            _viewModel.ResultDisplayState.OnChange += Result;
            
            _viewModel.OnHitLeft += ShowHitEffectLeft;
            _viewModel.OnHitRight += ShowHitEffectRight;
            _viewModel.OnMissLeft += ShowMissEffectLeft;
            _viewModel.OnMissRight += ShowMissEffectRight;
            
            _viewModel.OnLoadPrefabs += LoadPrefabs;
            _viewModel.SetPlayerNum(GameDataStore.Instance.PlayerNum);
            _viewModel.FinishDisplayState.OnChange += ShowFinish;
            
            // try
            // {
            //     // Debug.Log("try:get");
            //     // var obj = ResourceStore.Instance.Get<GameObject>("StartCount");
            //     // StartCountDownPhase();
            // }
            // catch (KeyNotFoundException e)
            // {
            //     Debug.Log(e);
            //     StartLoadPhase();
            // }
            StartLoadPhase();
            // 鳴ってたらこの辺りで音を止めたい
        }
        
        protected override void UpdateMain()
        {
            var deltaTime = Time.deltaTime;
            _currentPhase?.Run(deltaTime);
        }
        
        #endregion
        
        #region Method
        
        protected void OnDestroy()
        {
            base.OnDestroy();
            AssetLoader.Instance.Unload("Game");
        }
        
        private void StartLoadPhase()
        {
            _currentPhase = new PhaseLoad(_viewModel);
            _currentPhase.OnEndPhase = StartCountDownPhase;
            _currentPhase.Init();
        }
        
        private void StartCountDownPhase()
        {
            // NeedRestartCountDown = false;
            
            playerSelect.SetActive(false);
            _currentPhase = new PhaseCountDown(_viewModel);
            
            _currentPhase.OnEndPhase = StartGamePhase;
            _currentPhase.Init();
            LoadPrefabs();
        }
        
        private void StartGamePhase()
        {
            // NeedRestartCountDown = true;
            // 音を鳴らしたい
            // InputObserver.Instance.GetHit();
            _currentPhase = new PhaseGame(_viewModel);
            _currentPhase.OnEndPhase = StartFinishPhase;
            _currentPhase.Init();
        }
        
        private void StartFinishPhase()
        {
            _currentPhase = new PhaseFinish(_viewModel);
            _currentPhase.OnEndPhase = StartResultPhase;
            _currentPhase.Init();
        }
        
        private void StartResultPhase()
        {
            DestroyEnemies();
            _currentPhase = new PhaseResult(_viewModel);
            _currentPhase.Init();
            _currentPhase.OnEndPhase = () =>
            {
                _currentPhase = null;
                FadeManager.Instance.FadeOut(NextScene);
            };
        }
        
        private void ShowCountDown()
        {
            var prefab = ResourceStore.Instance.Get<GameObject>("StartCount");
            var obj = Instantiate(prefab, overlayCanvas.transform);
            var cd = obj.GetComponent<CountDownAnimation>();
            cd.SetVisible(true);
            cd.SetOnEndCallBack(StartGamePhase);
            Debug.Log("カウントダウン開始");
        }
        
        private void AppearEnemy(IEnemy enemy)
        {
            if (enemy.Type == EnemyType.Group)
            {
                _leftEnemy = SetEnemyTypeGroup(enemyPrefab, leftContainer);
                _rightEnemy = SetEnemyTypeGroup(enemyPrefab, rightContainer);
                // 音も鳴らしたい
            }
            else
            {
                GameObject prefab = enemyPrefab;
                switch (enemy.Type)
                {
                    case EnemyType.Normal:
                        switch (enemy.Color)
                        {
                            case EnemyColor.None:
                                prefab = enemyPrefab;
                                break;
                            default:
                                prefab = enemyPrefab;
                                break;
                        }
                        // 音を鳴らしたい
                        break;
                    case EnemyType.Guard:
                        prefab = enemyPrefab;
                        // 音を鳴らしたい
                        break;
                    case EnemyType.Danger:
                        prefab = enemyPrefab;
                        // 音を鳴らしたい
                        break;
                }
                _leftEnemy = SetEnemy(prefab, leftContainer);
                _rightEnemy = SetEnemy(prefab, rightContainer);
            }

            _leftEnemy.OnFinishedEnter = () =>
            {
                _viewModel.AppearedEnemy();
            };
            _leftEnemy.StartEnterAnimation();
            _rightEnemy.StartEnterAnimation();
        }
        
        private EnemyController SetEnemy(GameObject prefab, GameObject container)
        {
            var obj = Instantiate(prefab);
            var position = obj.transform.position;
            var enemyController = obj.AddComponent<EnemyController>();
            obj.transform.SetParent(container.transform);
            obj.transform.rotation = Quaternion.Euler(0, 180, 0);
            obj.transform.localPosition = position;
            return enemyController;
        }
        
        private EnemyController SetEnemyTypeGroup(GameObject prefab, GameObject container)
        {
            var obj = new GameObject();
            obj.transform.SetParent(container.transform);
            obj.transform.localPosition = Vector3.zero;
            var controller = obj.AddComponent<GroupEnemyController>();
            controller.Prefab = prefab;
            return controller;
        }
        
        private void EscapeLeft()
        {
            EscapeEnemy(_leftEnemy);
        }
        
        private void EscapeRight()
        {
            EscapeEnemy(_rightEnemy);
        }
        
        private void EscapeEnemy(EnemyController enemy)
        {
            enemy.OnFinishedEscape = () =>
            {
                DestroyImmediate(_leftEnemy.gameObject);
                DestroyImmediate(_rightEnemy.gameObject);
                _viewModel.EscapedEnemy();
            };
            enemy.StartEscapeAnimation();
        }
        
        private void EscapeAllEnemy()
        {
            if (_viewModel.currentEnemy.Type != EnemyType.Group)
            {
                // 音を鳴らしたい
            }
            _leftEnemy.OnFinishedEscape = () =>
            {
                Destroy(_leftEnemy.gameObject);
                Destroy(_rightEnemy.gameObject);
                _viewModel.EscapedEnemy();
            };
            _leftEnemy.StartEscapeAnimation();
            _rightEnemy.StartEscapeAnimation();
        }
        
        private void DefeatLeft()
        {
            if (_viewModel.currentEnemy.Type != EnemyType.Danger)
            {
                _leftEnemy.StartDefeatAnimation();
                // 音を鳴らしたい
            }
            else
            {
                _leftEnemy.StartEscapeAnimation();
                // 音を鳴らしたい
            }
            _scoreGenerator.Instantiate(_viewModel.currentEnemy.Point, new Vector3(-700, 0, 0), Destroy);
        }
        
        private void DefeatRight()
        {
            if (_viewModel.currentEnemy.Type != EnemyType.Danger)
            {
                _rightEnemy.StartDefeatAnimation();
                // 音を鳴らしたい
            }
            else
            {
                _rightEnemy.StartEscapeAnimation();
                // 音を鳴らしたい
            }
            _scoreGenerator.Instantiate(_viewModel.currentEnemy.Point, new Vector3(300, 0, 0), Destroy);
        }
        
        private void Result(bool display)
        {
            var prefab = ResourceStore.Instance.Get<GameObject>("GameResult");
            var obj = Instantiate(prefab, overlayCanvas.transform);
            var animator = obj.GetComponent<Animator>();
            
            if (_viewModel.Score1.Value > _viewModel.Score2.Value)
            {
                animator.Play("1PWin");
            }
            else if (_viewModel.Score2.Value > _viewModel.Score1.Value)
            {
                if (GameDataStore.Instance.PlayerNum > 1) animator.Play("2PWin");
                else animator.Play("CPUWin");
            }
            else
            {
                animator.Play("Draw");
            }
            
            // obj.GetComponent<AnimationEvent>().OnEnd = g => NextScene();
        }
        
        private void ShowHitEffectRight()
        {
            // 効果音を鳴らしたい
            Instantiate(_hitEffect, rightContainer.transform);
        }
        
        private void ShowHitEffectLeft()
        {
            // 効果音を鳴らしたい
            Instantiate(_hitEffect, leftContainer.transform);
        }
        
        private void ShowMissEffectRight()
        {
            // 効果音を鳴らしたい
            Instantiate(_missEffect, rightContainer.transform);
        }
        
        private void ShowMissEffectLeft()
        {
            // 効果音を鳴らしたい
            Instantiate(_missEffect, leftContainer.transform);
        }
        
        // private void ShowMissHitEffectRight()
        // {
        //     Instantiate(_missEffect, rightContainer.transform);
        // }
        //
        // private void ShowMissHitEffectLeft()
        // {
        //     Instantiate(_missEffect, leftContainer.transform);
        // }

        private void DestroyEnemies()
        {
            if (_leftEnemy)
            {
                Destroy(_leftEnemy.gameObject);
                _leftEnemy = null;
            }

            if (_rightEnemy)
            {
                Destroy(_rightEnemy.gameObject);
                _rightEnemy = null;
            }
        }
        
        private void LoadPrefabs()
        {
            enemyPrefab = ResourceStore.Instance.Get<GameObject>("Enemy");
            _hitEffect = ResourceStore.Instance.Get<GameObject>("PlayerAttackEffect");
            _missEffect = ResourceStore.Instance.Get<GameObject>("PlayerAttackEffectMiss");
            var generator = ResourceStore.Instance.Get<GameObject>("ScoreGenerator");
            _scoreGenerator = Instantiate(generator).GetComponent<ScoreGenerator>();
            _scoreGenerator.SetTargetCanvas(overlayCanvas);
            
            Instantiate(ResourceStore.Instance.Get<GameObject>("Stage"), rightContainer.transform);
            Instantiate(ResourceStore.Instance.Get<GameObject>("Stage"), leftContainer.transform);
        }
        
        private void ShowFinish(bool state)
        {
            if (state)
            {
                var prefab = ResourceStore.Instance.Get<GameObject>("GameFinish");
                var obj = Instantiate(prefab, overlayCanvas.transform);
                obj.SetActive(true);
                _finishObject = obj;
            }
            else
            {
                if (_finishObject != null)
                {
                    Destroy(_finishObject);
                    _finishObject = null;
                }
            }
        }
        
        private void NextScene()
        {
            FadeManager.Instance.FadeOut(OnFinishFadeOut);
        }
        
        private void OnFinishFadeOut()
        {
            // 今はこの辺は適当な名前でしかない
            // GameDataStore.Instance.NextSceneName = "ShootingGameScene";
            // GameDataStore.Instance.AssetBundleLabel = "Game";
            // SceneManager.LoadScene("ShootingGameScene");
        }
        
        #endregion
    }
}