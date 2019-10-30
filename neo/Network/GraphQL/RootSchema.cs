using GraphQL;
using GraphQL.Types;

namespace Neo.Network.GraphQL
{
    public class RootSchema : Schema
    {
        public RootSchema(IDependencyResolver resolver) :
           base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
        }
    }
}