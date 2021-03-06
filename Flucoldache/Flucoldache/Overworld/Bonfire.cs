﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flucoldache;
using Monofoxe.Engine;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Flucoldache.Overworld
{
	public class Bonfire : OverworldObj
	{
		
		double[] _colors = new double[5];
		int[] _colorDirections = new int[5];
		float _colorChangeCycle = 5f; // 1/s

		public Bonfire()
		{
			for(var i = 0; i < _colors.Length; i += 1)
			{
				_colors[i] = GameplayController.Random.NextDouble();
				if (GameplayController.Random.NextDouble() >= 0.5)
				{
					_colorDirections[i] = 1;
				}
				else
				{
					_colorDirections[i] = -1;
				}
			}
		}
		
		public override void Update()
		{
			for(var i = 0; i < _colors.Length; i += 1)
			{
				_colors[i] += _colorDirections[i] * GameCntrl.Time(_colorChangeCycle);
				if (_colors[i] > 1 || _colors[i] < 0)
				{
					_colors[i] -= (_colors[i] - (int)(_colors[i])) * 2.0 *_colorDirections[i];
					_colorDirections[i] *= -1;
				}
			}
		}

		public override bool TriggerAction()
		{
			Inventory inv = (Inventory)Objects.ObjFind<Inventory>(0);

			inv.Health = inv.MaxHealth;

			// Refilling potions.
			foreach(KeyValuePair<string, InventoryItem> potion in inv.Potions)
			{
				potion.Value.Amount = potion.Value.Stack;
			}
			// Refilling potions.

			if (Argument != "")
			{
				inv.AddPotion(Argument, 9999);
				var line2 = 
				new Dialogue(
					new string[]{"", "", ""}, 
					new string[]
					{
						Strings.BonfireLine1, 
						Strings.BonfireLine2.Replace("{0}",Inventory.ItemPool[Argument].Name.ToLower()), 
						Strings.BonfireLine3
					}
				);
				Argument = "";
			}
			else
			{
				new Dialogue(
					new string[]{"", "", ""}, 
					new string[]
					{
						Strings.BonfireLine1,
						Strings.BonfireLine3
					}
				);
			}

			GameplayController.SaveGame();

			return true;
		}



		public override void Draw()
		{
			GameConsole.ForegroundColor = Color.White;
			GameConsole.BackgroundColor = Color.Lerp(Color.Orange, Color.OrangeRed, (float)_colors[0]);
			
			GameConsole.DrawChar('л', Pos);

			Terrain terr = (Terrain)Objects.ObjFind<Terrain>(0);

			var i = 0;
			foreach(Vector2 v in Player.Rotation)
			{
				i += 1;
				Tile tile = terr.GetTile(Pos + v);
				if (tile.Char == ' ' || tile.Char == ';')
				{
					tile.ForegroundColor = Color.Lerp(Color.Orange, Color.OrangeRed, (float)_colors[i]);
					tile.BackgroundColor = GameConsole.BaseBackgroundColor;
					tile.Char = ';';
				}

			}

		}
	}
}
