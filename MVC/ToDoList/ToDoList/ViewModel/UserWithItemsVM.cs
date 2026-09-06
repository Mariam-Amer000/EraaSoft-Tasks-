namespace ToDoList.ViewModel
{
    public class UserWithItemsVM
    {
        public User User { get; set; } = null!;
        public List<Item>Items { get; set; } = new List<Item>();
    }
}
