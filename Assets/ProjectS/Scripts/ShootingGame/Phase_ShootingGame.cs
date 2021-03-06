using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectS
{
    #region interfaceClass
    
    public interface IPhase_ShootingGame
    {
        Action OnEndPhase { get; set; }
        void Init();
        void Run(float deltaTime);
    }
    
    #endregion
    
    #region PhaseLoad
    
    public class PhaseLoad : IPhase_ShootingGame
    {
        public Action OnEndPhase { get; set; }
        private ViewModel_ShootingGame _viewModel;
        private Task<IList<GameObject>> _loadTask;
        
        public PhaseLoad(ViewModel_ShootingGame _viewModel)
        {
            this._viewModel = _viewModel;
        }
        
        public void Init()
        {
            _loadTask = AssetLoader.Instance.Load<GameObject>("Game");
        }
        
        public void Run(float deltaTime)
        {
            if (_loadTask.IsCompleted)
            {
                _viewModel.LoadPrefabs();
                OnEndPhase?.Invoke();
            }
        }
    }
    
    #endregion
    
    #region PhaseCountDown

    public class PhaseCountDown : IPhase_ShootingGame
    {
        public Action OnEndPhase { get; set; }
        private ViewModel_ShootingGame _viewModel;
        private GameObject readyObj = null;

        public PhaseCountDown(ViewModel_ShootingGame viewModel)
        {
            this._viewModel = viewModel;

            if (readyObj == null)
            {
                readyObj = MonoBehaviour.Instantiate(ResourceStore.Instance.Get<GameObject>("ReadyCanvas"));
                readyObj.SetActive(false);
            }
        }
        
        public void Init()
        {
            FadeManager.Instance.FadeIn(OnFinishFadeIn);
        }

        private void OnFinishFadeIn()
        {
            readyObj.SetActive(true);
        }

        public void Run(float deltaTime)
        {
            if (readyObj == null || !readyObj.activeSelf) return;

            if (InputObserver.Instance.CheckKeyDownDecide())
            {
                Debug.Log("Aキーが押された：ゲーム開始");
                readyObj.SetActive(false);
                _viewModel.ShowCountDown();
            }
        }
    }
    
    #endregion
    
    #region PhaseGame

    public class PhaseGame : IPhase_ShootingGame
    {
        private const float TIME = ViewModel_ShootingGame.TIME;
        private const float INTERVAL = 0.5f;

        private enum Turn
        {
            Interval,
            AppearWait,
            Appeared,
            EscapeWait,
            End
        }

        public Action OnEndPhase { get; set; }
        private float _time;
        private float _enemyTime;
        private ViewModel_ShootingGame _viewModel;
        private Turn turn = Turn.AppearWait;
        private float _enemyAppearInterval;
        private IEnemy _currentEnemy;
        private float _cpuAttackTime;

        public PhaseGame(ViewModel_ShootingGame viewModel)
        {
            this._viewModel = viewModel;
            this._viewModel.EnemyManager.OnNext += NextEnemy;
            this._viewModel.HitPowerLeft.OnChange += InputLeft;
            this._viewModel.HitPowerRight.OnChange += InputRight;
            this._viewModel.OnEnemyAppeared += EnemyAppeared;
            this._viewModel.OnDefeat += EnemyDefeat;
            this._viewModel.OnFinishedEscapeEnemy += EnemyEscaped;
        }
        
        public void Init()
        {
            _viewModel.ShowGameUI();
            _viewModel.UpdateScore1(0);
            _viewModel.UpdateScore2(0);
            _viewModel.NextEnemy();
            _time = 0;
        }
        
        public void Run(float deltaTime)
        {
            _time += deltaTime;
            _viewModel.UpdateTime(TIME - _time);
            
            // _viewModel.UpdateIsAttackRight(InputObserver.Instance.IsHitRight);
            // _viewModel.UpdateIsAttackLeft(InputObserver.Instance.IsHitLeft);
            if (InputObserver.Instance.HitDown == 1) _viewModel.UpdateIsHitLeft(true);
            
            switch (turn)
            {
                case Turn.Interval:
                    _enemyAppearInterval -= deltaTime;
                    if (_enemyAppearInterval <= 0) _viewModel.NextEnemy();
                    break;
                case Turn.AppearWait:
                    break;
                case Turn.Appeared:
                    if (_viewModel.PlayerNum == 1)
                    {
                        _cpuAttackTime -= deltaTime;
                        if (_cpuAttackTime < 0)
                        {
                            _viewModel.AttackRight(1);
                            if (_currentEnemy.Type == EnemyType.Group) NextGroupEnemy();
                            else if (_currentEnemy.Type == EnemyType.Guard) NextGuardEnemy();
                        }
                    }
                    _enemyTime -= deltaTime;
                    if (_enemyTime <= 0)
                    {
                        turn = Turn.EscapeWait;
                        _viewModel.EscapeEnemy();
                    }
                    break;
                case Turn.EscapeWait:
                    break;
                case Turn.End:
                    break;
            }
            if (_time >= TIME) End();
        }
        
        private void End()
        {
            _viewModel.EnemyManager.OnNext -= NextEnemy;
            _viewModel.HitPowerLeft.OnChange -= InputLeft;
            _viewModel.HitPowerRight.OnChange -= InputRight;
            _viewModel.OnEnemyAppeared -= EnemyAppeared;
            _viewModel.OnDefeat -= EnemyDefeat;
            _viewModel.OnFinishedEscapeEnemy -= EnemyEscaped;
            _viewModel.HideGameUI();
            turn = Turn.End;
            OnEndPhase?.Invoke();
        }
        
        private void NextEnemy(IEnemy enemy)
        {
            _currentEnemy = enemy;
            turn = Turn.AppearWait;
            _enemyTime = enemy.Duration;
            if (enemy.Type == EnemyType.Group) NextGroupEnemy();
            else if (enemy.Type == EnemyType.Guard) NextGuardEnemy();
            else
            {
                ICPU_ShootingGame CPU = _viewModel.CPU;
                var random = UnityEngine.Random.Range(0, 100);
                int missRate;
                if (enemy.Type == EnemyType.Danger) missRate = CPU.MissRateDanger;
                else missRate = CPU.MissRate;
                if (random < missRate) _cpuAttackTime = 999;
                else
                {
                    var randomTime = UnityEngine.Random.value;
                    _cpuAttackTime = CPU.AttackTime + CPU.AttackOffset * randomTime;
                }
            }
        }
        
        private void NextGroupEnemy()
        {
            ICPU_ShootingGame CPU = _viewModel.CPU;
            var random = UnityEngine.Random.value;
            _cpuAttackTime = CPU.AttackTime + CPU.AttackOffset * random;
        }
        
        private void NextGuardEnemy()
        {
            ICPU_ShootingGame CPU = _viewModel.CPU;
            var random = UnityEngine.Random.Range(0, 100);
            if (random < CPU.MissRateGuard) _cpuAttackTime = 999;
            else
            {
                var randomTime = UnityEngine.Random.value;
                _cpuAttackTime = CPU.AttackTime + CPU.AttackOffset * randomTime;
                _cpuAttackTime /= 2.0f;
            }
        }
        
        private void InputLeft(int hitPower)
        {
            if (hitPower <= 0) return;
            // if (_viewModel.PlayerNum == 1) return;
            if (turn != Turn.Appeared)
            {
                Debug.Log("ターン：" + turn);
                _viewModel.MissLeft();
                Debug.Log("ミス");
                return;
            }
            _viewModel.AttackLeft(hitPower);
            Debug.Log("攻撃");
        }
        
        private void InputRight(int hitPower)
        {
            if (hitPower <= 0) return;
            if (turn != Turn.Appeared)
            {
                _viewModel.MissRight();
                return;
            }
            _viewModel.AttackRight(hitPower);
        }
        
        private void EnemyAppeared()
        {
            turn = Turn.Appeared;
        }
        
        private void EnemyDefeat()
        {
            turn = Turn.EscapeWait;
        }

        private void EnemyEscaped()
        {
            turn = Turn.Interval;
            _enemyAppearInterval = INTERVAL;
        }
    }
    
    
    
    #endregion
    
    #region PhaseFinish

    public class PhaseFinish : IPhase_ShootingGame
    {
        public Action OnEndPhase { get; set; }
        
        private ViewModel_ShootingGame _viewModel;
        private float _duration = 2;
        
        public PhaseFinish(ViewModel_ShootingGame viewModel)
        {
            this._viewModel = viewModel;
        }
        
        public void Init()
        {
            // 音を鳴らしたい
            _viewModel.ShowFinish();
        }
        
        public void Run(float deltaTime)
        {
            if (_duration <= 0) return;
            _duration -= deltaTime;
            if (_duration <= 0)
            {
                _viewModel.DeleteFinish();
                OnEndPhase?.Invoke();
            }
        }
    }

    #endregion

    #region PhaseResult
    
    public class PhaseResult : IPhase_ShootingGame
    {
        public Action OnEndPhase { get; set; }
        private ViewModel_ShootingGame _viewModel;
        
        public PhaseResult(ViewModel_ShootingGame viewModel)
        {
            this._viewModel = viewModel;
        }
        
        public void Init()
        {
            // 音を鳴らしたい
            _viewModel.ShowResult();
        }
        
        public void Run(float deltaTime)
        {
            // ここは処理なし
        }
    }
    
    #endregion
}
