using System;
using System.Collections.Generic;
using System.Linq;

namespace Flucoldache
{
	public static class ObjCntrl
	{
		
		/// <summary>
		/// List of all game objects.
		/// </summary>
		public static List<GameObj> GameObjects = new List<GameObj>();

		/// <summary>
		/// List of newly created game objects. Since it won't be that cool to modify main list 
		/// in mid-step, they'll be added in next one.
		/// </summary>
		private static List<GameObj> _newGameObjects = new List<GameObj>();

		/// <summary>
		/// List of objects that should be destroyed in next step.
		/// </summary>
		private static List<GameObj> _destroyedGameObjects = new List<GameObj>();

		private static double _fixedUpdateAl = 0;


		public static void Update()
		{
			// Clearing main list from destroyed objects.
			for(var i = 0; i < _destroyedGameObjects.Count; i += 1)
			{
				GameObjects.Remove(_destroyedGameObjects[i]);
			}
			_destroyedGameObjects.Clear();
			// Clearing main list from destroyed objects.


			// Adding new objects to the list.
			GameObjects.AddRange(_newGameObjects);
			_newGameObjects.Clear();
			// Adding new objects to the list.


			// Normal updates.
			foreach(GameObj obj in GameObjects)
			{
				if (obj.Active)
				{
					obj.UpdateBegin();
				}
			}

			foreach(GameObj obj in GameObjects)
			{
				if (obj.Active)
				{
					obj.Update();
				}
			}
		
			foreach(GameObj obj in GameObjects)
			{
				if (obj.Active)
				{
					obj.UpdateEnd();
				}
			}
			// Normal updates.
		}



		/// <summary>
		/// Adds object to object list.
		/// </summary>
		/// <param name="obj"></param>
		public static void AddObject(GameObj obj) => _newGameObjects.Add(obj);


		#region user functions 

		/// <summary>
		/// Returns list of objects of certain type.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		public static List<T> GetList<T>() => (List<T>)(GameObjects.OfType<T>());


		/// <summary>
		/// Counts amount of objects of certain type.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <returns>Returns amount of objects.</returns>
		public static int Count<T>() => GameObjects.OfType<T>().Count();


		/// <summary>
		/// Destroys game object.
		/// </summary>
		/// <param name="obj">Game object.</param>
		public static void Destroy(GameObj obj)
		{
			if (!_destroyedGameObjects.Contains(obj))
			{
				_destroyedGameObjects.Add(obj);
			}
		}


		/// <summary>
		/// Checks if given instance exists.
		/// </summary>
		/// <param name="obj">Object to check.</param>
		public static bool ObjExists(GameObj obj)
		{
			foreach(GameObj obj1 in GameObjects)
			{
				if (obj == obj1)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Finds n-th object of given type.
		/// </summary>
		/// <typeparam name="T">Type to search.</typeparam>
		/// <param name="count">Number of the object in object list.</param>
		/// <returns>Returns object if it was found, or null, if it wasn't.</returns>
		public static GameObj ObjFind<T>(int count)
		{
			int counter = 0;

			foreach(GameObj obj in GameObjects)
			{
				if (counter >= count && obj is T)
				{
					return obj;
				}
				counter += 1;
			}
			return null;
		}

		#endregion user functions 

	}
}
