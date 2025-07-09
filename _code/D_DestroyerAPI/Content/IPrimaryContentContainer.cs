namespace AICPA.Destroyer.Content
{
    /// <summary>
    /// 	
    /// </summary>
    public interface IPrimaryContentContainer
    {
        /// <summary>
        ///   Gets the first document or a uri associated with this node type
        /// </summary>
        ContentWrapper PrimaryContent { get; }
    }
}