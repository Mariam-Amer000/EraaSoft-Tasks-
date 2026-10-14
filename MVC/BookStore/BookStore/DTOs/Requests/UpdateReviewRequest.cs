namespace BookStore.DTOs.Requests;

public record UpdateReviewRequest(int reviewId, int newRate, string newComment);
