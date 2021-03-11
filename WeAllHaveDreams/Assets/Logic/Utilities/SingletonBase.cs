using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    protected static T Singleton
    {
        get
        {
            if (_Singleton == null)
            {
                _Singleton = GameObject.FindObjectOfType<T>();

                if (_Singleton == null)
                {
                    string errorMessage = string.Format("No active {0} found in scene", typeof(T).ToString());
                    throw new System.Exception(errorMessage);
                }
            }

            return _Singleton;
        }
    }
    private static T _Singleton { get; set; }
}