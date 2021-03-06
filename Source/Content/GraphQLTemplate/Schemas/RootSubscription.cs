namespace GraphQLTemplate.Schemas
{
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using GraphQL.Resolvers;
    using GraphQL.Types;
    using GraphQLTemplate.Models;
    using GraphQLTemplate.Repositories;
    using GraphQLTemplate.Types;

    /// <example>
    /// subscription whenHumanCreated {
    ///   humanCreated(homePlanets: ["Earth"])
    ///   {
    ///     id,
    ///     name
    ///   }
    /// }
    /// </example>
    public class RootSubscription : ObjectGraphType<object>
    {
        public RootSubscription(IHumanRepository humanRepository)
        {
            this.Name = "Subscription";
            this.Description = "The subscription type, represents all updates can be pushed to the client in real time over web sockets.";

            this.AddField(
                new EventStreamFieldType()
                {
                    Name = "humanCreated",
                    Arguments = new QueryArguments(
                        new QueryArgument<ListGraphType<StringGraphType>>()
                        {
                            Name = "homePlanets"
                        }),
                    Type = typeof(HumanCreatedEvent),
                    Resolver = new FuncFieldResolver<Human>(context => context.Source as Human),
                    Subscriber = new EventStreamResolver<Human>(context =>
                    {
                        var homePlanets = context.GetArgument<List<string>>("homePlanets");
                        return humanRepository
                            .WhenHumanCreated
                            .Where(x => homePlanets == null || homePlanets.Contains(x.HomePlanet));
                    }),
                });
        }
    }
}
