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
        private Task<IList<GameObject>> loadTask;
        
        public PhaseLoad(ViewModel_ShootingGame _viewModel)
        {
            this._viewModel = _viewModel;
        }
        
        public void Init()
        {
            loadTask = AssetLoader.Instance.Load<GameObject>("ShootingGame");
        }
        
        public void Run(float deltaTime)
        {
            if (loadTask.IsCompleted)
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
            this._viewModel.OnEnemyAppeared += EnemyAppeared;
            this._viewModel.OnDefeat += EnemyDefeat;
            this._viewModel.OnFinishedEscapeEnemy += EnemyEscaped;
        }
        
        public void Init()
        {
            _viewModel.ShowGameUI();
            _viewModel.UpdateScore(0);
            _viewModel.NextEnemy();
            _time = 0;
        }
        
        public void Run(float deltaTime)
        {
            _time += deltaTime;
            _viewModel.UpdateTime(TIME - _time);

            switch (turn)
            {
                case Turn.Interval:
                    _enemyAppearInterval -= deltaTime;
                    if (_enemyAppearInterval <= 0) _viewModel.NextEnemy();
                    break;
                case Turn.AppearWait:
                    break;
                case Turn.Appeared:
                    _cpuAttackTime -= deltaTime;
                    if (_cpuAttackTime < 0)
                    {
                        _viewModel.Attack(1);
                        if (_currentEnemy.Type == EnemyType.Group) NextGroupEnemy();
                        else if (_currentEnemy.Type == EnemyType.Guard) NextGuardEnemy();
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
                var rand = UnityEngine.Random.Range(0, 100);
                int missRate;
                if (enemy.Type == EnemyType.Danger) missRate = CPU.MissRateDanger;
                else missRate = CPU.MissRate;
                if (rand < missRate) _cpuAttackTime = 999;
                else
                {
                    var randTime = UnityEngine.Random.value;
                    _cpuAttackTime = CPU.AttackTime + CPU.AttackOffset * randTime;
                }
            }
        }
        
        private void NextGroupEnemy()
        {
            ICPU_ShootingGame CPU = _viewModel.CPU;
            var rand = UnityEngine.Random.value;
            _cpuAttackTime = CPU.AttackTime + CPU.AttackOffset * rand;
        }
        
        private void NextGuardEnemy()
        {
            ICPU_ShootingGame CPU = _viewModel.CPU;
            var rand = UnityEngine.Random.Range(0, 100);
            if (rand < CPU.MissRateGuard) _cpuAttackTime = 999;
            else
            {
                var randTime = UnityEngine.Random.value;
                _cpuAttackTime = CPU.AttackTime + CPU.AttackOffset * randTime;
                _cpuAttackTime /= 2.0f;
            }
        }
        
        private void Input(int hitPower)
        {
            if (turn != Turn.Appeared)
            {
                _viewModel.Miss();
                return;
            }
            _viewModel.Attack(hitPower);
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
