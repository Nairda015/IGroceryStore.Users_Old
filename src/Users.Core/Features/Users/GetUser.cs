using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Users.Exceptions;
using IGroceryStore.Users.Persistence.Mongo;
using IGroceryStore.Users.Persistence.Mongo.DbModels;
using IGroceryStore.Users.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace IGroceryStore.Users.Features.Users;

internal record GetUser(Guid Id) : IHttpQuery;

public class GetUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Users.MapGet<GetUser, GetUserHttpHandler>("{id}")
            .Produces<UserReadModel>()
            .Produces<UserNotFoundException>(404)
            .Produces(401)
            .WithName(nameof(GetUser));
}

internal class GetUserHttpHandler : IHttpQueryHandler<GetUser>
{
    private readonly IMongoCollection<UserDbModel> _collection;

    public GetUserHttpHandler(IMongoCollection<UserDbModel> collection)
    {
        _collection = collection;
    }
    
    public async Task<IResult> HandleAsync(GetUser query, CancellationToken cancellationToken)
    {
        var user = await _collection
            .Find(x => x.Id == query.Id)
            .Project(x => new UserReadModel(x.Id, x.FirstName, x.LastName, x.Email))
            .FirstAsync(cancellationToken);

        return user is null 
            ? Results.NotFound(new UserNotFoundException(query.Id)) 
            : Results.Ok(user);
    }
}
