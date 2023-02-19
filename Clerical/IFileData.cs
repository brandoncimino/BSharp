using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical;

/// <summary>
/// Combines a <see cref="FileInfo"/> with the deserialized <see cref="T"/> data that it contains.
/// </summary>
/// <typeparam name="T">the <see cref="Type"/> that this data deserializes as</typeparam>
public interface IFileData<out T> {
    /// <summary>
    /// The implementation-specific, "meaningful" data that this <see cref="IFileData{TData}"/> manages.
    /// </summary>
    /// <remarks>
    /// If <see cref="Data"/> is <c>null</c>, then the <see cref="PersistenceState"/> <b>must</b> be <see cref="Clerical.PersistenceState.NotYetDeserialized"/>.
    /// </remarks>
    public T? Data { get; }

    /// <summary>
    /// The <see cref="System.IO.FileInfo"/> that the <see cref="Data"/> will be written to / read from.
    /// </summary>
    public FileInfo File { get; }

    /// <returns>if it is safe to call <see cref="Deserialize"/></returns>
    public bool CanDeserialize { get; }

    /// <returns>if it is safe to call <see cref="Serialize"/></returns>
    public bool CanSerialize { get; }

    /// <summary>
    /// Populates the <see cref="Data"/> object with the contents of <see cref="File"/>.
    /// </summary>
    /// <exception cref="IOException">if <see cref="CanDeserialize"/> is <c>false</c></exception>
    public void Deserialize();

    /// <summary>
    /// Writes the <see cref="Data"/> object to the <see cref="File"/>.
    /// </summary>
    /// <exception cref="IOException">if <see cref="CanSerialize"/> is <c>false</c></exception>
    public void Serialize();

    /// <summary>
    /// Ensures that:
    /// <ul>
    /// <li><see cref="Data"/> is not <c>null</c></li>
    /// <li><see cref="FileInfo"/> <see cref="FileInfoExtensions.ExistsWithContent"/></li>
    /// <li>The contents of <see cref="FileInfo"/> match the <see cref="Data"/> object</li>
    /// </ul>
    /// </summary>
    /// <remarks>
    /// This method should be <a href="https://en.wikipedia.org/wiki/Idempotence">idempotent</a>,
    /// i.e. calling <see cref="Sync"/> twice in a row should <b>not</b> cause any issues.
    /// </remarks>
    /// <exception cref="IOException">if we were unable to corroborate the <see cref="Data"/> and <see cref="FileInfo"/></exception>
    public void Sync();
}