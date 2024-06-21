using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Wolf : MonoBehaviour
{
	public float bodySize = 1;
	public float maxHunger = 100;
	public float currentHunger = 100;
	public float hungerDrainSpeed = 1;
	public float eatingSpeed = 1f;
	public float stunTimer = 0;
	public float visionRadius = 10f;

	public bool GracePeriod = false;
	public SpriteRenderer sprite;
	public float slowTimer = 0;
	public float slowDuration;
	public bool isSlowed = false;

	public float eatTime, foodValue, effectValue;
	public string effect;

	public Collider visionCollider;
	public List<IConsumable> foodInRange = new List<IConsumable>();

	private float timeSinceLastIncrease = 0f;

	private void Update()
	{
		timeSinceLastIncrease += Time.deltaTime;

		if (timeSinceLastIncrease >= 60f)
		{
			IncreaseEatingSpeed();
			timeSinceLastIncrease = 0f;
		}

		if(slowTimer >= slowDuration)
		{
			isSlowed = false;
			sprite.color = Color.white;
		}
		else
		{
			slowTimer += 1 * Time.deltaTime;
		}
	}

	private void IncreaseEatingSpeed()
	{
		eatingSpeed += 0.1f;
	}

	public void AddHunger(float foodValue)
	{
		if (currentHunger < maxHunger)
		{
			currentHunger += foodValue;
		}
	}

	public void UpdateSpeed(float speed)
	{
		hungerDrainSpeed = speed;
	}

	public void Slowed()
	{
		sprite.color = Color.magenta;
		slowTimer = 0;
		slowDuration = effectValue;
		isSlowed = true;
	}


}
