using System;
using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Assertions;

public class TouchEntity : MonoBehaviour
{
    private TapGesture _tapGesture;
    
    private void Awake()
    {
        _tapGesture = GetComponent<TapGesture>();
    }

    private void OnEnable()
    {
        _tapGesture.Tapped += OnTapped;
    }

    private void OnTapped(object sender, EventArgs e)
    {
        var entity = gameObject.GetEntityLink().entity as GameEntity;
        Assert.IsNotNull(entity);

        entity.isTouch = true;
    }

    private void OnDisable()
    {
        _tapGesture.Tapped -= OnTapped;
    }
}
