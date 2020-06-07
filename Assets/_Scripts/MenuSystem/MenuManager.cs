using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;
using UnityEngine.Assertions;

using Object = UnityEngine.Object;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;

    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuManager>();
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }
    }
    
    private readonly Stack<Menu> _menuStack = new Stack<Menu>();
    private Transform _uiTransform;
    
    private readonly PromiseTimer _promiseTimer = new PromiseTimer();

    public PromiseTimer GetPromiseTimer()
    {
        return _promiseTimer;
    }

    private Dictionary<Type, string> _assetDict = new Dictionary<Type, string>()
    {
        {typeof(TestMenu), "Assets/_Experiment/TestMenu.prefab"},
    };
    
    private string GetAssetPath<T>() where T : Menu<T>
    {
        Type type = typeof(T);

        string path;
        if(_assetDict.TryGetValue(type, out path))
        {
            return path;
        }
        else
        {
            throw new Exception($"!!! Fail to get path Type: {typeof(T)}");
        }
    }

    // ================================ Utils ============================
    public IPromise SpawnInstance<T>() where T : Menu<T>
    {
        var promise = new Promise();

        string path = GetAssetPath<T>();
        ResourceManager.Instance.GetAsset<GameObject>(path, this)
            .Then(asset =>
            {
                Instantiate(asset, GetUserInterfaceRoot());
                promise.Resolve();
            })
            .Catch(ex => promise.Reject(ex));

        return promise;
    }

    private Transform GetUserInterfaceRoot()
    {
        if (_uiTransform == null)
        {
            _uiTransform = GameObject.Find("Canvas").GetComponent<Transform>();
            Assert.IsNotNull(_uiTransform);
        }

        return _uiTransform;
    }

    // ================================ Utils ============================

    public void OpenMenu<T>(Menu instance, Promise<T> promise) where T : Menu<T>
    {
        if (instance.MenuState == MenuState.Closed)
        {
            OpenMenuInternal(instance, promise);
        }
        else if(instance.MenuState == MenuState.Closing)
        {
            _promiseTimer.WaitUntil(time => instance.MenuState == MenuState.Closed)
                .Then(() =>
                {
                    OpenMenuInternal(instance, promise);
                })
                .Catch(ex => Debug.LogException(ex));
        }
        else
        {
            throw new Exception($"OpenMenu {instance.MenuState}");
        }
    }

    private void OpenMenuInternal<T>(Menu instance, Promise<T> promise) where T : Menu<T>
    {
        // resolve initialize promise
        promise?.Resolve(instance as T);
        
        instance.GetCanvasGroup().interactable = true;
        
        instance.PreShowMenu();
        instance.MenuState = MenuState.Showing;

        if (_menuStack.Count > 0)
        {
            if (instance.DisableMenusUnderneath)
            {
                foreach (var menu in _menuStack)
                {
                    menu.gameObject.SetActive(false);

                    if (menu.DisableMenusUnderneath)
                        break;
                }
            }

            var topCanvas = instance.GetCanvas();
            var previousCanvas = _menuStack.Peek().GetCanvas();

            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }

        _menuStack.Push(instance);

        // effect
        instance.gameObject.SetActive(true);
        instance.ShowingMenu()
            .Then(() =>
            {
                if (instance.MenuState == MenuState.Showing)
                {
                    instance.MenuState = MenuState.Opened;
                    instance.PostShowMenu();
                }
                else
                {
                    throw new Exception($"PostShowMenu while {instance.MenuState}");
                }
            })
            .Catch(ex => Debug.LogException(ex));
    }

    public void CloseMenu(Menu menu)
    {
        if (_menuStack.Count == 0)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
            return;
        }

        if (_menuStack.Peek() != menu)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
            return;
        }

        CloseTopMenu();
    }

    private void CloseTopMenu()
    {
        var instance = _menuStack.Peek();

        if (instance.MenuState == MenuState.Opened)
        {
            CloseTopMenuInternal();
        }
        else if(instance.MenuState == MenuState.Showing)
        {
            _promiseTimer.WaitUntil(time => instance.MenuState == MenuState.Opened)
                .Then(() =>
                {
                    CloseTopMenuInternal();
                })
                .Catch(ex => Debug.LogException(ex));
        }
        else
        {
            throw new Exception($"CloseTopMenu {instance.MenuState}");
        }
    }

    private void CloseTopMenuInternal()
    {
        var instance = _menuStack.Pop();
        instance.GetCanvasGroup().interactable = false;

        instance.PreHideMenu();
        instance.MenuState = MenuState.Closing;

        // effect
        instance.HidingMenu()
            .Then(() =>
            {
                if (instance.MenuState == MenuState.Closing)
                {
                    if (instance.DestroyWhenClosed)
                        Destroy(instance.gameObject);
                    else
                        instance.gameObject.SetActive(false);

                    instance.PostHideMenu();
                    instance.MenuState = MenuState.Closed;
                }
                else
                {
                    throw new Exception($"PostHideMenu while {instance.MenuState}");
                }
            })
            .Catch(ex => Debug.LogException(ex));

        // Re-activate top menu
        // If a re-activated menu is an overlay we need to activate the menu under it
        foreach (var menu in _menuStack)
        {
            menu.gameObject.SetActive(true);

            if (menu.DisableMenusUnderneath)
                break;
        }
    }

    private void Update()
    {
        _promiseTimer.Update(Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.Escape) && _menuStack.Count > 0)
        {
            _menuStack.Peek().OnBackPressed();
        }
    }
}
