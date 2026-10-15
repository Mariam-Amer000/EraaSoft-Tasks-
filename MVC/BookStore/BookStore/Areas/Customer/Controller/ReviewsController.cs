using System.Security.Claims;

namespace BookStore.Areas.Customer.Controller;

[Route("api/[area]/[controller]")]
[ApiController]
[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<UserBookReview> _userBookReviewRepository;
    private readonly IRepository<Book> _book;

    public ReviewsController(
        UserManager<ApplicationUser> userManager,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<UserBookReview> userBookReviewRepository,
        IRepository<Book> book)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _userBookReviewRepository = userBookReviewRepository;
        _book = book;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get(string? query, int page = 1, int size = 4)
    {
        var reviews = _userBookReviewRepository.Get();

        if (query is not null)
            reviews = reviews.Where(e =>
                e.Comment != null && e.Comment.ToLower().Contains(query.ToLower()));

        var totalPages = Math.Ceiling(reviews.Count() / (double)size);
        reviews = reviews.Skip((page - 1) * size).Take(size);

        return Ok(new ReviewWithFilterResponse
        {
            Reviews = reviews,
            Query = query ?? "",
            TotalPages = totalPages,
            CurrentPage = page,
        });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetOne(int id)
    {
        var review = _userBookReviewRepository.GetOne(
            e => e.Id == id,
            tracked: false);

        if (review is null)
            return NotFound();

        return Ok(review);
    }

    [HttpPost("Comment")]
    public async Task<IActionResult> Create(
        CreateNewReview createNewReview,
        CancellationToken ct = default)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var book = _book.GetOne(e => e.Id == createNewReview.bookId);

        if (book is null)
            return NotFound();

        var existingReview = _userBookReviewRepository.GetOne(
            e => e.ApplicationUserId == userId &&
                 e.BookId == createNewReview.bookId);

        if (existingReview is not null)
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "You have already reviewed this book."
            });

        var userOrders = _orderRepository.Get(
            e => e.ApplicationUserId == userId &&
                 e.OrderStatus == OrderStatus.Delivered);

        foreach (var item in userOrders)
        {
            var userOrderItems = _orderItemRepository.Get(
                e => e.OrderId == item.Id &&
                     e.BookId == createNewReview.bookId);

            if (userOrderItems.Any())
            {
                var created = await _userBookReviewRepository.CreateAsync(new()
                {
                    ApplicationUserId = userId,
                    Comment = createNewReview.comment,
                    Rate = createNewReview.rate,
                    BookId = createNewReview.bookId,
                }, ct);

                if (!created)
                    return StatusCode(500, new ErrorResponse
                    {
                        ErrorNotification = "Could not create review."
                    });

                await _userBookReviewRepository.CommitAsync(ct);

                return Ok(new SuccessResponse()
                {
                    SuccessNotification = "Create new review successfully"
                });
            }
        }

        return BadRequest(new ErrorResponse()
        {
            ErrorNotification = "Failed to create new review"
        });
    }

    [HttpPost("Reply")]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Reply(CreateNewReply createNewReply)
    {
        var book = _book.GetOne(e => e.Id == createNewReply.bookId);

        if (book is null)
            return NotFound();

        var review = _userBookReviewRepository.GetOne(
            e => e.Id == createNewReply.reviewId);

        if (review is null)
            return NotFound();

        if (review.BookId != createNewReply.bookId)
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "The review does not belong to this book."
            });

        review.Reply = createNewReply.reply;

        await _userBookReviewRepository.CommitAsync();

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Add Reply successfully"
        });
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateReviewRequest updateReviewRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var review = _userBookReviewRepository.GetOne(
            e => e.Id == updateReviewRequest.reviewId);

        if (review is null)
            return NotFound();

        if (review.ApplicationUserId != userId)
            return Forbid();

        if (review.Reply is not null)
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = "You can not update this comment"
            });

        review.Rate = updateReviewRequest.newRate;
        review.Comment = updateReviewRequest.newComment;

        await _userBookReviewRepository.CommitAsync();

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Update review successfully"
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct = default)
    {
        var review = _userBookReviewRepository.GetOne(e => e.Id == id);

        if (review is null)
            return NotFound();

        _userBookReviewRepository.Delete(review);

        await _userBookReviewRepository.CommitAsync(ct);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Delete review successfully"
        });
    }
}