using System;
using UnityEngine.Rendering;

namespace ProjectS
{
    public class ViewModel_ShootingGame
    {
        #region Member
        
        public const float TIME = 60;
        public ValueObserver<bool> TitleDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<int> Score1 { get; } = new ValueObserver<int>(0);
        public ValueObserver<int> Score2 { get; } = new ValueObserver<int>(0);
        public ValueObserver<bool> Score1ViewDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> Score2ViewDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> CountDownDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> TimeDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<float> TimeValue { get; } = new ValueObserver<float>(TIME);
        public ValueObserver<bool> ResultDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> EnemyOrderListDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<bool> FinishDisplayState { get; } = new ValueObserver<bool>(true);
        public ValueObserver<int> HitPowerPlayer1 { get; } = new ValueObserver<int>(0);
        public ValueObserver<int> HitPowerPlayer2 { get; } = new ValueObserver<int>(0);
        public EnemyManager EnemyManager { get; } = new EnemyManager();
        public IEnemy currentEnemy { get; private set; }
        public int PlayerNum { get; private set; }
        public ICPU_ShootingGame CPU { get; }
        
        public Action OnDefeatPlayer1;
        public Action OnDefeatPlayer2;
        public Action OnEscapePlayer1;
        public Action OnEscapePlayer2;
        public Action OnDefeat;
        public Action OnEscapeEnemy;
        public Action OnEnemyAppeared;
        public Action OnFinishedEscapeEnemy;
        
        public Action OnHitPlayer1;
        public Action OnHitPlayer2;
        public Action OnMissPlayer1;
        public Action OnMissPlayer2;
        public Action OnLoadPrefabs;
        
        private int _enemyHpPlayer1Side;
        private int _enemyHpPlayer2Side;
        
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

        public void SetPlayerNum(int playerNum)
        {
            PlayerNum = playerNum;
        }
        
        public void ShowCountDown()
        {
            CountDownDisplayState.Value = true;
        }

        public void UpdateTime(float value)
        {
            TimeValue.Value = value;
        }
        
        public void UpdateScore1(int value)
        {
            Score1.Value = value;
        }
        public void UpdateScore2(int value)
        {
            Score2.Value = value;
        }
        public void ShowGameUI()
        {
            TimeDisplayState.Value = true;
            Score1ViewDisplayState.Value = true;
            Score2ViewDisplayState.Value = true;
            EnemyOrderListDisplayState.Value = true;
        }
        
        public void HideGameUI()
        {
            TimeDisplayState.Value = false;
            Score1ViewDisplayState.Value = false;
            Score2ViewDisplayState.Value = false;
            EnemyOrderListDisplayState.Value = false;
        }
        
        public void NextEnemy()
        {
            currentEnemy = EnemyManager.Next();
            _enemyHpPlayer1Side = currentEnemy.Hp;
            _enemyHpPlayer2Side = currentEnemy.Hp;
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
        
        public void AttackPlayer1(int damage)
        {
            if (currentEnemy.Type == EnemyType.Danger)
            {
                UpdateScore(Score1);
                OnDefeat?.Invoke();
                OnDefeatPlayer1?.Invoke();
                OnEscapePlayer1?.Invoke();
                return;
            }
            if (damage > 0) OnHitPlayer1?.Invoke();
            if (currentEnemy.Type == EnemyType.Group)
            {
                UpdateScore(Score1);
                OnDefeatPlayer1?.Invoke();
                return;
            }
            _enemyHpPlayer1Side -= damage;
            if (_enemyHpPlayer1Side <= 0)
            {
                UpdateScore(Score1);
                OnDefeatPlayer1?.Invoke();
                OnEscapePlayer1?.Invoke();
                OnEscapePlayer2?.Invoke();
            }
        }
        
        public void AttackPlayer2(int damage)
        {
            if (currentEnemy.Type == EnemyType.Danger)
            {
                UpdateScore(Score2);
                OnDefeat?.Invoke();
                OnDefeatPlayer2?.Invoke();
                OnEscapePlayer2?.Invoke();
                return;
            }
            if (damage > 0) OnHitPlayer2?.Invoke();
            if (currentEnemy.Type == EnemyType.Group)
            {
                UpdateScore(Score2);
                OnDefeatPlayer2?.Invoke();
                return;
            }
            _enemyHpPlayer2Side -= damage;
            if (_enemyHpPlayer2Side <= 0)
            {
                UpdateScore(Score2);
                OnDefeat?.Invoke();
                OnDefeatPlayer2?.Invoke();
                OnEscapePlayer2?.Invoke();
            }
        }
        
        public void MissPlayer1()
        {
            OnMissPlayer1?.Invoke();
        }
        
        public void MissPlayer2()
        {
            OnMissPlayer2?.Invoke();
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