namespace EduAgentAI.Domain.Entities;

public class Student
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Student(string name, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("O telefone é obrigatório.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
