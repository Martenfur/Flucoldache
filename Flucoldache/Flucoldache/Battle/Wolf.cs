﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class Wolf : Enemy
	{
		public Wolf()
		{
			Name = Strings.WolfName1;
			Name1 = Strings.WolfName2;

			MaxHealth = 20;
			Health = MaxHealth;
			MinAttack = 10;
			MaxAttack = 15;
			Defence = 3;
			Speed = 20;
			ForegroundColor = Color.Gray;
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
			
			GameConsole.DrawChar('в', Pos);
		}



	}
}
