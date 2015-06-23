using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Endgame
{
	class CustomSelectable : Selectable
	{
		public ListView Owner = null;

		public override Selectable FindSelectableOnUp()
		{
			bool advanceSelection = true;
			if ((Owner != null) && Owner.Initialised)
			{
				advanceSelection = false;

				if ((Owner.Items.Count == 0) || Owner.IsFirstItemSelected)
				{
					advanceSelection = true;
				}
			}

			Selectable result = this;
			if (advanceSelection)
			{
				result = base.FindSelectableOnUp();
			}
			return result;
		}

		public override Selectable FindSelectableOnDown()
		{
			bool advanceSelection = true;
			if ((Owner != null) && Owner.Initialised)
			{
				advanceSelection = false;

				if ((Owner.Items.Count == 0) || Owner.IsLastItemSelected)
				{
					advanceSelection = true;
				}
			}

			Selectable result = this;
			if (advanceSelection)
			{
				result = base.FindSelectableOnDown();
			}
			return result;
		}

		public override void OnSelect(BaseEventData eventData)
		{
			if (this.Owner != null)
			{
				this.Owner.OnSelect(true);
			}
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			if (this.Owner != null)
			{
				this.Owner.OnSelect(false);
			}
		}
	}
}
