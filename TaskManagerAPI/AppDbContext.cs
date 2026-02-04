using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.models;

public class AppContext : Dbcontext
{
	public AppDbContext(DbContextOptions options)
		: base(options)
	{
	}
	/*
	public Dbset>TaskItem> Tasks(get; set;)
	{
	}
	*/
}
