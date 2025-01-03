namespace gaco_api.Utilerias
{
    public class ClsModResult
    {
        public ClsModResult()
        {
            MsgError = string.Empty;
        }
        public dynamic? Object { get; set; }
        public string? MsgInfo { get; set; }
        public string MsgError { get; set; }
        public bool IsError
        {
            get
            {
                return MsgError != "";
            }
        }
        public dynamic? Items { get; set; }
        public int Cant { get; set; }
    }
}
