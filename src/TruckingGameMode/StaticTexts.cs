namespace TruckingGameMode
{
    public struct StaticTexts
    {
        /// <summary> Static text for houses labels
        /// <para>
        ///  {0} - First color
        ///  {1} - Second color
        ///  {2} - House id
        ///  {3} - House price
        ///  {4} - House maximum level
        /// </para>
        /// </summary>
        public static string TextHouseForSale { get; } = "{0}House: {1}{2}\n{0}Price: {1}${3:##,###}\n{0}Max Level: {1}{4}\nType /buyhouse to buy it.";

        /// <summary> Static text for houses labels
        /// <para>
        ///  {0} - First color
        ///  {1} - Second color
        ///  {2} - House id
        ///  {3} - Owner(string)
        ///  {4} - House level
        /// </para>
        /// </summary>
        public static string TextHouseOwned { get; } = "{0}House: {1}{2}\n{0}Owner: {1}{3}\n{0}Level: {1}{4}\nType /enter to enter the house.";
    }
}