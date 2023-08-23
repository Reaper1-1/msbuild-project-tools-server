using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MSBuildProjectTools.LanguageServer.SemanticModel
{
    /// <summary>
    ///     Metadata for an assembly containing MSBuild tasks.
    /// </summary>
    public class MSBuildTaskAssemblyMetadata
    {
        /// <summary>
        ///     The assembly's full name.
        /// </summary>
        [JsonProperty("assemblyName")]
        public string AssemblyName { get; set; }

        /// <summary>
        ///     The full path to the assembly file.
        /// </summary>
        [JsonProperty("assemblyPath")]
        public string AssemblyPath { get; set; }

        /// <summary>
        ///     The assembly file's timestamp.
        /// </summary>
        [JsonProperty("timestampUtc")]
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        ///     Tasks defined in the assembly.
        /// </summary>
        [JsonProperty("tasks", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<MSBuildTaskMetadata> Tasks { get; } = new List<MSBuildTaskMetadata>();
    }

    /// <summary>
    ///     Metadata for an MSBuild task.
    /// </summary>
    public class MSBuildTaskMetadata
    {
        /// <summary>
        ///     The task name.
        /// </summary>
        [JsonProperty("taskName")]
        public string Name { get; set; }

        /// <summary>
        ///     The full name of the type that implements the task.
        /// </summary>
        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        /// <summary>
        ///     The task parameters (if any).
        /// </summary>
        [JsonProperty("parameters", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<MSBuildTaskParameterMetadata> Parameters { get; } = new List<MSBuildTaskParameterMetadata>();
    }

    /// <summary>
    ///     Metadata for a parameter of an MSBuild task.
    /// </summary>
    public class MSBuildTaskParameterMetadata
    {
        /// <summary>
        ///     The parameter name.
        /// </summary>
        [JsonProperty("parameterName")]
        public string Name { get; set; }

        /// <summary>
        ///     The full name of the parameter's data type.
        /// </summary>
        [JsonProperty("parameterType")]
        public string TypeName { get; set; }

        /// <summary>
        ///     Is the parameter type an enum?
        /// </summary>
        [JsonIgnore]
        public bool IsEnum => EnumMemberNames != null;

        /// <summary>
        ///     If the parameter type is an <see cref="Enum"/>, the names of the values that the parameter can contain.
        /// </summary>
        [JsonProperty("enum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public List<string> EnumMemberNames { get; set; }

        /// <summary>
        ///     Is the parameter mandatory?
        /// </summary>
        [JsonProperty("required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsRequired { get; set; }

        /// <summary>
        ///     Is the parameter an output parameter?
        /// </summary>
        [JsonProperty("output", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsOutput { get; set; }
    }
}
