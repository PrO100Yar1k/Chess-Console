using Chess_Console.Views;
using Chess_Console.Core.Board;
using Chess_Console.Core.Rules;
using Chess_Console.StateMachine.Base;
using Chess_Console.StateMachine.Manager;
using Chess_Console.StateMachine.GameState;
using Chess_Console.Core.Common.Interfaces;
using Chess_Console.Infrastructure.Inputs.Handlers;
using Chess_Console.Infrastructure.Inputs.Instances;
using Chess_Console.Infrastructure.Inputs.Interfaces;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static IServiceProvider ConfigureServices()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddSingleton<BoardModel>();
        services.AddSingleton<RulesValidator>();
        services.AddSingleton<StepController>();
        services.AddSingleton<MoveValidator>();
        services.AddSingleton<BoardRenderer>();

        services.AddTransient<MainMenuState>();
        services.AddTransient<GameplayState>();
        services.AddTransient<GameCompletionState>();

        services.AddSingleton<IPieceFactory, PieceFactory>();

        services.AddSingleton<ISwitchableState, GameStateController>();

        services.AddKeyedSingleton<IInputHandler, PlayerInputHandler>("Player Input");
        services.AddKeyedSingleton<IInputHandler, EnemyInputHandler>("Enemy Input");

        services.AddKeyedSingleton<IGeneralInput, PlayerInput>("Player Input");
        services.AddKeyedSingleton<IGeneralInput, EnemyInput>("Enemy Input");

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        InitializeAllServices(services, serviceProvider);

        RulesValidator rulesValidator = serviceProvider.GetRequiredService<RulesValidator>();
        StepController stepController = serviceProvider.GetRequiredService<StepController>();

        rulesValidator.SetStepController(stepController);

        return serviceProvider;
    }

    private static void InitializeAllServices(ServiceCollection services, IServiceProvider serviceProvider)
    {
        foreach (var serviceDescriptor in services)
        {
            if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton)
            {
                if (serviceDescriptor.IsKeyedService)
                    continue;

                var service = serviceProvider.GetRequiredService(serviceDescriptor.ServiceType);

                if (service is IServiceInitializable initializable)
                    initializable.InitializeService();
            }
        }
    }
}