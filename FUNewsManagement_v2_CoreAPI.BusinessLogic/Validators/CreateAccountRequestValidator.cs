using FluentValidation;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Validators
{
    /// <summary>
    /// Validator cho CreateAccountRequest
    /// </summary>
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Tên tài khoản không được để trống")
                .MaximumLength(100).WithMessage("Tên tài khoản không được vượt quá 100 ký tự"); 

            RuleFor(x => x.AccountEmail)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự");

            RuleFor(x => x.AccountPassword)
                .NotEmpty().WithMessage("Password không được để trống")
                .MinimumLength(6).WithMessage("Password phải có ít nhất 6 ký tự")
                .MaximumLength(100).WithMessage("Password không được vượt quá 100 ký tự");

            RuleFor(x => x.AccountRole)
                .NotNull().WithMessage("Role không được để trống")
                .Must(role => role == 1 || role == 2)
                .WithMessage("Role phải là 1 (Staff) hoặc 2 (Lecturer)");
        }
    }
}
