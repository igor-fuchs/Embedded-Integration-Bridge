namespace Bridge.Domain.Interfaces;

using Bridge.Domain.Common;
using Bridge.Domain.DTOs;

/// <summary>
/// Service contract for processing command nodes from the API
/// and writing them to OPC UA server.
/// </summary>
public interface ICommandNodeService
{
    /// <summary>
    /// Processes command nodes by validating against registered nodes
    /// and writing valid commands to OPC UA server.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<Result> ProcessCommandsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and writes a single command node to OPC UA server.
    /// </summary>
    /// <param name="command">The command node to process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<Result> WriteCommandAsync(NodeDTO command, CancellationToken cancellationToken = default);
}
