using GraphQL;
using GraphQL.Types;

namespace Neo.Network.GraphQL
{
    public class GraphSchema : Schema
    {
        public GraphSchema(IDependencyResolver resolver) :
           base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
        }
    }
}