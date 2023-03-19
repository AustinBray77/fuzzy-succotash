using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IToggleableObject
{
    bool Activated { get; }

    void Activate();
    void Deactivate();
}
