namespace NewsflowApi.Data.Seeds
{
    public class StructuralSeeder(RoleSeeder roleSeeder, PermissionSeeder permissionSeeder, RolePermissionSeeder rolePermissionSeeder)
    {
        private readonly RoleSeeder _roleSeeder = roleSeeder;
        private readonly PermissionSeeder _permissionSeeder = permissionSeeder;
        private readonly RolePermissionSeeder _rolePermissionSeeder = rolePermissionSeeder;

        public async Task SeedAsync()
        {
            await _roleSeeder.SeedAsync();
            await _permissionSeeder.SeedAsync();
            await _rolePermissionSeeder.SeedAsync();
        }
    }
}
