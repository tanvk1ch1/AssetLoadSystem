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
        private ScoreView scoreView;
        [SerializeField]
        private ScoreView scoreViewCpu;
        [SerializeField]
        private EnemyOrderListView enemyOrderListView;
        [SerializeField]
        private GameObject enemyOrder;
        [SerializeField]
        private GameObject redPrefab;
        [SerializeField]
        private GameObject container;
        [SerializeField]
        private GameObject overlayCanvas;
        private ViewModel_ShootingGame _viewModel = new ViewModel_ShootingGame();
        private EnemyController _enemy;
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
            // _viewModel.CountDownDisplayState.OnChange += display =>
            // {
            //     if (display) ShowCountDown();
            // };
            _viewModel.ScoreViewDisplayState.OnChange += scoreView.gameObject.SetActive;
            _viewModel.Score.OnChange += scoreView.SetScore;
            
            if (GameDataStore.Instance.PlayerNum == 1)
            {
                // CPU
            }
            
            _viewModel.EnemyOrderListDisplayState.OnChange += enemyOrder.SetActive;
            // _viewModel.EnemyManager.OnAddList += enemyOrderListView.AddList;
            _viewModel.EnemyManager.OnNext += AppearEnemy;
            // _viewModel.EnemyManager.OnNext += enemyOrderListView.Move;
            _viewModel.EnemyManager.Init();
            
            _viewModel.OnEscapeEnemy += EscapeAllEnemy;
            _viewModel.OnDefeat += Defeat;
            _viewModel.OnEscape += Escape;
            
            _viewModel.ResultDisplayState.OnChange += Result;
            
            _viewModel.OnHit += ShowHitEffect;
            _viewModel.OnMiss += ShowMissEffect;
            
            _viewModel.OnLoadPrefabs += LoadPregabs;
            _viewModel.SetPlayerNum(GameDataStore.Instance.PlayerNum);
            _viewModel.FinishDisplayState.OnChange += ShowFinish;
            
            try
            {
                var obj = ResourceStore.Instance.Get<GameObject>("StartCount");
                StartCountDownPhase();
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(e);
                StartLoadPhase();
            }
            // 音を止めたい
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
            AssetLoader.Instance.Unload("ScoreAttack");
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
            // 音を鳴らしたい

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
            var cd = obj.GetComponent<CoundDownAnimation>();
            cd.SetVisible(true);
            cd.SetOnEndCallBack(StartGamePhase);
        }
        
        private void AppearEnemy(IEnemy enemy)
        {
            if (enemy.Type == EnemyType.Group)
            {
                _enemy = SetEnemyTypeGroup(enemyPrefab, container);
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
                            case EnemyColor.Red:
                                prefab = enemyPrefab;
                                break;
                            case EnemyColor.None:
                                prefab = enemyPrefab;
                                break;
                            default:
                                prefab = redPrefab;
                                break;
                        }
                        // 音を鳴らしたい
                        break;
                    case EnemyType.Guard:
                        prefab = redPrefab;
                        // 音を鳴らしたい
                        break;
                    case EnemyType.Danger:
                        prefab = redPrefab;
                        // 音を鳴らしたい
                        break;
                }
                _enemy = SetEnemy(prefab, container);
            }
            
            _enemy.OnFinishedEnter = () =>
            {
                _viewModel.AppearedEnemy();
            };
            _enemy.StartEnterAnimation();
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
        
        private void Escape()
        {
            EscapeEnemy(_enemy);
        }
        
        private void EscapeEnemy(EnemyController enemy)
        {
            enemy.OnFinishedEscape = () =>
            {
                DestroyImmediate(_enemy.gameObject);
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
            _enemy.OnFinishedEscape = () =>
            {
                Destroy(_enemy.gameObject);
                _viewModel.EscapedEnemy();
            };
            _enemy.StartEscapeAnimation();
        }
        
        private void Defeat()
        {
            if (_viewModel.currentEnemy.Type != EnemyType.Danger)
            {
                _enemy.StartDefeatAnimation();
                // 音を鳴らしたい
            }
            else
            {
                _enemy.StartEscapeAnimation();
                // 音を鳴らしたい
            }
            _scoreGenerator.Instantiate(_viewModel.currentEnemy.Point, new Vector3(-700, 0, 0), Destroy);
        }
        
        private void Result(bool display)
        {
            
        }
        
        #endregion
    }
}