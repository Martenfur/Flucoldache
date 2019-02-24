using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class Villager : Enemy
	{
		public Villager()
		{
			Name = Strings.VillagerName1;
			Name1 = Strings.VillagerName2;

			MaxHealth = 30;
			Health = MaxHealth;
			MinAttack = 10;
			MaxAttack = 15;
			Defence = 0;
			Speed = 1;
			ForegroundColor = Color.White;
		}

		

		public override void Update()
		{
			base.Update();
			
			if (Initiative && !Waiting)
			{
				PerformBasicAttack();
			}
		}

		public override void Draw()
		{
			base.Draw();
			
			GameConsole.DrawChar('M', Pos);
		}



	}
}
