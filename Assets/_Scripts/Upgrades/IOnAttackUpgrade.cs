using UnityEngine;

public interface IOnAttackUpgrade: IBlockUpgrade
{
    public int onAttackPriority { get; set; } //I do not know if this is strickly necessary

    public void GetTargets();

    public void Attack();


}
