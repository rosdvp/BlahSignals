namespace BlahSignals
{
public interface IBlahSystem
{
}

public interface IBlahInitSystem : IBlahSystem
{
	/// <summary>
	/// This method will be called exactly at life-time.<br/>
	/// Call happens before <see cref="IBlahResumePauseSystem.Resume"/>.<br/>
	/// Call happens before <see cref="IBlahRunSystem.Run"/>.
	/// </summary>
	/// <param name="initData">Cast this interface to desire type.</param>
	public void Init(IBlahSystemsInitData initData);
}

public interface IBlahRunSystem : IBlahSystem
{
	/// <summary>
	/// This method will be called every time at <see cref="BlahSystemsGroupsContext{TGroupId}.Run"/>.<br/>
	/// Call happens after <see cref="IBlahInitSystem.Init"/>.<br/>
	/// Call happens after <see cref="IBlahResumePauseSystem.Pause"/>.<br/>
	/// </summary>
	public void Run();
}

public interface IBlahResumePauseSystem : IBlahSystem
{
	/// <summary>
	/// This method will be called every time the system becomes active,
	/// AND once <see cref="IBlahInitSystem.Init"/> called.<br/>
	/// Call happens after <see cref="IBlahInitSystem.Init"/>.<br/>
	/// Call happens before <see cref="IBlahRunSystem.Run"/>.<br/>
	/// </summary>
	public void Resume();
	/// <summary>
	/// This method will be called every time the systems becomes inactive.<br/>
	/// It is guaranteed that no <see cref="IBlahRunSystem.Run"/> calls will be performed
	/// until next <see cref="Resume"/> call.
	/// </summary>
	public void Pause();
}
}