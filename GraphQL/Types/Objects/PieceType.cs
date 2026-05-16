using Domain.Models.Campaign;

namespace GraphQL.Types.Objects;

public class PieceType : ObjectType<Piece>
{
    protected override void Configure(IObjectTypeDescriptor<Piece> descriptor)
    {
        descriptor.Name("Piece");
    }
}
