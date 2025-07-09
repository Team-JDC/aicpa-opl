namespace AICPA.Destroyer.Content.Subscription
{
    /// <summary>
    ///   Summary description for Subscription.
    /// </summary>
    public interface ISubscription
    {
        ///<summary>
        ///  The subscription's unique code.
        ///</summary>
        string Code { get; }

        ///<summary>
        ///  The title of the subscription.
        ///</summary>
        string Title { get; set; }

        ///<summary>
        ///  The description of the subscription.
        ///</summary>
        string Description { get; set; }

        ///<summary>
        ///  An array of book names associated with this subscription.
        ///</summary>
        string[] BookNames { get; }

        ///<summary>
        ///  Saves the subscription
        ///</summary>
        void Save();

        ///<summary>
        ///  Adds a book to the subscription.
        ///</summary>
        void AddBook(string bookName);

        ///<summary>
        ///  Removes a book from the subscription.
        ///</summary>
        void RemoveBook(string bookName);
    }
}