using System;
using UnityEngine;

public class InputObserver : MonoBehaviour
{
    public static InputObserver Instance { get; } = new InputObserver();
    public event Action OnAttackLeft;
    public event Action OnAttackRight;
    // public event Action<int> OnHitAttack;
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
                    Debug.Log("Aキーを押した：1");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
    }
    
    public bool IsAttackRight
    {
        get
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
    
    public bool IsAttackLeft
    {
        get
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
    
    public bool CheckKeyDownDecide()
    {
        bool flag = Input.GetKeyDown(KeyCode.A);
        // Debug.Log("Aキーを押すまでConsoleに流れ続ける：3");
        return flag;
    }
    
    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         _hit = 1;
    //         Debug.Log("Aキーを押した：2");
    //     }
    //     _hitValue = _hit;
    //     OnHitAttack?.Invoke(_hit);
    //     
    // }
    
    public void ClearEvents()
    {
        OnAttackLeft = null;
        OnAttackRight = null;
    }
}
