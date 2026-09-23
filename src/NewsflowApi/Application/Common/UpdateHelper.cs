namespace NewsflowApi.Application.Common
{
    public static class UpdateHelper
    {
        public static void UpdateIfProvided<T>(T? value, Action<T> update)
        {
            if (value is not null) update(value);
        }
    }
}
