namespace B3Alert {
    public class QuotationApiException : Exception {
        public QuotationApiException(string message, Exception innerException) 
            : base(message, innerException) 
        { 
        }
    }
}
