using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache
{
	public class InventoryItem
	{
		public string Token;
		public string Name;
		public string Description;
		public int Stack;
		public int Amount;
		public bool Spendable;

		public InventoryItem() {}

		public InventoryItem(InventoryItem item)
		{
			Token = item.Token;
			Name = item.Name;
			Description = item.Description;
			Stack = item.Stack;
			Amount = item.Amount;
			Spendable = item.Spendable;
		}
	}
}
