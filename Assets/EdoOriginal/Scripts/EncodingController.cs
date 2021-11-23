using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EncodingController : MonoBehaviour
{
    private static EncodingController instance;
    [SerializeField] private EncodingType encodingType = EncodingType.UTF8;

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }

    public static EncodingController Instance
    {
        get
        {
            return instance;
        }
    }

    public EncodingType EncodingType
    {
        get
        {
            return encodingType;
        }
        set
        {
            encodingType = value;
        }
    }
}
