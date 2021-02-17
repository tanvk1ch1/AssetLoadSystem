using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectS
{
    #region Member
    
    public enum EnemyType
    {
        Normal,
        Guard,
        Group,
        Danger
    }
    
    public enum EnemyColor
    {
        Red,
        None
    }
    
    #endregion
    
    #region EnemyPattern
    
    public interface IEnemy
    {
        float Duration { get; }
        int Point { get; }
        int Hp { get; }
        EnemyType Type { get; }
        EnemyColor Color { get; }
    }
    
    public class EnemyRed : IEnemy
    {
        public float Duration => 3;
        public int Point => 100;
        public int Hp => 1;
        public EnemyType Type => EnemyType.Normal;
        public EnemyColor Color => EnemyColor.Red;
    }

    public class EnemyGuard : IEnemy
    {
        public float Duration => 3;
        public int Point => 300;
        public int Hp => 2;
        public EnemyType Type => EnemyType.Guard;
        public EnemyColor Color => EnemyColor.None;
    }
    
    public class EnemyGroup : IEnemy
    {
        public float Duration => 5;
        public int Point => 2;
        public int Hp => -1;
        public EnemyType Type => EnemyType.Group;
        public EnemyColor Color => EnemyColor.None;
    }
    
    public class EnemyDanger : IEnemy
    {
        public float Duration => 3;
        public int Point => -300;
        public int Hp => 1;
        public EnemyType Type => EnemyType.Danger;
        public EnemyColor Color => EnemyColor.None;
    }
    
    #endregion
    
    #region Class_EnemyManager
    
    public class EnemyManager
    {
        #region Member
        
        private static readonly Dictionary<EnemyType, int> RATIO = new Dictionary<EnemyType, int>()
        {
            {EnemyType.Normal, 14},
            {EnemyType.Guard, 2},
            {EnemyType.Group, 1},
            {EnemyType.Danger, 2},
        };
        
        public Action<IEnemy> OnAddList;
        public Action<IEnemy> OnNext;
        private List<IEnemy> _enemies = new List<IEnemy>();
        private System.Random _random = new System.Random((int) DateTime.Now.Ticks);
        private IEnemy _lastEnemy;
        private Dictionary<EnemyType, int> _ratio = new Dictionary<EnemyType, int>(RATIO);
        
        #endregion
        
        #region Method
        
        public void Init()
        {
            CreateList();
        }
        
        public IEnemy Next()
        {
            var enemy = _enemies[0];
            _enemies.RemoveAt(0);
            CreateList();
            OnNext?.Invoke(enemy);
            return enemy;
        }
        
        private void CreateList()
        {
            while (_enemies.FindAll(enemy => enemy.Type != EnemyType.Danger).Count <5)
            {
                IEnemy[] createdEnemies = CreateEnemyList();
                
                foreach (var createdEnemy in createdEnemies)
                {
                    _enemies.Add(createdEnemy);
                    OnAddList?.Invoke(createdEnemy);
                }
            }
        }
        
        private IEnemy[] CreateEnemyList()
        {
            var _ratio = new Dictionary<EnemyType, int>(this._ratio);
            int[] percent = new int[_ratio.Values.Count];
            _ratio.Values.CopyTo(percent, 0);
            var types = CreateEnemyList(percent);
            IEnemy[] enemy = new IEnemy[types.Length + 1];
            enemy[0] = NewNormalEnemy();
            
            for (int i = 0; i < types.Length; i++)
            {
                switch (types[i])
                {
                    case EnemyType.Normal:
                        enemy[i + 1] = NewNormalEnemy();
                        break;
                    case EnemyType.Guard:
                        enemy[i + 1] = new EnemyGuard();
                        break;
                    case EnemyType.Group:
                        enemy[i + 1] = new EnemyGroup();
                        break;
                    case EnemyType.Danger:
                        enemy[i + 1] = new EnemyDanger();
                        break;
                    default:
                        enemy[i + 1] = NewNormalEnemy();
                        break;
                }
            }
            return enemy;
        }
        
        private IEnemy NewNormalEnemy()
        {
            switch (_random.Next(0,1))
            {
                case 0:
                    return new EnemyRed();
                default:
                    break;
            }
            return new EnemyRed();
        }
        
        private EnemyType[] CreateEnemyList(int[] ratio)
        {
            int[] indexies = CalcIndexes(ratio);
            indexies = indexies.OrderBy(i => Guid.NewGuid()).ToArray();
            var types = Array.ConvertAll(indexies, s => (EnemyType) s);
            return types;
        }
        
        private static int[] CalcIndexes(int[] ratio)
        {
            var gcd = GCD(ratio);
            var list = new List<int>();
            for (int i = 0; i < ratio.Length; i++)
            {
                var count = ratio[i] / gcd;
                for (int j = 0; j < count; j++)
                {
                    list.Add(i);
                }
            }
            return list.ToArray();
        }
        
        private static int GCD(int[] array)
        {
            return array.Aggregate(GCD);
        }
        
        private static int GCD(int temp0, int temp1)
        {
            return temp1 == 0 ? temp0 : GCD(temp1, temp0 / temp1);
        }
        
        #endregion
    }
    
    #endregion
}