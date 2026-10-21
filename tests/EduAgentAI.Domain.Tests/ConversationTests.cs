
using EduAgentAI.Domain.Entities;
using EduAgentAI.Domain.Enums;

namespace EduAgentAI.Domain.Tests;

public class ConversationTests
{
    [Fact]
    public void Conversation_ShouldCreateSuccessfully()
    {
        var studentId = Guid.NewGuid();

        var conversation = new Conversation(studentId);

        Assert.NotEqual(Guid.Empty, conversation.Id);
        Assert.Equal(studentId, conversation.StudentId);
        Assert.False(conversation.IsClosed);
        Assert.Empty(conversation.Messages);
    }

    [Fact]
    public void Conversation_ShouldAddMessage()
    {
        var conversation = new Conversation(Guid.NewGuid());

        var message = new Message(
            conversation.Id,
            MessageRole.Student,
            "Quando é minha próxima aula?");

        conversation.AddMessage(message);

        Assert.Single(conversation.Messages);
    }

    [Fact]
    public void Conversation_ShouldNotAcceptMessageFromAnotherConversation()
    {
        var conversation = new Conversation(Guid.NewGuid());

        var message = new Message(
            Guid.NewGuid(),
            MessageRole.Student,
            "Olá!");

        Assert.Throws<ArgumentException>(() =>
            conversation.AddMessage(message));
    }

    [Fact]
    public void Conversation_ShouldNotAcceptMessagesAfterClosing()
    {
        var conversation = new Conversation(Guid.NewGuid());

        conversation.Close();

        var message = new Message(
            conversation.Id,
            MessageRole.Student,
            "Olá!");

        Assert.Throws<InvalidOperationException>(() =>
            conversation.AddMessage(message));
    }

    [Fact]
    public void Message_ShouldNotAcceptEmptyContent()
    {
        Assert.Throws<ArgumentException>(() =>
            new Message(
                Guid.NewGuid(),
                MessageRole.Student,
                ""));
    }
}
