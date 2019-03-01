using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Flucoldache.Battle;

namespace Flucoldache
{
	public class Inventory : GameObj
	{
		static string _invItemsFile;
		Vector2 _listSize;
		Vector2 _listPos;
		
		/// <summary>
		/// Contatins samples of each inventory item.
		/// </summary>
		public static Dictionary<string, InventoryItem> ItemPool;

		public Dictionary<string, InventoryItem> Items = new Dictionary<string, InventoryItem>();
		public Dictionary<string, InventoryItem> Potions = new Dictionary<string, InventoryItem>();
		
		public int MaxHealth = 100;
		public int Health = 100;
		

		SelectionMenu _currentSelectionMenu = null;
		Dictionary<string, InventoryItem> _currentInventory = null;

		delegate void Use(InventoryItem item);

		Dictionary<string, Use> _itemActions = new Dictionary<string, Use>();

		public Inventory()
		{
			
			LoadItemPool();
			_listSize = new Vector2(32, GameConsole.H - Dialogue.Size.Y - 4);
			_listPos = new Vector2(GameConsole.W - _listSize.X - 1, 1);

			_itemActions.Add("book", UseBook);
			_itemActions.Add("lab", UseLab);

			_itemActions.Add("chicken", UseChicken);
			_itemActions.Add("bread", UseBread);
			_itemActions.Add("kingfood", UseKingfood);


			_itemActions.Add("cold", UsePotion);
			_itemActions.Add("weakness", UsePotion);
			_itemActions.Add("flu", UsePotion);
			_itemActions.Add("rabies", UsePotion);
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

						if (item.Spendable)
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
								if (Objects.ObjExists<Battle.Arena>())
								{
									if (item.Amount < 0)
									{
										item.Amount = 0;
										invoke = false;
										_currentSelectionMenu.Activated = false;
									}
								}
								else
								{
									Objects.Destroy(_currentSelectionMenu);
									new Dialogue(new string[]{""}, new string[]{Strings.CantUsePotionsNow});		
									invoke = false;						
									_currentSelectionMenu = null;
									_currentInventory = null;
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
				DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));
				GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;
				GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
				GameConsole.DrawRectangle(Dialogue.Pos, Dialogue.Size);
				GameConsole.DrawFrame(Dialogue.Pos - Vector2.One, Dialogue.Size + Vector2.One * 2);
				
				string desc;

				if (_currentSelectionMenu.SelectedItem >= _currentInventory.Count)
				{
					desc = Strings.BackTip;
				}
				else
				{
					desc = _currentInventory.ElementAt(_currentSelectionMenu.SelectedItem).Value.Description;
				}

				GameConsole.DrawText(desc, Dialogue.Pos);
				DrawCntrl.ResetTransformMatrix();
			}
		}


		/// <summary>
		/// Loads item info from data file.
		/// </summary>
		public static void LoadItemPool()
		{
			_invItemsFile = "Resources/" + Strings.Localization + "/InventoryItems.xml";

			ItemPool = new Dictionary<string, InventoryItem>();

			XmlDocument xml = new XmlDocument();
			xml.Load(_invItemsFile);
			
			XmlNodeList nodes = xml.DocumentElement.SelectNodes("item");
			

			foreach(XmlNode node in nodes)
			{
				InventoryItem item = new InventoryItem();
				
				item.Token = node.SelectSingleNode("token").FirstChild.Value;
				
				item.Name = node.SelectSingleNode("name").FirstChild.Value;
				item.Name1 = node.SelectSingleNode("name1").FirstChild.Value;
				item.Spendable = (node.SelectSingleNode("type").FirstChild.Value == "spendable");

				item.Stack = Int32.Parse(node.SelectSingleNode("stack").FirstChild.Value);

				item.Description = node.SelectSingleNode("description").FirstChild.Value.Replace("\t", "");
				
				if (item.Description.StartsWith(Environment.NewLine))
				{
					item.Description = item.Description.Remove(0, Environment.NewLine.Length);
				}

				ItemPool.Add(item.Token, item);
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
			if (ItemPool.ContainsKey(token))
			{
				if (inv.ContainsKey(token))
				{
					inv[token].Amount += amount;
				}
				else
				{
					InventoryItem item = new InventoryItem(ItemPool[token]);
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

		
		/// <summary>
		/// Saves inventory into the file.
		/// </summary>
		/// <param name="path"></param>
		public void SaveInventory(string path)
		{
			XmlDocument xml = new XmlDocument();

			XmlElement root = xml.CreateElement("Inventory");
			root.SetAttribute("health", Health.ToString());
			XmlElement itemList = xml.CreateElement("Items");
			XmlElement potionList = xml.CreateElement("Potions");
			
			foreach(KeyValuePair<string, InventoryItem> item in Items)
			{
				XmlElement element = xml.CreateElement("item");
				element.SetAttribute("token", item.Value.Token);
				element.SetAttribute("amount", item.Value.Amount.ToString());
				itemList.AppendChild(element);
			}

			foreach(KeyValuePair<string, InventoryItem> item in Potions)
			{
				XmlElement element = xml.CreateElement("item");
				element.SetAttribute("token", item.Value.Token);
				element.SetAttribute("amount", item.Value.Amount.ToString());
				potionList.AppendChild(element);
			}


			root.AppendChild(itemList);
			root.AppendChild(potionList);
			xml.AppendChild(root);

			xml.Save(Environment.CurrentDirectory + path + "/Inventory.xml");	
		}

		public void LoadInventory(string path)
		{
			Items.Clear();
			Potions.Clear();

			XmlDocument xml = new XmlDocument();
			
			try
			{
				xml.Load(path + "/Inventory.xml");
			
				XmlNodeList nodes = xml.DocumentElement.SelectNodes("loot");
				
				Health = Int32.Parse(xml.DocumentElement.Attributes["health"].Value);
				if (Health > MaxHealth)
				{
					Health = MaxHealth;
				}

				XmlNode items = xml.DocumentElement.SelectSingleNode("Items");
			
				foreach(XmlElement item in items.SelectNodes("item"))
				{
					AddItem(item.Attributes["token"].Value, Int32.Parse(item.Attributes["amount"].Value));
				}

				XmlNode potions = xml.DocumentElement.SelectSingleNode("Potions");

				foreach(XmlElement item in potions.SelectNodes("item"))
				{
					AddPotion(item.Attributes["token"].Value, Int32.Parse(item.Attributes["amount"].Value));
				}
			}
			catch(Exception) {}
		}


		public SelectionMenu ShowItems()
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
				menuItems[i] = Strings.Back;

				_currentSelectionMenu = new SelectionMenu(Strings.Inventory, menuItems, _listPos, _listSize);
				_currentInventory = Items;
			}

			return _currentSelectionMenu;
		}

		public SelectionMenu ShowPotions()
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
				menuItems[i] = Strings.Back;

				_currentSelectionMenu = new SelectionMenu(Strings.Potions, menuItems, _listPos, _listSize);
				_currentInventory = Potions;
			}

			return _currentSelectionMenu;
		}

		bool RestoreHealth(int health)
		{
			if (Objects.ObjExists<Arena>())
			{
				ArenaPlayer player = (ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0);

				if (player.Health == player.MaxHealth)
				{
					return false;
				}

				player.Health += health;
				if (player.Health > player.MaxHealth)
				{
					player.Health = player.MaxHealth;
				}
			}
			else
			{
				if (Health == MaxHealth)
				{
					return false;
				}

				Health += health;
				if (Health > MaxHealth)
				{
					Health = MaxHealth;
				}
			}
			return true;
		}

		void PassDialogue(Dialogue dialogue)
		{
			ArenaPlayer player= (ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0);

			if (player != null)
			{
				player.WaitForDialogue(dialogue);
			}
		}

		#region Item actions
		
		void NothingHappenedAction(InventoryItem item)
		{
			string[] names = {"", ""};
			string[] lines = {Strings.NothingHappened.Replace("{0}", item.Name1.ToLower())};
			PassDialogue(new Dialogue(names, lines));
		}

		void UseBook(InventoryItem item)
		{
			string[] names = {""};
			string[] lines = {Strings.CantUsePotionsNow};
			PassDialogue(new Dialogue(names, lines));
		}

		void UseLab(InventoryItem item)
		{
			string[] names = {""};
			string[] lines = {Strings.CantUseLab};
			PassDialogue(new Dialogue(names, lines));
		}

		#region Food

		void UseChicken(InventoryItem item)
		{
			UseFood(item, 50);
		}

		void UseBread(InventoryItem item)
		{
			UseFood(item, 25);
		}

		void UseKingfood(InventoryItem item)
		{
			UseFood(item, 100);
		}


		void UseFood(InventoryItem item, int health)
		{
			string[] lines;

			string[] names = {""};

			if (RestoreHealth(health))
			{	
				lines = new string[]{Strings.EatingFood.Replace("{0}", item.Name1.ToLower())};
			}
			else
			{
				lines = new string[]{Strings.EatingDeny.Replace("{0}", item.Name1.ToLower())};
				item.Amount += 1;
			}
			PassDialogue(new Dialogue(names, lines));
		}

		#endregion Food


		void UsePotion(InventoryItem item)
		{
			ArenaPlayer player = (ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0);

			player.ChooseEnemiesInit();
			player.CurrentStatEffect = StatEffect.Effects[item.Token];
		}


		#endregion Item actions

	}
}
