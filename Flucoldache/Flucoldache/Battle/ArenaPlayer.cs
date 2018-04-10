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

		
		enum Modes
		{
			Menu,
			Items,
			ChoosingEnemy,
			Acting,
		}
		Modes _mode = Modes.Menu;

		SelectionMenu _menu;
		string[] _menuItems = {"Атака", "Инвентарь", "Зелья"};

		Inventory _inv = (Inventory)Objects.ObjFind<Inventory>(0);

		SelectionMenu _inventoryMenu = null;

		bool _waitingForDialogue = false;
		Dialogue _waitingDialogue = null;

		bool _choosingEnemy;
		bool _choosingFrontOnly;
		Enemy _chosenEnemy;
		public Enemy ChoiceResult = null;

		bool _attacking = false;

		public ArenaPlayer()
		{
			Name = "Игрок";
			
			MaxHealth = 100;

			Health = _inv.Health;
			MinAttack = 30003;
			MaxAttack = 50000;
			Defence = 5;
			Speed = 10;

			ForegroundColor = Color.White;
		}

		public override void Update()
		{
			base.Update();

			_inv.Health = Health;

			if (Initiative)
			{
				if (_mode == Modes.Menu)
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
								ChooseEnemiesInit(false);
							}

							if (_menu.SelectedItem == 1)
							{
								_inventoryMenu = _inv.ShowItems();
								_mode = Modes.Items;
							}

							if (_menu.SelectedItem == 2)
							{
								_inventoryMenu = _inv.ShowPotions();
								_mode = Modes.Items;
							}

							Objects.Destroy(_menu);
							_menu = null;		
						}
					}

				}

				if (_mode == Modes.Items)
				{
					if (_inventoryMenu.Activated && _inventoryMenu.SelectedItem == _inventoryMenu.Items.Length - 1)
					{
						_menu = new SelectionMenu("", _menuItems, Dialogue.Pos, Vector2.Zero);
						_menu.DisplayBorders = false;
						_mode = Modes.Menu;
					}
				}

				if (_mode == Modes.ChoosingEnemy)
				{
					_chosenEnemy.Selected = true;

					if (Controls.KeyCheckPress(Controls.KeyA))
					{
						ChoiceResult = _chosenEnemy;
						_mode = Modes.Acting;
					}	

					if (Controls.KeyCheckPress(Controls.KeyB))
					{
						Objects.Destroy(_chosenEnemy);
						_chosenEnemy = (Enemy)Objects.ObjFind<Enemy>(0);
					}

					Vector2 direction = Vector2.Zero;
					
					if (Controls.KeyCheckPress(Controls.KeyUp))
					{
						direction = -Vector2.UnitY;
					}
					if (Controls.KeyCheckPress(Controls.KeyDown))
					{
						direction = Vector2.UnitY;
					}
					if (Controls.KeyCheckPress(Controls.KeyLeft))
					{
						direction = -Vector2.UnitX;
					}
					if (Controls.KeyCheckPress(Controls.KeyRight))
					{
						direction = Vector2.UnitX;
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

				if (_mode == Modes.Acting)
				{
					if (_attacking)
					{
						if (_waitingDialogue == null)
						{
							int dmg = Attack(ChoiceResult);
							_waitingForDialogue = true;
							_waitingDialogue = new Dialogue(new string[]{""}, new string[] {"Вы бьёте со всей силы и " + ChoiceResult.Name + " получает " + dmg + " урона!"});
						}
					}
				}
			}
			else
			{
				_mode = Modes.Menu;
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
		}

		public void WaitForDialogue(Dialogue dialogue)
		{
			_waitingForDialogue = true;
			_waitingDialogue = dialogue;
		}

		public void ChooseEnemiesInit(bool frontOnly)
		{
			_choosingEnemy = true;
			if (_chosenEnemy == null || _chosenEnemy.Destroyed)
			{
				_chosenEnemy = (Enemy)Objects.ObjFind<Enemy>(0);
			}
			_choosingFrontOnly = frontOnly;
			_mode = Modes.ChoosingEnemy;
		}


	}
}
