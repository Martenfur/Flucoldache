using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Flucoldache
{
	public class Inventory : GameObj
	{
		string _invItemsFile = "Resources/InventoryItems.xml";
		Vector2 _listSize;
		Vector2 _listPos;
		
		/// <summary>
		/// Contatins samples of each inventory item.
		/// </summary>
		Dictionary<string, InventoryItem> _itemPool;

		Dictionary<string, InventoryItem> Items = new Dictionary<string, InventoryItem>();
		Dictionary<string, InventoryItem> Potions= new Dictionary<string, InventoryItem>();
		
		SelectionMenu _currentSelectionMenu = null;
		Dictionary<string, InventoryItem> _currentInventory = null;

		delegate void Use(InventoryItem item);

		Dictionary<string, Use> _itemActions = new Dictionary<string, Use>();

		public Inventory()
		{
			LoadItems();
			_listSize = new Vector2(32, GameConsole.H - Dialogue.Size.Y - 4);
			_listPos = new Vector2(GameConsole.W - _listSize.X - 1, 1);

			_itemActions.Add("testitem", ItemTestAction);
			_itemActions.Add("fox", ItemFoxAction);

		}

		
		public override void Update()
		{
			if (_currentSelectionMenu != null)
			{
				if (_currentSelectionMenu.Activated)
				{
					if (_currentSelectionMenu.SelectedItem >= _currentInventory.Count)
					{
						// Back button.
						
						Objects.Destroy(_currentSelectionMenu);
						_currentSelectionMenu = null;
						_currentInventory = null;
						// Back button.
					}
					else
					{
						// Item.
						InventoryItem item =  _currentInventory.ElementAt(_currentSelectionMenu.SelectedItem).Value;
						bool invoke = true;

						if (item.Type == InventoryItemType.Spendable)
						{
							item.Amount -= 1;
							
							if (_currentInventory == Items)
							{
								if (item.Amount <= 0)
								{
									_currentInventory.Remove(item.Token);
								}
							}
							else
							{
								if (item.Amount < 0)
								{
									item.Amount = 0;
									invoke = false;
									_currentSelectionMenu.Activated = false;
								}
							}
							
						}

						

						
						if (invoke)
						{
							Objects.Destroy(_currentSelectionMenu);
							_currentSelectionMenu = null;
							_currentInventory = null;
							_itemActions[item.Token].Invoke(item);
						}
						// Item.
					}
				}
			}
		}
		

		public override void Draw()
		{
			if (_currentSelectionMenu != null)
			{
				GameConsole.BackgroundColor = Color.Black;
				GameConsole.ForegroundColor = Color.Gray;
				GameConsole.DrawRectangle(Dialogue.Pos, Dialogue.Size);
				GameConsole.DrawFrame(Dialogue.Pos - Vector2.One, Dialogue.Size + Vector2.One * 2);
				
				string desc;

				if (_currentSelectionMenu.SelectedItem >= _currentInventory.Count)
				{
					desc = "Вернуться назад.";
				}
				else
				{
					desc = _currentInventory.ElementAt(_currentSelectionMenu.SelectedItem).Value.Description;
				}

				GameConsole.DrawText(desc, Dialogue.Pos);
			}
		}


		/// <summary>
		/// Loads item info from data file.
		/// </summary>
		private void LoadItems()
		{
			_itemPool = new Dictionary<string, InventoryItem>();

			XmlDocument xml = new XmlDocument();
			xml.Load(_invItemsFile);
			
			XmlNodeList nodes = xml.DocumentElement.SelectNodes("item");
			

			foreach(XmlNode node in nodes)
			{
				InventoryItem item = new InventoryItem();
				
				item.Token = node.SelectSingleNode("token").FirstChild.Value;
				
				item.Name = node.SelectSingleNode("name").FirstChild.Value;
				switch(node.SelectSingleNode("type").FirstChild.Value)
				{
					case "default":
						item.Type = InventoryItemType.Default;
						break;
					case "spendable":
						item.Type = InventoryItemType.Spendable;
						break;
					case "usable":
						item.Type = InventoryItemType.Usable;
						break;
				}

				item.Stack = Int32.Parse(node.SelectSingleNode("stack").FirstChild.Value);
				item.Description = node.SelectSingleNode("description").FirstChild.Value.Replace("\t", "");
				if (item.Description.StartsWith(Environment.NewLine))
				{
					item.Description = item.Description.Remove(0, Environment.NewLine.Length);
				}

				_itemPool.Add(item.Token, item);
				Debug.WriteLine("Desc:" + item.Description);
			}
		}
	

		public void AddItem(string token, int amount)
		{
			AddToInventory(Items, token, amount);
		}


		public void AddPotion(string token, int amount)
		{
			AddToInventory(Potions, token, amount);
		}


		private void AddToInventory(Dictionary<string, InventoryItem> inv, string token, int amount)
		{
			if (_itemPool.ContainsKey(token))
			{
				if (inv.ContainsKey(token))
				{
					inv[token].Amount += amount;
				}
				else
				{
					InventoryItem item = new InventoryItem(_itemPool[token]);
					item.Amount = amount;
					inv.Add(token, item);
				}
				// Check for maximum capacity.
				if (inv[token].Amount > inv[token].Stack)
				{
					inv[token].Amount = inv[token].Stack;
				}
				// Check for maximum capacity.
			}
		}

		

		public void ShowItems()
		{
			if (_currentSelectionMenu == null)
			{
				string[] menuItems = new string[Items.Count + 1];
				var i = 0;
				foreach(KeyValuePair<string, InventoryItem> item in Items)
				{
					menuItems[i] = item.Value.Name;
					if (item.Value.Amount > 1)
					{
						menuItems[i] += " x" + item.Value.Amount;
					}
					i += 1;
				}
				menuItems[i] = "Назад";

				_currentSelectionMenu = new SelectionMenu("Инвентарь", menuItems, _listPos, _listSize);
				_currentInventory = Items;
			}
		}

		public void ShowPotions()
		{
			if (_currentSelectionMenu == null)
			{
				string[] menuItems = new string[Potions.Count + 1];
				var i = 0;
				foreach(KeyValuePair<string, InventoryItem> item in Potions)
				{
					menuItems[i] = item.Value.Name;
					menuItems[i] += " (" + item.Value.Amount + "/" + item.Value.Stack + ")";
					
					i += 1;
				}
				menuItems[i] = "Назад";

				_currentSelectionMenu = new SelectionMenu("Зелья", menuItems, _listPos, _listSize);
				_currentInventory = Potions;
			}
		}


		#region Item actions

		void ItemFoxAction(InventoryItem item)
		{
			Debug.WriteLine("Welp. Got a " + item.Token);
			//ShowPotions();
		}

		void ItemTestAction(InventoryItem item)
		{
			Debug.WriteLine("LE TEST!");
		}

		#endregion Item actions

	}
}
