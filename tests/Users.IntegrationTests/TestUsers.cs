using System.Security.Claims;
using Bogus;
using IGroceryStore.Users.Features.Users;

namespace IGroceryStore.Users.IntegrationTests;

internal static class TestUsers
{
    private static readonly Faker<Register> UserGenerator = new RegisterFaker();
    
    public static readonly List<Register> Registers = UserGenerator.Generate(10);
    public static readonly Register Register = Registers.First();
    
    private sealed class RegisterFaker : Faker<Register>
    {
        public RegisterFaker()
        {
            CustomInstantiator(ResolveConstructor);
        }

        private Register ResolveConstructor(Faker faker)
        {
            var password = Guid.NewGuid().ToString();

            var body = new Register.RegisterBody(
                faker.Person.Email,
                password,
                password,
                faker.Person.FirstName,
                faker.Person.LastName);

            return new Register(body);
        }
    }
}

public interface IMockUser
{
    List<Claim> Claims { get; }
}
public class MockUser : IMockUser
{
    public List<Claim> Claims { get; private set; } = new();
    public MockUser(params Claim[] claims)
        => Claims = claims.ToList();
}

public static class AuthConstants
{
    public const string Scheme = "TestAuth";
}
