using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class Rabbit : Enemy
	{
		public Rabbit()
		{
			Name = Strings.RabbitName1;
			Name1 = Strings.RabbitName2;

			MaxHealth = 16;
			Health = MaxHealth;
			MinAttack = 5;
			MaxAttack = 7;
			Defence = 0;
			Speed = 50;
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
			
			GameConsole.DrawChar('з', Pos);
		}



	}
}
