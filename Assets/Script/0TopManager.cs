using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class will grab all other managers and initialize them at the beginning of the program
/// </summary>
public class ManagerManager : MonoBehaviour
{
    void Awake()
    {
        foreach (var manager in FindObjectsOfType<Manager>())
        {
            manager.ManagerInit();
        }
    }
}

/// <summary>
/// Inherit from this if you want an early init for DATA MANAGERS ONLY
/// Do NOT inherit game systems here, use AWAKE
/// Only data managers!
/// </summary>
public abstract class Manager : MonoBehaviour
{
    public abstract void ManagerInit();
}