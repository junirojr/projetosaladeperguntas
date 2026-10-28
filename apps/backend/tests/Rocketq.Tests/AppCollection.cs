using FastEndpoints.Testing;
using Xunit;

namespace Rocketq.Tests;

[CollectionDefinition(nameof(AppCollection))]
public class AppCollection : TestCollection<AppFactory> { }
