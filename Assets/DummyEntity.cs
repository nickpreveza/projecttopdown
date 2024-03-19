using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEntity : Entity
{
    public override void Damage(int amount)
    {
        visuals.VisualFeedbackHit(false);
    }

    private void Update()
    {
        HandleFreeze();
    }
}
