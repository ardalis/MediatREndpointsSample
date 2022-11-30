using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// See: https://www.youtube.com/watch?v=euUg_IHo7-s&ab_channel=NickChapsas

builder.Services.AddControllers();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// add each endpoint like this
app.MediateGet<PersonRequest>("people/{name}");

app.MapGet("person/{name}", ([AsParameters] PersonRequest request) =>
{
	return Results.Ok(new
	{
		message = $"The age was: {request.Age} and name was {request.Name}"
	});
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class PersonRequest : IHttpRequest
{
	public int Age { get; set; }
	public string Name { get; set; } = "";
}

public interface IHttpRequest : IRequest<IResult> { }

public class PersonRequestHandler : IRequestHandler<PersonRequest, IResult>
{
	public async Task<IResult> Handle(PersonRequest request, CancellationToken cancellationToken)
	{
		await Task.Delay(10, cancellationToken);
		return Results.Ok(new
		{
			message = $"The age was: {request.Age} and name was {request.Name}"
		});
	}
}

public static class EndpointExtensions
{
	public static WebApplication MediateGet<TRequest>(
		this WebApplication app,
		string template) where TRequest : IHttpRequest
	{
		app.MapGet(template, async (IMediator mediator,
			[AsParameters] TRequest request) => await mediator.Send(request));
		return app;
	}
}

// example way to organize requests with handlers using static classes
//public static class GetUsers
//{
//	public record Query(int Age) : IRequest<xxx>;
//	public class Handler : IRequestHandler<Query, xxx> { ... }
//}
