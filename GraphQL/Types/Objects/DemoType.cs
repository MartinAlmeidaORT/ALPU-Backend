
using Domain.Interfaces.Private;
using Domain.Models;

namespace GraphQL.Types.Objects;

public class DemoType : ObjectType<Demo>
{
    protected override void Configure(IObjectTypeDescriptor<Demo> descriptor)
    {
        descriptor.Name("Demo");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(d => d.BroadcasterId);

        descriptor.Field(d => d.FileName).Name("fileKey");
        descriptor.Field(d => d.Language);
        descriptor.Field(d => d.Title);

        // audioUrl: pre-signed GET url generated on demand, never persisted.
        // Reuses the FileName member expression so projections still pull that column.
        descriptor.Field("audioUrl")
            .Type<StringType>()
            .Resolve(ctx =>
            {
                Demo demo = ctx.Parent<Demo>();
                var s3Service = ctx.Service<IAmazonS3Service>();
                return s3Service.GetDemoPlaybackUrl(demo.FileName);
            });
    }
}
