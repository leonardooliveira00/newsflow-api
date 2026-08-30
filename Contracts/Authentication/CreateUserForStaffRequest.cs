namespace NewsflowApi.Contracts.Authentication
{
    public class CreateUserForStaffRequest
    {
        public Guid StaffId { get; set; }

        public required string Email { get; set; }
    }
}
