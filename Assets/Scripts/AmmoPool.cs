using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPool {

	//----------------------------------------------------------------------------------------------------------

	private Dictionary<string, int> pools;

	//----------------------------------------------------------------------------------------------------------

	public AmmoPool() {
		pools = new Dictionary<string, int>();
	}

	//----------------------------------------------------------------------------------------------------------

	public int Get(string type) {
		if(!pools.ContainsKey(type)) return 0;
		return pools[type];
	}

	//----------------------------------------------------------------------------------------------------------

	public int Take(string type, int desiredAmount) {
		if(!pools.ContainsKey(type)) return 0;

		int amount = pools[type] < desiredAmount ? pools[type] : desiredAmount;
		pools[type] -= amount;
		return amount;
	}

	//----------------------------------------------------------------------------------------------------------

	public int Add(string type, int desiredAmount) {
		if(!pools.ContainsKey(type)) {
			pools.Add(type, desiredAmount);
		} else {
			pools[type] += desiredAmount;
		}

		return 0;
	}

	//----------------------------------------------------------------------------------------------------------

	public int Set(string type, int desiredAmount) {
		if(!pools.ContainsKey(type)) { 
			pools.Add(type, desiredAmount);
		} else {
			pools[type] = desiredAmount;
		}

		return 0;
	}

	//----------------------------------------------------------------------------------------------------------

}