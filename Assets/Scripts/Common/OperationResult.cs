public class OperationResult
{
	public bool Success => ErrorMessage == null;
	public string ResultData;
	public string ErrorMessage;
	public long Code;
}

