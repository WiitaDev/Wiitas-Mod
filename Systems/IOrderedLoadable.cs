namespace WiitaMod.Systems
{
	interface IOrderedLoadable
	{
		void Load();
		void Unload();
		float Priority { get; }
	}
}