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
        public ValueObserver<int> HitPowerRight { get; } = new ValueObserver<int>(0);
        public ValueObserver<int> HitPowerLeft { get; } = new ValueObserver<int>(0);
        public EnemyManager EnemyManager { get; } = new EnemyManager();
        public IEnemy currentEnemy { get; private set; }
        public int PlayerNum { get; private set; }
        public ICPU_ShootingGame CPU { get; }
        
        public Action OnDefeatLeft;
        public Action OnDefeatRight;
        public Action OnEscapeLeft;
        public Action OnEscapeRight;
        public Action OnDefeat;
        public Action OnEscapeEnemy;
        public Action OnEnemyAppeared;
        public Action OnFinishedEscapeEnemy;
        
        public Action OnHitLeft;
        public Action OnHitRight;
        public Action OnHitGreatLeft;
        public Action OnHitGreatRight;
        public Action OnMissLeft;
        public Action OnMissRight;
        public Action OnMissHitLeft;
        public Action OnMissHitRight;
        public Action OnLoadPrefabs;
        
        private int _enemyHpLeftSide;
        private int _enemyHpRightSide;
        
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
            _enemyHpLeftSide = currentEnemy.Hp;
            _enemyHpRightSide = currentEnemy.Hp;
        }
        
        public void UpdateIsHitRight(bool isAttack)
        {
            if (isAttack) HitPowerRight.Value = 1;
            else HitPowerRight.Value = 0;
        }
        
        public void UpdateIsHitLeft(bool isAttack)
        {
            if (isAttack) HitPowerLeft.Value = 1;
            else HitPowerLeft.Value = 0;
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
        
        public void AttackLeft(int damage)
        {
            if (currentEnemy.Type == EnemyType.Danger)
            {
                UpdateScore(Score1);
                OnDefeat?.Invoke();
                OnDefeatLeft?.Invoke();
                OnEscapeLeft?.Invoke();
                return;
            }
            if (damage > 0) OnHitLeft?.Invoke();
            if (currentEnemy.Type == EnemyType.Group)
            {
                UpdateScore(Score1);
                OnDefeatLeft?.Invoke();
                return;
            }
            _enemyHpLeftSide -= damage;
            if (_enemyHpLeftSide <= 0)
            {
                UpdateScore(Score1);
                OnDefeat?.Invoke();
                OnDefeatLeft?.Invoke();
                OnEscapeRight?.Invoke();
            }
        }
        
        public void AttackRight(int damage)
        {
            if (currentEnemy.Type == EnemyType.Danger)
            {
                OnMissHitRight?.Invoke();
                UpdateScore(Score2);
                OnDefeat?.Invoke();
                OnDefeatRight?.Invoke();
                OnEscapeLeft?.Invoke();
                return;
            }
            if (damage > 0) OnHitRight?.Invoke();
            if (currentEnemy.Type == EnemyType.Group)
            {
                UpdateScore(Score2);
                OnDefeatRight?.Invoke();
                return;
            }
            _enemyHpRightSide -= damage;
            if (_enemyHpRightSide <= 0)
            {
                UpdateScore(Score2);
                OnDefeat?.Invoke();
                OnDefeatRight?.Invoke();
                OnEscapeLeft?.Invoke();
            }
        }
        
        public void MissLeft()
        {
            OnMissLeft?.Invoke();
        }
        
        public void MissRight()
        {
            OnMissRight?.Invoke();
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