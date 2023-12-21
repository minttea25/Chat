using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
namespace Core
{
    [Serializable]
    public class DBInfo
    {
        public bool IsNew = true;
        public bool AllFound = false;
        public List<string> NotFoundObjects = new List<string>();

    }

    public abstract partial class BaseUI : MonoBehaviour
    {
        /// <summary>
        /// A value for only Editor. Do not use in contents.
        /// </summary>
        [SerializeField]
        public DBInfo DBInfo = new();


        public List<string> AutoAssignUIs()
        {
            FindUI(transform, GetContextFieldsNames());

            return CheckBindUI();
        }

        void FindUI(Transform parent, List<string> names)
        {
            foreach (Transform tf in parent)
            {
                if (names.Contains(tf.name))
                {
                    // NOTE: Context.GetType().GetField() is not null.
                    var property = GetContextFieldValue(tf.name);

                    if (property != null)
                    {
                        var type = property.GetType(); // Property<>

                        var bindObject = type.GetField(Const.BindObjectFieldName);
                        bindObject?.SetValue(property, tf.gameObject);

                        var bindObjectType = type.GetField(Const.BindObjectTypeFieldName);
                        // If CUSTOM type, it does not search the type.
                        if ((ObjectType)bindObjectType.GetValue(property) == ObjectType.CUSTOM) continue;
                        else bindObjectType?.SetValue(property, UIComponentMapper.GetObjectType(tf.gameObject));
                    }
                }

                // recursive call for search all childs
                if (tf.childCount != 0) FindUI(tf, names);
            }
        }

        List<string> CheckBindUI()
        {
            List<string> notFounds = new List<string>();
            var list = GetContextFieldsNames();
            foreach (var name in list)
            {
                var fieldValue = GetContextFieldValue(name);
                GameObject o = fieldValue.GetType().GetField(Const.BindObjectFieldName).GetValue(fieldValue) as GameObject;

                if (o == null)
                {
                    notFounds.Add(name);
                    Debug.LogWarning($"Can not find UI Object name={name}. Check the name in hierarchy.", o);
                }
            }

            return notFounds;
        }

        public bool CheckAll()
        {
            var list = GetContextFieldsNames();
            foreach (var name in list)
            {
                var fieldValue = GetContextFieldValue(name);
                GameObject o = fieldValue.GetType().GetField(Const.BindObjectFieldName).GetValue(fieldValue) as GameObject;

                if (o == null)
                {
                    return false;
                }
            }
            return true;
        }

        public abstract Type GetContextType();
        protected abstract object GetContext();

        protected List<string> GetContextFieldsNames()
        {
            return GetContextType().GetFields().Select(f => f.Name).ToList();
        }
        protected object GetContextFieldValue(string fieldName)
        {
            return GetContextType().GetField(fieldName).GetValue(GetContext());
        }
    }
}
#endif
