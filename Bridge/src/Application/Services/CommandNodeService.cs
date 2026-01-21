namespace Bridge.Application.Services;

using Bridge.Domain.Common;
using Bridge.Domain.DTOs;
using Bridge.Domain.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service responsible for processing command nodes from the API
/// and writing valid commands to the OPC UA server.
/// </summary>
public sealed class CommandNodeService : ICommandNodeService
{
    private readonly IApiClient _apiClient;
    private readonly IOpcUaClient _opcUaClient;
    private readonly IRegisteredNodeCache _registeredNodeCache;
    private readonly ILogger<CommandNodeService> _logger;

    public CommandNodeService(
        IApiClient apiClient,
        IOpcUaClient opcUaClient,
        IRegisteredNodeCache registeredNodeCache,
        ILogger<CommandNodeService> logger)
    {
        _apiClient = apiClient;
        _opcUaClient = opcUaClient;
        _registeredNodeCache = registeredNodeCache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> ProcessCommandsAsync(CancellationToken cancellationToken = default)
    {
        // Fetch command nodes from API
        var commandsResult = await _apiClient.GetCommandNodesAsync(cancellationToken);

        if (commandsResult.IsFailure)
        {
            _logger.LogWarning(
                "Failed to fetch command nodes: {Error}",
                commandsResult.Error.Description);
            return Result.Failure(commandsResult.Error);
        }

        var commands = commandsResult.Value.Commands;

        if (commands.Count == 0)
        {
            _logger.LogDebug("No command nodes to process");
            return Result.Success();
        }

        _logger.LogDebug("Processing {Count} command nodes", commands.Count);

        var errors = new List<Error>();
        var successCount = 0;
        var skippedCount = 0;

        foreach (var command in commands)
        {
            // Validate that the node is registered
            if (!_registeredNodeCache.IsRegistered(command.Name))
            {
                _logger.LogWarning(
                    "⚠️ Skipping command for unregistered node: {NodeName}",
                    command.Name);
                skippedCount++;
                continue;
            }

            var writeResult = await WriteCommandAsync(command, cancellationToken);

            if (writeResult.IsSuccess)
            {
                successCount++;
            }
            else
            {
                errors.Add(writeResult.Error);
            }
        }

        _logger.LogInformation(
            "📊 Command processing completed: {Success} succeeded, {Skipped} skipped, {Failed} failed",
            successCount,
            skippedCount,
            errors.Count);

        if (errors.Count > 0)
        {
            return Result.Failure(
                Error.Failure(
                    "CommandProcessing.PartialFailure",
                    $"Some commands failed to execute: {errors.Count} failures"));
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> WriteCommandAsync(NodeDTO command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Validate that the node is registered
        if (!_registeredNodeCache.IsRegistered(command.Name))
        {
            var error = Error.Validation(
                "Command.UnregisteredNode",
                $"Node '{command.Name}' is not registered and cannot receive commands");

            _logger.LogWarning("⚠️ {ErrorDescription}", error.Description);
            return Result.Failure(error);
        }

        // Attempt to write the value to OPC UA server
        var success = await _opcUaClient.WriteNodeValueAsync(
            command.Name,
            command.Value,
            cancellationToken);

        if (!success)
        {
            var error = Error.Failure(
                "Command.WriteFailed",
                $"Failed to write value to node '{command.Name}'");

            _logger.LogError("❌ {ErrorDescription}", error.Description);
            return Result.Failure(error);
        }

        _logger.LogDebug(
            "✅ Successfully wrote command to node {NodeName}: {Value}",
            command.Name,
            command.Value);

        return Result.Success();
    }
}
