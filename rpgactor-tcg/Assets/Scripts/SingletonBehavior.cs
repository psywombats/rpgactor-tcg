using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
  private static T instance;
  private static bool creating;

  public static T Instance
  {
    get
    {
      if (instance == null)
      {
        creating = true;
        var obj = new GameObject( typeof( T ).Name );
        if (!Application.IsPlaying(obj))
        {
          DestroyImmediate(obj);
          throw new System.Exception("For runtime only");
        }
        instance = obj.AddComponent<T>();
        DontDestroyOnLoad( obj );
        instance.Init();
        creating = false;
      }
      return instance;
    }
  }

  protected virtual T GetThis()
  {
    if (this is T that)
    {
      return that;
    }
    else
    {
      throw new System.NotImplementedException();
    }
  }

  protected void Awake()
  {
    if (!creating)
    {
      if (instance == null || !instance)
      {
        instance = GetThis();
      }
      else
      {
        HandleConflictingSingleton();
      }
    }
  }

  protected virtual void Init() {}

  /// <summary>
  /// Called on this object when it's instantiated, but there's already an instance around
  /// </summary>
  protected virtual void HandleConflictingSingleton()
  {
    var old = instance;

    var oldScene = old.gameObject.scene;

    if (oldScene.name == "DontDestroyOnLoad")
    {
      //prefer old instance
      Destroy( gameObject );
      return;
    }

    if (!oldScene.IsValid() || !oldScene.isLoaded || oldScene.name == gameObject.scene.name)
    {
      //prefer the new instance
      instance = GetThis();
      Destroy(this);
    }
    else
    {
      //prefer the old instance
      Destroy(gameObject);
    }
  }

  protected virtual void OnDestroy()
  {
    if (instance == GetThis())
    {
      instance = null;
    }
  }
}
