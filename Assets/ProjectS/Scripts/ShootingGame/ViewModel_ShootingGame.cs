using System;
using UnityEngine.Rendering;

namespace ProjectS
{
    public class ViewModel_ShootingGame
    {
        #region Member
        
        public const float TIME = 90;
        public ValueObserver<bool> TitleDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<int> Score { get; } = new ValueObserver<int>(0);
        public ValueObserver<bool> ScoreViewDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> CountDownDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> TimeDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<float> TimeValue { get; } = new ValueObserver<float>(TIME);
        public ValueObserver<bool> ResultDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> EnemyOrderListDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> FinishDisplayState { get; } = new ValueObserver<bool>(true);
        public EnemyManager EnemyManager { get; } = new EnemyManager();
        public IEnemy currentEnemy { get; private set; }
        public ICPU_ShootingGame CPU { get; }
        
        public Action OnDefeat;
        public Action OnEscape;
        public Action OnEscapeEnemy;
        public Action OnEnemyAppeared;
        public Action OnFinishedEscapeEnemy;
        public Action OnHit;
        public Action OnMiss;
        public Action OnLoadPrefabs;
        
        private int _enemyHp;
        
        #endregion
        
        public ViewModel_ShootingGame(int playerNum = 1, GameDataStore.Level cpuLevel = GameDataStore.Level.Level2)
        {
            switch (cpuLevel)
            {
                case GameDataStore.Level.Level1:
                    CPU = new CPUEasy();
                    break;
                case GameDataStore.Level.Level2:
                    CPU = new CPUNormal();
                    break;
                case GameDataStore.Level.Level3:
                    CPU = new CPUHard();
                    break;
            }
        }

        #region Method
        
        public void ShowCountDown()
        {
            CountDownDisplayState.Value = true;
        }

        public void UpdateTime(float value)
        {
            TimeValue.Value = value;
        }
        
        public void UpdateScore(int value)
        {
            Score.Value = value;
        }
        public void ShowGameUI()
        {
            TimeDisplayState.Value = true;
            ScoreViewDisplayState.Value = true;
            EnemyOrderListDisplayState.Value = true;
        }
        
        public void HideGameUI()
        {
            TimeDisplayState.Value = false;
            ScoreViewDisplayState.Value = false;
            EnemyOrderListDisplayState.Value = false;
        }
        
        public void NextEnemy()
        {
            currentEnemy = EnemyManager.Next();
            _enemyHp = currentEnemy.Hp;
        }
        
        public void AppearedEnemy()
        {
            OnEnemyAppeared?.Invoke();
        }
        
        public void EscapeEnemy()
        {
            OnEscapeEnemy?.Invoke();
        }
        
        public void EscapedEnemy()
        {
            OnFinishedEscapeEnemy?.Invoke();
        }
        
        public void Attack(int damage)
        {
            if (currentEnemy.Type == EnemyType.Danger)
            {
                OnMiss?.Invoke();
                UpdateScore(Score);
                OnDefeat?.Invoke();
                OnEscape?.Invoke();
                return;
            }
            if (damage > 0) OnHit?.Invoke();
            if (currentEnemy.Type == EnemyType.Group)
            {
                UpdateScore(Score);
                OnDefeat?.Invoke();
                return;
            }
            _enemyHp -= damage;
            if (_enemyHp <= 0)
            {
                UpdateScore(Score);
                OnDefeat?.Invoke();
                // OnEscape?.Invoke();
            }
        }
        
        public void Miss()
        {
            OnMiss?.Invoke();
        }
        
        private void UpdateScore(ValueObserver<int> score)
        {
            var currentScore = score.Value;
            var newScore = currentScore + currentEnemy.Point;
            if (newScore < 0) newScore = 0;
            score.Value = newScore;
        }
        
        public void ShowResult()
        {
            ResultDisplayState.Value = true;
        }
        
        public void LoadPrefabs()
        {
            OnLoadPrefabs?.Invoke();
        }
        
        public void ShowFinish()
        {
            FinishDisplayState.Value = true;
        }
        
        public void DeleteFinish()
        {
            FinishDisplayState.Value = false;
        }
        
        #endregion
        
        
    }
}