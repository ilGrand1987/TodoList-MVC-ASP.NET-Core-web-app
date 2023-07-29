using System;
using System.Collections.Generic;

namespace TodoList.Model;

public partial class Activity
{ 

    public int Id { get; set; }

    public DateTime DataInserimento { get; set; }

    public string Descrizione { get; set; } = null!;

    public bool Completato { get; set; }
}
