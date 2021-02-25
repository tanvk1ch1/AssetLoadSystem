using System;
using UnityEngine;

public class InputObserver : MonoBehaviour
{
    public static InputObserver Instance { get; } = new InputObserver();
    public event Action OnAttackLeft;
    public event Action OnAttackRight;
    public event Action<int> OnHitAttack;
    private int _hitValue;
    private int _hit;
    
    public int HitDown
    {
        get
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    OnHitAttack?.Invoke(_hit);
                }
                return 0;
            }
            return 0;
        }
    }
    
    public int GetHit()
    {
        return HitDown;
    }

    public bool CheckKeyDownDecide()
    {
        bool flag = Input.GetKeyDown(KeyCode.A);
        return flag;
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) _hit = 1;
        _hitValue = _hit;
        OnHitAttack?.Invoke(_hit);
    }
    
    public void ClearEvents()
    {
        OnAttackLeft = null;
        OnAttackRight = null;
    }
}
