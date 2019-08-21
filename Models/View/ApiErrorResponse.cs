namespace BasicWebAPI.Models.View
{
    public class ApiErrorResponse
    {
        private string messageTemplate;

        public string Message {get;set;}
        public int httpStatusCode {get;set;}
        public int appStatusCode {get;set;}
        public ApiErrorResponse(string messageTemplate)
        {
            this.messageTemplate = messageTemplate;
        }
        public ApiErrorResponse AddMessageParameter(string param)
        {
            Message = string.Format(messageTemplate, param);
            return this;
        }
    }
}