using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Script.Utils
{
    public class UIUtil
    {
        Dictionary<Type, UnityEngine.Object[]> objectDictionary = new Dictionary<Type, UnityEngine.Object[]>();

        protected void Bind<T>(GameObject _Object, string[] names) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objs;
            int currentObjsIndex = 0;
            if (objectDictionary.ContainsKey(typeof(T)))
            {
                objs = objectDictionary[typeof(T)];
                currentObjsIndex = objs.Length;
                Array.Resize(ref objs, objs.Length + names.Length);
                objectDictionary[typeof(T)] = objs;
            }
            else
            {
                objs = new UnityEngine.Object[names.Length];
                objectDictionary.Add(typeof(T), objs);
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objs[i + currentObjsIndex] = Util.FindChild(_Object, names[i], true);
                else
                    objs[i + currentObjsIndex] = Util.FindChild<T>(_Object, names[i], true);

                if (objs[i + currentObjsIndex] == null)
                    Debug.LogWarning($"Failed To Bind({names[i]})");
            }
        }
        protected void Bind<T>(GameObject _Object,Type type) where T : UnityEngine.Object  { Bind<T>(_Object, Enum.GetNames(type));  }

        protected void BindAll<T>(GameObject _Object) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objs;
            var components = _Object.GetComponentsInChildren<T>();
            int currentObjectIndex = 0;
            
            if (objectDictionary.ContainsKey(typeof(T)))
            {
                objs = objectDictionary[typeof(T)];
                currentObjectIndex = objs.Length;
                Array.Resize(ref objs, objs.Length + components.Length);
                objectDictionary[typeof(T)] = objs;
            }
            else
            {
                objs = new UnityEngine.Object[components.Length];
                objectDictionary.Add(typeof(T), objs);
            }

            for (int i = 0; i < components.Length; i++)
                objs[i + currentObjectIndex] = components[i];
            
        }
        //protected void Bind<T>(Type type) where T : UnityEngine.Object{ Bind<T>(gameObject, type); }
        //protected void Bind<T>(string[] names) where T : UnityEngine.Object{ Bind<T>(gameObject, names); }

        protected T Get<T>(int index) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objs = null;
            if (objectDictionary.TryGetValue(typeof(T), out objs) == false)  return null;

            return objs[index] as T;
        }
        protected T[] Gets<T>() where T : UnityEngine.Object { return objectDictionary[typeof(T)] as T[];   }

        protected int GetTypeCount(Type type) { return objectDictionary[type].Length; }

        protected Text GetText(int index) { return Get<Text>(index); }
        protected Button GetButton(int index) { return Get<Button>(index); }
        protected Image GetImage(int index) { return Get<Image>(index); }
        protected InputField GetInputFiled(int index) { return Get<InputField>(index); }
    }
}