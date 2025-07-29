using UnityEngine;

public interface IOnHitUpgrade:IBlockUpgrade
{

    public int onHitPriority { get; set; }

    public void Hit();



}
