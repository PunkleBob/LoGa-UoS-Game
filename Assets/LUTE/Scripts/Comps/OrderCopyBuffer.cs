using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//temporary class to hold the order copy buffer for the order copy paste feature
[AddComponentMenu("")]
public class OrderCopyBuffer : Node
{
    public static OrderCopyBuffer instance;

    protected virtual void Start()
    {
        if (Application.isPlaying)
            Destroy(gameObject);
    }

    //returns the instance of the order copy buffer
    //will create a new gameobject if none exists
    public static OrderCopyBuffer GetInstance()
    {
        if (instance == null)
        {
            //static variables are not serialized, so we need to find the instance (e.g. playing in editor mode)
            GameObject go = GameObject.Find("OrderCopyBuffer");
            if (go == null)
            {
                go = new GameObject("OrderCopyBuffer");
                go.hideFlags = HideFlags.HideAndDontSave;
            }

            instance = go.GetComponent<OrderCopyBuffer>();
            if (instance == null)
            {
                instance = go.AddComponent<OrderCopyBuffer>();
            }
        }
        return instance;
    }

    public virtual bool HasOrders()
    {
        return GetOrders().Length > 0;
    }

    public virtual Order[] GetOrders()
    {
        return GetComponents<Order>();
    }

    public virtual void Clear()
    {
        Order[] orders = GetOrders();
        for (int i = 0; i < orders.Length; i++)
        {
            DestroyImmediate(orders[i]);
        }
    }
}
