using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CuyControl.Tests.Validators;

[TestClass]
public class CuyValidatorTests
{
    [TestMethod]
    public void Placeholder_CuyValidator_ShouldRun()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = expected;

        // Assert
        Assert.IsTrue(actual);
    }
}
