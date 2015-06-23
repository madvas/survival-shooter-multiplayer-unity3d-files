using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Endgame
{
	public class CustomButton : Button
	{
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);

			if (this.onPointerDown != null)
			{
				this.onPointerDown.Invoke();
			}
		}

		public PointerDownEvent onPointerDown { get; set; }

		[Serializable]
		public class PointerDownEvent : UnityEvent
		{
			public PointerDownEvent()
			{
			}
		}
	}
}
