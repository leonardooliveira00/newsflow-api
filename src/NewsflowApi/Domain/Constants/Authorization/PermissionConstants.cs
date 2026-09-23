namespace NewsflowApi.Domain.Constants.Authorization
{
    public class PermissionConstants
    {
        public const string CreateStaff = "CREATE_STAFF";
        public const string ViewStaff = "VIEW_STAFF";
        public const string UpdateStaff = "UPDATE_STAFF";
        public const string DeactivateStaff = "DEACTIVATE_STAFF";

        public const string CreateUser = "CREATE_USER";
        public const string UpdateUser = "UPDATE_USER";
        public const string SuspendUser = "SUSPEND_USER";
        public const string ManageRole = "MANAGE_ROLE";

        public const string CreateArticle = "CREATE_ARTICLE";
        public const string EditOwnArticle = "EDIT_OWN_ARTICLE";
        public const string EditAnyArticle = "EDIT_ANY_ARTICLE";
        public const string SubmitArticle = "SUBMIT_ARTICLE";
        public const string ReviewArticle = "REVIEW_ARTICLE";
        public const string RequestArticleChanges = "REQUEST_ARTICLE_CHANGES";
        public const string ApproveArticle = "APPROVE_ARTICLE";
        public const string PublishArticle = "PUBLISH_ARTICLE";

        public const string UploadMedia = "UPLOAD_MEDIA";
        public const string ViewAnalytics = "VIEW_ANALYTICS";
    }
}
