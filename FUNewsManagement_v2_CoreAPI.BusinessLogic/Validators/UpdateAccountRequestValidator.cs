using FluentValidation;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Validators
{
    /// <summary>
    /// Validator cho UpdateAccountRequest
    /// </summary>
    public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountRequestValidator()
        {
            RuleFor(x => x.AccountName)
                .MaximumLength(100).WithMessage("Tên tài khoản không được vượt quá 100 ký tự")
                .When(x => !string.IsNullOrEmpty(x.AccountName));

            RuleFor(x => x.AccountEmail)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
                .When(x => !string.IsNullOrEmpty(x.AccountEmail));

            RuleFor(x => x.NewPassword)
                .MinimumLength(6).WithMessage("Password mới phải có ít nhất 6 ký tự")
                .MaximumLength(100).WithMessage("Password mới không được vượt quá 100 ký tự") 
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Phải cung cấp password cũ khi đổi password")
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            RuleFor(x => x.AccountRole)
                .Must(role => role == 1 || role == 2)
                .WithMessage("Role phải là 1 (Staff) hoặc 2 (Lecturer)")
                .When(x => x.AccountRole.HasValue);
        }
    }
}
