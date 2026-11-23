using GymManagementSystem.BLL.Services.Members;
using GymManagementSystem.BLL.ViewModels.Members;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ILogger<MemberController> _logger;

        public MemberController(IMemberService memberService, ILogger<MemberController> logger)
        {
            _memberService = memberService;
            _logger = logger;
        }

        // 1. Index (GET: /Member)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var members = await _memberService.GetAllMembersAsync();
            return View(members);
        }

        // 2. Details (GET: /Member/Details/5)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // HealthRecordDetails (GET: /Member/HealthRecord/5)
        [HttpGet]
        public async Task<IActionResult> HealthRecord(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var healthRecord = await _memberService.GetHealthRecordDetailsAsync(id);
            if (healthRecord == null)
            {
                TempData["ErrorMessage"] = "Health record not found for this member.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View("HealthRecordDetails", healthRecord);
        }

        // 3. Create (GET: /Member/Create)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateMemberViewModel
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))
            });
        }

        // 4. Create (POST: /Member/Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _memberService.CreateMemberAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create member.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Member Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        // 5. Edit (GET: /Member/Edit/5)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var member = await _memberService.GetMemberForEditAsync(id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // 6. Edit (POST: /Member/Edit/5)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditMemberViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _memberService.UpdateMemberAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update member.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Member Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        // 7. Delete (GET: /Member/Delete/5 - Confirmation Page)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var member = await _memberService.GetMemberForDeleteAsync(id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // 8. Delete (POST: /Member/Delete/5 - Commit Deletion)
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _memberService.DeleteMemberAsync(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["SuccessMessage"] = "Member Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
