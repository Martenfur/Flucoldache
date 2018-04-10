using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class DummyEnemy : Enemy
	{
		public DummyEnemy()
		{
			Name = "Чучело";

			MaxHealth = 100;
			Health = MaxHealth;
			MinAttack = 10;
			MaxAttack = 20;
			Defence = 2;
			Speed = 1;
		}

		Dialogue _dialogue;


		public override void Update()
		{
			base.Update();
			
			if (Initiative)
			{
				if (_dialogue == null)
				{
					int dmg = Attack((ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0));
					
					_dialogue = new Dialogue(new string[]{""}, new string[]{"Чучело атакует и наносит " + dmg + " урона! О нет! :0"});
				}
				else
				{
					if (_dialogue.Destroyed)
					{
						_dialogue = null;
						Arena arena = (Arena)Objects.ObjFind<Arena>(0);
						arena.GiveInitiative();
					}
				}
			}
		}

		public override void Draw()
		{
			base.Draw();
			
			GameConsole.DrawChar('D', Pos);
		}



	}
}
