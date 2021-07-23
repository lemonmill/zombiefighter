using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UniRx;
using UnityEngine;

public class DespawnAfter : MonoBehaviour
{
    public float timer = 3f;
    
    private void OnEnable()
    {
        Observable.Timer(TimeSpan.FromSeconds(timer)).Subscribe(_ => Despawn());
    }

    private void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }
}
