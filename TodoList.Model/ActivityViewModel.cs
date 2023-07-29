namespace TodoList.Model
{
    public class ActivityViewModel
    {
        public int Id { get; set; }

        public DateTime DataInserimento { get; set; }

        public string Descrizione { get; set; } = null!;

        public bool Completato { get; set; }
    }
}
