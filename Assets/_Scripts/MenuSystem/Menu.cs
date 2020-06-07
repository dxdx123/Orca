using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;

public enum MenuState
{
   Closed,
   Showing,
   Opened,
   Closing,
}

public abstract class Menu : MonoBehaviour
{
   public bool DestroyWhenClosed = true;

   public bool DisableMenusUnderneath = false;

   [NonSerialized]
   public MenuState MenuState = MenuState.Closed;
   
   public abstract void OnBackPressed();

   public abstract Canvas GetCanvas();
   public abstract CanvasGroup GetCanvasGroup();

   // Show
   public virtual void PreShowMenu()
   { }
   public abstract IPromise ShowingMenu();
   public virtual void PostShowMenu()
   { }

   // Hide
   public virtual void PreHideMenu()
   { }
   public abstract IPromise HidingMenu();
   public virtual void PostHideMenu()
   { }
}

public abstract class Menu<T> : Menu where T : Menu<T>
{
   public static T Instance { get; private set; }
   
   private Canvas _canvas;
   private CanvasGroup _canvasGroup;

   protected virtual void Awake()
   {
      Instance = this as T;
   }

   protected virtual void OnDestroy()
   {
      Instance = null;
   }

   protected static void Open(Promise<T> promise)
   {
      if (Instance != null)
      {
         Instance.gameObject.SetActive(true);
         
         MenuManager.Instance.OpenMenu(Instance, promise);
      }
      else
      {
         MenuManager.Instance.SpawnInstance<T>()
            .Then(() =>
            {
               MenuManager.Instance.OpenMenu(Instance, promise);
            })
            .Catch(ex => Debug.LogException(ex));
      }
   }

   protected static void Close()
   {
      if (Instance == null)
      {
         Debug.LogErrorFormat("Trying to close menu {0} but Instance is null", typeof(T));
         return;
      }

      MenuManager.Instance.CloseMenu(Instance);
   }

   public override void OnBackPressed()
   {
      Close();
   }
   
   public override Canvas GetCanvas()
   {
      if (_canvas == null)
      {
         _canvas = GetComponent<Canvas>();
      }

      return _canvas;
   }

   public override CanvasGroup GetCanvasGroup()
   {
      if (_canvasGroup == null)
      {
         _canvasGroup = GetComponent<CanvasGroup>();
      }

      return _canvasGroup;
   }
   
   public override IPromise ShowingMenu()
   {
      return Promise.Resolved();
   }

   public override IPromise HidingMenu()
   {
      return Promise.Resolved();
   }
}
