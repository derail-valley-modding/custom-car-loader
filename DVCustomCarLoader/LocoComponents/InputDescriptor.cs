using System;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
	public class InputDescriptor
	{
		public readonly KeyCode[] IncreaseKeys;
		public readonly KeyCode[] DecreaseKeys;
		public readonly float MaxChangeSpeed;

		public readonly Func<float> GetValueFunc;
		public readonly Action<float> SetValueFunc;

		public readonly float Min;
		public readonly float Max;

		public float CurrentChangeSpeed = 0;
		public float CurrentChangeDerivative = 0;

		public InputDescriptor(KeyCode[] increaseKeys, KeyCode[] decreaseKeys, float maxChangeSpeed,
			Func<float> getValueFunc, Action<float> setValueFunc,
			float min = 0, float max = 1)
		{
			IncreaseKeys = increaseKeys;
			DecreaseKeys = decreaseKeys;
			MaxChangeSpeed = maxChangeSpeed;

			GetValueFunc = getValueFunc;
			SetValueFunc = setValueFunc;

			Min = min;
			Max = max;
		}
	}
}