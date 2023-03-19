namespace PikaServer.Common.HdBankHttpApiBase;

public static class HdBankResponseCodeSpec
{
	public const string Success = "00";
	public const string Unauthorized = "01";
	public const string KeyExpiredOrNotMatch = "02";
	public const string FormatMessageInvalid = "03";
	public const string NotEnoughMoney = "04";
	public const string BankEndOfDate = "06";
	public const string BankAccountNotFound = "07";
	public const string BankAccountNotActive = "08";
	public const string BankAccountCcyInvalid = "09";
	public const string FeeNotFound = "10";
	public const string UserAlreadyExists = "11";
	public const string SystemError = "99";
}
