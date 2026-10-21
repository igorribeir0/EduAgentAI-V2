
using EduAgentAI.Domain.Entities;

namespace EduAgentAI.Domain.Tests;

public class StudentTests
{
    [Fact]
    public void Student_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao@email.com";
        var phone = "11999999999";

        // Act
        var student = new Student(name, email, phone);

        // Assert
        Assert.NotEqual(Guid.Empty, student.Id);
        Assert.Equal(name, student.Name);
        Assert.Equal(email, student.Email);
        Assert.Equal(phone, student.Phone);
    }

    [Fact]
    public void Student_ShouldThrowException_WhenNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Student("", "joao@email.com", "11999999999"));
    }
}
