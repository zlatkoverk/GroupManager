namespace GroupManager.Utilities
{
    public class UploadResult
    {
        public string Error { get; set; }

        public bool IsSuccess
        {
            get { return string.IsNullOrEmpty(Error); }
        }

        public string Url { get; set; }
    }
}