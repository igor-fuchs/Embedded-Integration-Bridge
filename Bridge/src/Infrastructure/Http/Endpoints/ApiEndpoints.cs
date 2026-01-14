namespace Bridge.Infrastructure.Http.Endpoints;

/// <summary>
/// Centralized API endpoint definitions.
/// </summary>
public static class ApiEndpoints
{
    /// <summary>
    /// OPC UA nodes related endpoints.
    /// </summary>
    public static class OpcUaNodes
    {
        private const string Base = "opc-ua/nodes";

        /// <summary>
        /// Gets all nodes endpoint.
        /// </summary>
        public static string GetAll => Base;

        /// <summary>
        /// Gets a specific node by name endpoint.
        /// </summary>
        public static string GetByName(string name) => $"{Base}/{name}";

        /// <summary>
        /// Creates a new node endpoint.
        /// </summary>
        public static string Create => Base;

        /// <summary>
        /// Updates a node by name endpoint.
        /// </summary>
        public static string Update(string name) => $"{Base}/{name}";

        /// <summary>
        /// Deletes a node by name endpoint.
        /// </summary>
        public static string Delete(string name) => $"{Base}/{name}";

        /// <summary>
        /// Gets all node names endpoint.
        /// </summary>
        public static string GetNames => $"{Base}/node-names";
    }
}
