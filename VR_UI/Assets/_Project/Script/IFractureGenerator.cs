
using System.Collections.Generic;
using UnityEngine;

public interface IFractureGenerator
{
  
   public CollisionData ColData { get; set; }
   public List<Rigidbody> AllFragment { get; set; }

   public void FractureBody(GameObject obj, Vector3 impactPoint);
   
   public void FractureBody(GameObject obj, Vector3 impactPoint, int numberOfFragment);

   public void FractureBody(Vector3 impactPoint);
   
   public void FractureBody(CollisionData collisionData);
   public void FractureBody();
   
   public void FractureBody(List<Fragment> fragments, Vector3 impactPoint);
   
   public void FractureBody(List<Fragment> fragments);

   public void Explode(CollisionData collisionData);
}
