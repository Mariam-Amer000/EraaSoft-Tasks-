namespace CinemaDashboard.ViewModel;

public record MovieFilter(string? Name, decimal? Maxprice, decimal? Minprice,
    bool? Status, DateTime? DateTime, int? CategoryId, int? CinemaId);
