namespace ProjectS
{
    public interface ICPU_ShootingGame
    {
        int MissRate { get; }
        int MissRateDanger { get; }
        int MissRateGuard { get; }
        float AttackTime { get; }
        float AttackOffset { get; }
    }
    
    public class CPUEasy : ICPU_ShootingGame
    {
        public int MissRate => 70;
        public int MissRateDanger => 45;
        public int MissRateGuard => 30;
        public float AttackTime => 2.5f;
        public float AttackOffset => 0.5f;
    }
    
    public class CPUNormal : ICPU_ShootingGame
    {
        public int MissRate => 40;
        public int MissRateDanger => 23;
        public int MissRateGuard => 70;
        public float AttackTime => 2.5f;
        public float AttackOffset => 0.5f;
    }
    
    public class CPUHard : ICPU_ShootingGame
    {
        public int MissRate => 25;
        public int MissRateDanger => 13;
        public int MissRateGuard => 80;
        public float AttackTime => 2.5f;
        public float AttackOffset => 0.5f;
    }
}