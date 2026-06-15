namespace depi__project.viewmodels.General
{
    public class ResponseStatus<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }= new List<string>();

        public ResponseStatus(T data, string message = null, bool success = true, List<string> errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>() ;
        }
        public ResponseStatus(string message = null, bool success = true , List<string> errors=null)
        {
            Success = success;
            Message = message;
            Errors = errors??new List<string>();
        }
    }
}
