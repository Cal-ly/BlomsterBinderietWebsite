namespace HttpWebshopCookie.Data.MockData;

public static class MockTags
{
    public static List<Tag> Tags { get; set; } = new List<Tag>
    {
        new Tag
        {
            Catergory = "Category 1",
            SubCategory = "SubTag 1"
        },
        new Tag
        {
            Catergory = "Category 2",
            SubCategory = "SubTag 2"
        },
        new Tag
        {
            Catergory = "Category 3",
            SubCategory = "SubTag 3"
        },
        new Tag
        {
            Catergory = "Category 4",
            SubCategory = "SubTag 4"
        },
        new Tag
        {
            Catergory = "Category 5",
            SubCategory = "SubTag 5"
        }
    };
}
