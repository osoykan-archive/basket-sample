namespace BasketContext.Domain
{
	public static class Messages
	{
		public const string InvalidItemQuantityExceptionMessage = "Provided quantity for ItemId: {0} should be greater than zero";
		public const string AggregateNotFoundExceptionMessage = "Aggregate not found with Id: {0}";
		public const string ItemNotFoundExceptionMessage = "Item not found with Id: {0}";
	}
}
