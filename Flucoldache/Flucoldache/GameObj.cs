
namespace Flucoldache
{
	/// <summary>
	/// Parent class of every in-game object.
	/// </summary>
	public class GameObj
	{
	
		/// <summary>
		/// If false, Update events won't be executed.
		/// </summary>
		public bool Active = true;

		public GameObj()
		{
			ObjCntrl.AddObject(this);
		}
		
		/// <summary>
		/// Begin of the update at every frame.
		/// </summary>
		public void UpdateBegin() {}		

		
		/// <summary>
		/// Update at every frame.
		/// </summary>
		public virtual void Update() {}
		
		
		/// <summary>
		/// End of the update at every frame.
		/// </summary>
		public virtual void UpdateEnd() {}
	}

}
