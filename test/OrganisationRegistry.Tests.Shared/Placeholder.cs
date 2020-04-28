namespace OrganisationRegistry.Tests.Shared
{
    using FluentAssertions;
    using Xunit;

    public class Placeholder
    {
        [Fact]
        public void AssertSanity()
        {
            true.Should().BeTrue();
        }
    }
}
