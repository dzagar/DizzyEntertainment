using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyWeaponDefinition {
	public WeaponType   type = WeaponType.blaster;
	public GameObject   projectilePrefab;          // Prefab for projectiles
	public Color        projectileColor = Color.red;
	public float        damageOnHit = 0;           // Amount of damage caused
	public float        delayBetweenShots = 0;
	public float        velocity = 20;             // Speed of projectiles
}

public class EnemyShoot : MonoBehaviour {
	static public Transform   ENEMY_PROJECTILE_ANCHOR;

	public EnemyWeaponDefinition enemyDef;
	public GameObject cockpit;
	public float shootChance = 0.5f;
	public float lastShot; // Time last shot was fired

	void Start(){
		if (ENEMY_PROJECTILE_ANCHOR == null) {
			GameObject go = new GameObject("_Projectile_Anchor");
			ENEMY_PROJECTILE_ANCHOR = go.transform;
		}
		InvokeRepeating ("ChanceToShoot", 1, 1);
	}

	void ChanceToShoot(){
		if (Random.value < shootChance) {
			Fire ();
		}
	}
	
	public void Fire() {
		// If this.gameObject is inactive, return
		if (!gameObject.activeInHierarchy) return;
		// If it hasn't been enough time between shots, return
		if (Time.time - lastShot < enemyDef.delayBetweenShots) {
			return;
		}
		Projectile p;
		p = MakeProjectile ();
		p.GetComponent<Rigidbody> ().velocity = Vector3.down * enemyDef.velocity;
		cockpit.GetComponent<AudioSource> ().Play ();
	}

	public Projectile MakeProjectile() {
		GameObject go = Instantiate( enemyDef.projectilePrefab ) as GameObject;
		if ( transform.parent.gameObject.tag == "Enemy" ) {
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer("ProjectileEnemy");
		} else {
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer("ProjectileHero");
		}
		go.transform.position = cockpit.transform.position;
		go.transform.parent = ENEMY_PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile>();
		p.GetComponent<Renderer> ().material.color = enemyDef.projectileColor;
		lastShot = Time.time;
		return( p );

	}
}
