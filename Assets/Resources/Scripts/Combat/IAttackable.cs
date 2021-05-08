using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public bool Invulnerable();
    public bool Invulnerable(bool b);
    public void OnAttack(GameObject attacker, Attack attack);
}
