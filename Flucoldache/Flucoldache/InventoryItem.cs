using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache
{
	public enum InventoryItemType
	{
		Default,
		Spendable,
		Usable
	}

	public class InventoryItem
	{
		public string Token;
		public string Name;
		public string Description;
		public int Stack;
		public int Amount;
		public InventoryItemType Type;

		public InventoryItem() {}

		public InventoryItem(InventoryItem item)
		{
			Token = item.Token;
			Name = item.Name;
			Description = item.Description;
			Stack = item.Stack;
			Amount = item.Amount;
			Type = item.Type;
		}
	}
}
