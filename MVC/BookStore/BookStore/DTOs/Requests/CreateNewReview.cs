namespace BookStore.DTOs.Requests;

public record CreateNewReview(int rate, int bookId, string? comment);
