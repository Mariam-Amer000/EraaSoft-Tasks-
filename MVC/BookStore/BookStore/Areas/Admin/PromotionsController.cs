namespace BookStore.Areas.Admin;

[Route("api/[area]/[controller]")]
[ApiController]
[Area("Admin")]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN},{RoleConstants.ADMIN}")]
public class PromotionsController : ControllerBase
{
    private readonly IRepository<Promotion> _promotionRepository;

    public PromotionsController(IRepository<Promotion> promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    [HttpGet]
    [Route(nameof(Get))]
    public IActionResult Get()
    {
        var promotions = _promotionRepository.Get();

        return Ok(promotions);
    }

    [HttpGet]
    [Route(nameof(GetOne))]
    public IActionResult GetOne(int id)
    {
        var promotion = _promotionRepository.GetOne(e => e.Id == id);

        if (promotion is null)
            return NotFound();

        return Ok(promotion);
    }

    [HttpPost]
    [Route(nameof(Create))]
    public async Task<IActionResult> Create(Promotion promotion, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _promotionRepository.CreateAsync(promotion, CT);

        if (!result)
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not create Promotion."
            });

        if (await _promotionRepository.CommitAsync(CT) == 0)
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not save Promotion."
            });

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Create Promotion Successfully"
        });
    }

    [HttpPut]
    [Route(nameof(Update))]
    public async Task<IActionResult> Update(int id, Promotion promotion, CancellationToken CT)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var promotionInDb = _promotionRepository.GetOne(e => e.Id == id);

        if (promotionInDb is null)
            return NotFound();

        promotionInDb.Code = promotion.Code;
        promotionInDb.DiscountPercentage = promotion.DiscountPercentage;
        promotionInDb.StartDate = promotion.StartDate;
        promotionInDb.EndDate = promotion.EndDate;
        promotionInDb.IsActive = promotion.IsActive;

        await _promotionRepository.CommitAsync(CT);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Update Promotion Successfully"
        });
    }

    [HttpDelete]
    [Route(nameof(Delete))]
    public async Task<IActionResult> Delete(int id, CancellationToken CT)
    {
        var promotion = _promotionRepository.GetOne(e => e.Id == id);

        if (promotion is null)
            return NotFound();

        _promotionRepository.Delete(promotion);

        await _promotionRepository.CommitAsync(CT);

        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Delete Promotion Successfully"
        });
    }
}