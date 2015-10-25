namespace GM.Models.Public
{
    public class UploadModel
    {
        public int UploadId { get; set; }

        public string FileName { get; set; }
        public virtual string ContentType { get; set; }
        public virtual byte[] Contents { get; set; }
    }
}
