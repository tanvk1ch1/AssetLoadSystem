namespace ProjectS
{
    public class EnemyConstants
    {
        #region Enum
        
        public enum Type
        {
            // TODO:バリエーションを増やしたいが、モデルの準備ができてからにする
            Normal = 0,
        }
        
        public enum State
        {
            None,
            Wait,
            Move,
            Attack,
            Shield,
            WaitAttack,
            Damage,
            Defeat
        }
        
        #endregion
        
        #region Member_SetPos
        
        public const float EnemyPosX = 0;
        public const float EnemyStartBreakPosZ = -5.5f;
        public const float EnemyStartStopPosZ = -5.0f;
        public const float EnemyEscapePosZ = 8.0f;
        
        #endregion
        
        #region Member_AppearTime
        
        public const float EnemyAppearAnimationTime = 0.3f;
        public const float EnemyAppearAnimationBackTime = 0.2f;
        public const float EnemyEscapeAnimationTime = 0.2f;
        public const float EnemyLeaveAnimationTime = 0.3f;
        
        #endregion
        
        #region Member_Destroy
        
        public const float EnemyDefeatPosX = EnemyPosX;
        public const float EnemyDefeatPosY = 4.5f;
        public const float EnemyDefeatPosZ = 15.0f;
        public const float EnemyDefeatRotateX = -1.0f;
        public const float EnemyDefeatRotateY = 1.0f;
        public const float EnemyDefeatRotateZ = 1.0f;
        
        #endregion
        
        #region Member_Attack
        
        public const float EnemyAttackChargeEffectTime = 2.0f;
        public const float EnemyAttackEffectTime = 0.5f;
        
        #endregion
        
        #region Member_View_DisplayPos
        
        public const float EnemyHpViewPosY = 18.0f;
        
        #endregion
        
        
    }
}