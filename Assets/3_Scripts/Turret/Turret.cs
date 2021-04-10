using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

	private Transform target;
	private Enemy targetEnemy;

	[Header("General")]

	public float range = 15f;

	[Header("Use Bullets (default)")]
	public GameObject bulletPrefab;
	public float fireRate = 1f;
	private float fireCountdown = 0f;

	[Header("Use Laser")]
	public bool useLaser = false;

	public float damageOverTime = 1f;

	public LineRenderer lineRenderer;
	//public ParticleSystem impactEffect;
	//public Light impactLight;

	[Header("Turret Setup")]

	public string enemyTag = "Enemy";

	public Transform partToRotate;
	public float turnSpeed = 10f;

	public Transform firePoint;

	// Use this for initialization
	void Start()
	{
		lineRenderer.enabled = false;
		InvokeRepeating("UpdateTarget", 0f, 0.5f);
	}

	void UpdateTarget()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
		
		float shortestDistance = Mathf.Infinity;
		GameObject nearestEnemy = null;
		foreach (GameObject enemy in enemies)
		{
			float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
			if (distanceToEnemy < shortestDistance)
			{
				shortestDistance = distanceToEnemy;
				nearestEnemy = enemy;
			}
		}

		if (nearestEnemy != null && shortestDistance <= range)
		{
			lineRenderer.enabled = true;
			target = nearestEnemy.transform;
			targetEnemy = nearestEnemy.GetComponent<Enemy>();
		}
		else
		{
			lineRenderer.enabled = false;
			target = null;
		}

	}

	// Update is called once per frame
	void Update()
	{
		if (target == null)
		{
			if (useLaser)
			{
				if (lineRenderer.enabled)
				{
					lineRenderer.enabled = false;
					//impactEffect.Stop();
					//impactLight.enabled = false;
				}
			}

			return;
		}

		LockOnTarget();

		if (useLaser)
		{
			Laser();
		}
		else
		{
			if (fireCountdown <= 0f)
			{
				Shoot();
				fireCountdown = 1f / fireRate;
			}

			fireCountdown -= Time.deltaTime;
		}

	}

	void LockOnTarget()
	{
		Vector3 dir = target.position - transform.position;
		Quaternion lookRotation = Quaternion.LookRotation(dir);
		Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
		partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
	}

	void Laser()
	{
		targetEnemy.TakeDoTDamage(damageOverTime);
		//targetEnemy.Slow(slowAmount);

		//if (!lineRenderer.enabled)
		//{
		//	lineRenderer.enabled = true;
		//	impactEffect.Play();
		//	impactLight.enabled = true;
		//}

		lineRenderer.SetPosition(0, new Vector3(firePoint.position.x, firePoint.position.y - 2f, firePoint.position.z - 1.75f));
		lineRenderer.SetPosition(1, new Vector3(target.position.x, target.position.y - 2f, target.position.z));

		Vector3 dir = firePoint.position - target.position;

		//impactEffect.transform.position = target.position + dir.normalized;

		//impactEffect.transform.rotation = Quaternion.LookRotation(dir);
	}

	void Shoot()
	{
		GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
		Bullet bullet = bulletGO.GetComponent<Bullet>();

		if (bullet != null)
			bullet.Seek(target);
	}
}
