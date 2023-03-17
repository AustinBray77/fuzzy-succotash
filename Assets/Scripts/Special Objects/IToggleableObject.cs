using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IToggleableObject
{
    public bool Activated { get; }

    void Activate();
    void Deactivate();
}
