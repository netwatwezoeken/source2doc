
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatrCode;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services
            .AddMediatR(cfg =>
            {
                cfg
                    .RegisterServicesFromAssembly(typeof(Program).Assembly);
            });
        var sp =services.BuildServiceProvider();

        var mediator = sp.GetRequiredService<IMediator>();
        mediator.Send(new CommandA());
        var t = await mediator.Send(new RequestA());
    }
}