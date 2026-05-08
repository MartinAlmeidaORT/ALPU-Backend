namespace Domain.Common.Inputs;

public record CalculateContractInput
{
    public CalculateContractServiceInput[] Services { get; set; } = null!;

    public int ClientId { get; set; }

    public int BroadcasterId { get; set; }
}

public record CalculateContractServiceInput
{
    public int ServiceId { get; set; }

    public string PieceName { get; set; } = null!;

    public ServiceFlagsInput Options { get; set; } = null!;
}

public record ServiceFlagsInput
{
    public bool? HasMedia { get; set; }

    public bool? IsNonComercial { get; set; }

    public bool? IsInterior { get; set; }

    public bool? HasInternetPromo { get; set; }

    public bool? HasLipSync { get; set; }

    public bool? HasMassMediaBroadcast { get; set; }

    public string? MessageIVR { get; set; } // IVR

    public int? AdditionalMessageIVR { get; set; } // IVR

    public int? NarrativeMinutes { get; set; } // Narrative

    public int? RoleQuantity { get; set; } // Narrative

    public int? Pieces { get; set; }

    public int? DurationId { get; set; }

    public bool? MultipleBroadcaster { get; set; }

    public decimal? OverridePrice { get; set; }
}
