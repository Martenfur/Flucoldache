using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flucoldache;
using Monofoxe.Engine;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;

namespace Flucoldache.Overworld
{
	public class LootContainer : OverworldObj
	{
		string[] _loot;
		int[] _lootAmount;
		string _name;

		static string _rootDir = "Resources/LootTables/";
		
		/// <summary>
		/// If this string is in the beginning of the path, root dir won't be added.
		/// </summary>
		static string _norootStr = "..";

		public LootContainer()
		{
			
		}

		public override void Update()
		{
			Player player = (Player)Objects.ObjFind<Player>(0);
			if (player != null)
			{
				if (Pos == player.Pos)
				{
					Open();
					Objects.Destroy(this);
				}
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.Black;
				GameConsole.BackgroundColor = Color.AliceBlue;
				GameConsole.DrawChar('C', Pos);
			}
		}

		public override void ProccessArgument()
		{
			XmlDocument xml = new XmlDocument();
			if (Argument.StartsWith(_norootStr))
			{
				xml.Load(Environment.CurrentDirectory + Argument.Replace(_norootStr, ""));
			}
			else
			{
				xml.Load(_rootDir + Argument);
			}
			
			_name = xml.DocumentElement.Attributes["name"].Value;

			XmlNodeList nodes = xml.DocumentElement.SelectNodes("loot");
			_loot = new string[nodes.Count];
			_lootAmount = new int[nodes.Count];


			var i = 0;
			foreach(XmlNode node in nodes)
			{
				_loot[i] = node.Attributes["token"].Value;
				_lootAmount[i] = Int32.Parse(node.Attributes["amount"].Value);
				i += 1;
			}
			if (_loot.Length == 0)
			{
				_loot = null;
			}
		}


		public void Open()
		{
			StringBuilder lootStr = new StringBuilder();
			Inventory inv = (Inventory)Objects.ObjFind<Inventory>(0);
			string[]  lines = new string[1];
			
			if (_loot != null)
			{
				for(var i = 0; i < _loot.Length; i += 1)
				{
					lootStr.Append(inv.ItemPool[_loot[i]].Name);
					if (_lootAmount[i] > 1)
					{
						lootStr.Append(" x" + _lootAmount[i]);
					}
					if (i < _loot.Length - 1)
					{
						lootStr.Append(", ");
					}
					inv.AddItem(_loot[i], _lootAmount[i]);
				}
				lootStr.Append(".");
				lines[0] = "Вы открыли " + _name + "." + Environment.NewLine + "Внутри оказались: " + lootStr;
				_loot = null;
			}
			else
			{
				lines[0] = "Вы открыли " + _name + ", но внутри ничего не было.";
			}

			new Dialogue(new string[]{""}, lines);

		}

		public override bool TriggerAction()
		{
			Open();
			return true;
		}

		public void GenerateLootTable(string path, int index)
		{
			XmlDocument xml = new XmlDocument();
			XmlElement root = xml.CreateElement("Container");
			root.SetAttribute("name", _name);

			if (_loot != null)
			{
				for(var i = 0; i < _loot.Length; i += 1)
				{
					XmlElement item = xml.CreateElement("loot");
					item.SetAttribute("token", _loot[i]);
					item.SetAttribute("amount", _lootAmount[i].ToString());
					root.AppendChild(item);
				}
			}

			xml.AppendChild(root);
			xml.Save(Environment.CurrentDirectory + path + "/loot_" + index + ".xml");
			Argument = _norootStr + path + "/loot_" + index + ".xml";
		}
	}
}
