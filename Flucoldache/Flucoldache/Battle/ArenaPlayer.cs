using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using System.Diagnostics;

namespace Flucoldache.Battle
{
	public class ArenaPlayer : ArenaObj
	{
		Dialogue _dialogue;

		
		public enum Modes
		{
			Menu,
			Items,
			ChoosingEnemy,
			Acting,
		}
		public Modes Mode = Modes.Menu;

		SelectionMenu _menu;
		string[] _menuItems;

		Inventory _inv = (Inventory)Objects.ObjFind<Inventory>(0);

		SelectionMenu _inventoryMenu = null;

		bool _waitingForDialogue = false;
		Dialogue _waitingDialogue = null;

		bool _choosingEnemy;
		bool _choosingFrontOnly;
		Enemy _chosenEnemy;
		public Enemy ChoiceResult = null;

		bool _attacking = false;

		public StatEffect CurrentStatEffect;

		public ArenaPlayer()
		{
			Name = Strings.PlayerName1;
			Name1 = Strings.PlayerName2;

			_menuItems = new string[]
			{
				Strings.Attack,
				Strings.Inventory,
				Strings.Potions
			};

			MaxHealth = 100;

			Health = _inv.Health;
			MinAttack = 10;
			MaxAttack = 20;
			Defence = 10;
			Speed = 10;

			ForegroundColor = Color.White;
		}

		public override void Update()
		{
			base.Update();

			_inv.Health = Health;

			if (Initiative && !Waiting)
			{
				if (Mode == Modes.Menu)
				{
					if (_menu == null)
					{
						_menu = new SelectionMenu("", _menuItems, Dialogue.Pos, Vector2.Zero);
						_menu.DisplayBorders = false;
					}
					else
					{
						if (_menu.Activated)
						{
							if (_menu.SelectedItem == 0)
							{
								_attacking = true;
								ChooseEnemiesInit();
							}

							if (_menu.SelectedItem == 1)
							{
								_inventoryMenu = _inv.ShowItems();
								Mode = Modes.Items;
							}

							if (_menu.SelectedItem == 2)
							{
								_inventoryMenu = _inv.ShowPotions();
								Mode = Modes.Items;
							}

							Objects.Destroy(_menu);
							_menu = null;		
						}
					}

				}

				if (Mode == Modes.Items)
				{
					if (_inventoryMenu.Activated && _inventoryMenu.SelectedItem == _inventoryMenu.Items.Length - 1)
					{
						_menu = new SelectionMenu("", _menuItems, Dialogue.Pos, Vector2.Zero);
						_menu.DisplayBorders = false;
						Mode = Modes.Menu;
					}
				}

				if (Mode == Modes.ChoosingEnemy)
				{
					_chosenEnemy.Selected = true;

					if (Controls.KeyCheckPress(Controls.KeyA))
					{
						ChoiceResult = _chosenEnemy;
						Mode = Modes.Acting;
					}	

					if (Controls.KeyCheckPress(Controls.KeyB))
					{
						Mode = Modes.Menu;
						if (!_attacking)
						{
							_inv.Potions[CurrentStatEffect.Token].Amount += 1;
						}
						_attacking = false;
						SoundController.PlaySound(SoundController.Back);
					}

					Vector2 direction = Vector2.Zero;
					
					if (Controls.KeyCheckPress(Controls.KeyUp))
					{
						direction = -Vector2.UnitY;
						SoundController.PlaySound(SoundController.Blip);
					}
					if (Controls.KeyCheckPress(Controls.KeyDown))
					{
						direction = Vector2.UnitY;
						SoundController.PlaySound(SoundController.Blip);
					}
					if (Controls.KeyCheckPress(Controls.KeyLeft))
					{
						direction = -Vector2.UnitX;
						SoundController.PlaySound(SoundController.Blip);
					}
					if (Controls.KeyCheckPress(Controls.KeyRight))
					{
						direction = Vector2.UnitX;
						SoundController.PlaySound(SoundController.Blip);
					}

					if (direction != Vector2.Zero)
					{
						List<Enemy> enemies = Objects.GetList<Enemy>();
						Arena arena = (Arena)Objects.ObjFind<Arena>(0);

						Vector2 rotatedDirection = new Vector2(direction.Y, -direction.X);

						Enemy newEnemy = null;

						Vector2 sideShift = Vector2.Zero;
						Vector2 shift = (Arena.UnitSpacing + Vector2.One) * direction;

						bool sideChange = false;

						while(newEnemy == null)
						{
							while(arena.IsInFormationBounds(_chosenEnemy.Pos + shift + sideShift) && newEnemy == null)
							{
								foreach(Enemy enemy in enemies)
								{
									if (enemy != _chosenEnemy && enemy.Pos == _chosenEnemy.Pos + shift + sideShift)
									{
										newEnemy = enemy;
										break;
									}
								}

								shift += (Arena.UnitSpacing + Vector2.One) * direction;
							}
							sideShift += (Arena.UnitSpacing + Vector2.One) * rotatedDirection;
							shift = (Arena.UnitSpacing + Vector2.One) * direction;

							if (!arena.IsInFormationBounds(_chosenEnemy.Pos + shift + sideShift))
							{
								if (sideChange)
								{
									break;
								}
								else
								{
									rotatedDirection *= -1;
									sideShift = Vector2.Zero;
									sideChange = true;
								}
							}
						}
						
						if (newEnemy != null)
						{
							_chosenEnemy = newEnemy;
						}

					}

				
				}

				if (Mode == Modes.Acting)
				{
					if (_attacking)
					{
						if (_waitingDialogue == null)
						{
							int dmg = Attack(ChoiceResult);
							_waitingForDialogue = true;
							var dialogueString = Strings.PlayerAttackDialogue
								.Replace("{0}", ChoiceResult.Name.ToLower())
								.Replace("{1}", dmg.ToString());
							_waitingDialogue = new Dialogue(new string[]{""}, new string[] {dialogueString});
						}
					}
					else
					{
						if (_waitingDialogue == null && CurrentStatEffect != null)
						{
							SoundController.PlaySound(SoundController.Potion);
							ChoiceResult.AddStatEffect(CurrentStatEffect, 0);
							_waitingForDialogue = true;
							var dialogueString = Strings.PlayerPotionDialogue
								.Replace("{0}", CurrentStatEffect.Name.ToLower())
								.Replace("{1}", ChoiceResult.Name1.ToLower());
							_waitingDialogue = new Dialogue(new string[]{""}, new string[] {dialogueString});
							CurrentStatEffect = null;
						}
					}
				}
			}
			else
			{
				Mode = Modes.Menu;
				_attacking = false;
			}

			if (_waitingForDialogue)
			{
				if (_waitingDialogue.Destroyed)
				{
					_waitingForDialogue = false;
					_waitingDialogue = null;
					Arena arena = (Arena)Objects.ObjFind<Arena>(0);
					arena.GiveInitiative();
				}
			}

		}


		


		public override void Draw()
		{
			base.Draw();

			GameConsole.DrawChar('@', Pos);

			GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;

			if (Mode == Modes.ChoosingEnemy)
			{
				GameConsole.DrawText(Strings.BattleTip, Dialogue.Pos);
			}
		}

		public void WaitForDialogue(Dialogue dialogue)
		{
			_waitingForDialogue = true;
			_waitingDialogue = dialogue;
		}

		public void ChooseEnemiesInit()
		{
			_choosingEnemy = true;
			if (_chosenEnemy == null || _chosenEnemy.Destroyed)
			{
				_chosenEnemy = (Enemy)Objects.ObjFind<Enemy>(0);
			}
			Mode = Modes.ChoosingEnemy;
		}


	}
}
