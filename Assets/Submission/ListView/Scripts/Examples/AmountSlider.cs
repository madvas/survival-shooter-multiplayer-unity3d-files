namespace Examples
{
	using UnityEngine;
	using System.Collections;
	using UnityEngine.UI;
	using Endgame;

	public class AmountSlider : MonoBehaviour
	{
		public Text AmountLabel;
		public Scrollbar AmountScrollBar;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		public void OnScrollValueChanged(float value)
		{
			int amount = (int)(this.AmountScrollBar.value * 10);
			this.AmountLabel.text = amount.ToString();
		}
	}
}
